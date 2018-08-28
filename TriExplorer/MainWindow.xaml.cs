using System;
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

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            bool isPathValid = SharedCacheReader.ValidateSCPath();
            // Demand a valid path or close the app
            while (!isPathValid)
            {
                using (var dialog = new FolderBrowserDialog())
                {
                    var dialogResult = dialog.ShowDialog();
                    if (dialogResult ==
                        System.Windows.Forms.DialogResult.OK &&
                        !String.IsNullOrWhiteSpace(dialog.SelectedPath))
                    {
                        isPathValid = SharedCacheReader.ValidateSCPath(dialog.SelectedPath);
                    }
                    if (dialogResult == System.Windows.Forms.DialogResult.Cancel)
                    {
                        System.Windows.Application.Current.Shutdown();
                    }
                }
            }

            var sc = await SharedCacheReader.ReadSCIndex(Settings.Default.SCPath);
            var parser = new SharedCacheParser();
            var output = parser.PopulateItemTree(sc);

            DataContext = new
            {
                TreeViewContent = output
            };
        }
        
        private void Window_Closed(object sender, EventArgs e)
        {
            Settings.Default.Save();
        }


        private void InitializeSettings()
        {
            // if (String.IsNullOrEmpty(Settings.Default.SCPath))
                // Settings.Default.SCPath = SharedCacheReader.ValidateSCPath();
        }
    }
}
