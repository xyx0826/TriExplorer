using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using TriExplorer.Properties;
using TriExplorer.Types;
using TriExplorer.Utils;

namespace TriExplorer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public string WindowTitle => GetType().Namespace;

        public MainWindow()
        {
            InitializeComponent();
            InitializeSettings();
        }

        /// <summary>
        /// Initialization here: read settings etc
        /// </summary>
        private void InitializeSettings()
        {
            // Attempt to validate/read settings
            SharedCacheReader.ValidateSCPath();
        }
        
        /// <summary>
        /// Fire on window loaded: call to prepare file tree
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = new
            {
                WindowTitle
            };

            await PrepareSharedCache();
        }

        /// <summary>
        /// Force a reload of shared cache with custom path.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ReloadSharedCache(object sender, RoutedEventArgs e)
        {
            await PrepareSharedCache(true);
        }

        /// <summary>
        /// Fire on app close: save settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            Settings.Default.Save();
        }

        /// <summary>
        /// Prepare shared cache file tree and ask for valid path when necessary.
        /// </summary>
        /// <returns></returns>
        private async Task PrepareSharedCache(bool forced = false)
        {
            // Call once to use registry value if avail, or force set invalid
            if (!forced) SharedCacheReader.ValidateSCPath();
            else Settings.Default.IsSCPathValid = false;

            // If called by button, choosing path is a must
            while (!Settings.Default.IsSCPathValid)
            {
                using (var dialog = new FolderBrowserDialog())
                {
                    dialog.Description = "Please select a valid Shared Cache path.\n" +
                        "You can get this info from EVE Launcher settings.";
                    var dialogResult = dialog.ShowDialog();
                    if (dialogResult ==
                        System.Windows.Forms.DialogResult.OK &&
                        !String.IsNullOrWhiteSpace(dialog.SelectedPath))
                    {
                        SharedCacheReader.ValidateSCPath(dialog.SelectedPath);
                    }
                    if (dialogResult == System.Windows.Forms.DialogResult.Cancel)
                    {
                        if (forced) return;  // no need to quit if user was specifying another path
                        System.Windows.Application.Current.Shutdown();  // only quit when cancelled on app launch
                    }
                }
            }

            StatusBar.DataContext = UIStrings.GetInstance();
            InfoStackPanel.DataContext = UIStrings.GetInstance();

            _currentNode = null;
            // Read raw from resfileindex.txt
            // Disable path selector until loading is complete
            UIStrings.GetInstance().IsPathBtnEnabled = false;
            Debug.WriteLine("Creating SC list...");
            UIStrings.GetInstance().LoadingProgValue = 0;
            UIStrings.GetInstance().LoadingText = "Reading Shared Cache index from " + Settings.Default.SCPath + "...";
            var scIndex = await SharedCacheReader.ReadSCIndex(Settings.Default.SCPath);

            // Parse raw into inherited structure
            Debug.WriteLine("SC list created. Creating SC tree..");
            UIStrings.GetInstance().LoadingText = "Organizing " + scIndex.Count + " entries...";
            UIStrings.GetInstance().LoadingProgMax = scIndex.Count;
            var scTree = await SharedCacheParser.PopulateItemTree(scIndex);

            // Loading finished, bind DataContext and re-enable path chooser
            UIStrings.GetInstance().LoadingText = "Successfully parsed " + scIndex.Count + " entries.";
            TreeView.DataContext = new
            {
                SharedCacheTree = scTree
            };
            UIStrings.GetInstance().IsPathBtnEnabled = true;
        }

        SharedCacheNode _currentNode;

        /// <summary>
        /// Fire on TreeView node select: get info & update UI 
        /// Also saves the selected node to variable
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _currentNode = e.NewValue as SharedCacheNode;
            Debug.WriteLine($"Selecting {_currentNode.DisplayName}");

            if (_currentNode.GetType() == typeof(SharedCacheDirectory))
            {
                UIStrings.GetInstance().IsCurrentNodeFile = false;
                UIStrings.GetInstance().CurrentNodeType = "Shared Resource Path";
            }

            // Assign file node infos for display
            if (_currentNode.GetType() == typeof(SharedCacheFile))
            {
                var fileNode = _currentNode as SharedCacheFile;
                UIStrings.GetInstance().IsCurrentNodeFile = true;
                // Use predefined description, or system description if file is unknown
                var systemDesc = FileHelper.GetFileTypeDescription(fileNode.DisplayName);
                var builtInDesc = fileNode.TypeDesc;
                UIStrings.GetInstance().CurrentNodeType = (String.IsNullOrEmpty(builtInDesc) ? systemDesc : builtInDesc);

                // Size and compressed size
                UIStrings.GetInstance().CurrentFileSize = fileNode.Info.RawSize.ToString();
                UIStrings.GetInstance().CurrentFileCompressedSize = fileNode.Info.CompressedSize.ToString();

                // Disk path and MD5 hash
                UIStrings.GetInstance().CurrentFileName = fileNode.Info.FilePath;
                UIStrings.GetInstance().CurrentFileHash = fileNode.Info.Md5;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_currentNode != null && _currentNode.GetType() == typeof(SharedCacheFile))
            {
                var filePath = Path.Combine(Settings.Default.SCPath, "ResFiles",
                    (_currentNode as SharedCacheFile).Info.FilePath).Replace('/', '\\');
                Process.Start("explorer.exe", "/Select, " + filePath);
            }
        }
    }
}
