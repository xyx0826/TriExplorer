using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using TriExplorer.Properties;

namespace TriExplorer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeSettings();
        }

        public string WindowTitle => GetType().Namespace;

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

        private async void ReloadSharedCache(object sender, RoutedEventArgs e)
        {
            await PrepareSharedCache(true);
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
        /// Initialization here: read settings etc
        /// </summary>
        private void InitializeSettings()
        {
            // Attempt to validate/read settings
            SharedCacheReader.ValidateSCPath();
        }

        /// <summary>
        /// Fire on TreeView node select: get info & update UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var selectedNode = e.NewValue as SharedCacheParser.SharedCacheNode;
            Debug.WriteLine($"Selecting {selectedNode.DisplayName}");

            if (selectedNode.GetType() == typeof(SharedCacheParser.SharedCacheFile))
                UIStrings.GetInstance().CurrentNodeType = "Shared resource file";
            else UIStrings.GetInstance().CurrentNodeType = "Shared resource path";

            // Only display file info for files, not directory nodes (duh)
            if (selectedNode.GetType() == typeof(SharedCacheParser.SharedCacheFile))
            {
                UIStrings.GetInstance().CurrentFileProps = "something something eve file";  // WIP
                UIStrings.GetInstance().CurrentFileSize = (selectedNode as SharedCacheParser.SharedCacheFile).Info.RawSize.ToString();
                UIStrings.GetInstance().CurrentFileName = (selectedNode as SharedCacheParser.SharedCacheFile).Info.ResName;
            }
            else
            {
                UIStrings.GetInstance().CurrentFileProps = "";
                UIStrings.GetInstance().CurrentFileSize = "";
                UIStrings.GetInstance().CurrentFileName = "";
            }
        }
    }
}
