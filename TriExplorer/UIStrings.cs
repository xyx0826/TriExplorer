using System;
using System.ComponentModel;
using System.Windows;
using TriExplorer.Utils;

namespace TriExplorer
{
    class UIStrings : INotifyPropertyChanged
    {
        #region Singleton
        static UIStrings _singleInstance;

        private UIStrings()
        {
            IsCurrentNodeFile = false;
            _loadingText = "Initializing...";
            _loadingProgMax = 1;
            _isPathBtnEnabled = false;
        }

        public static UIStrings GetInstance()
        {
            if (_singleInstance == null) _singleInstance = new UIStrings();
            return _singleInstance;
        }
        #endregion

        #region Binding implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        #region Loading bar StatusBar
        string _loadingText;
        int _loadingProgMax;
        int _loadingProgValue;
        bool _isPathBtnEnabled;

        /// <summary>
        /// Text on the left of status bar.
        /// </summary>
        public string LoadingText
        {
            get { return _loadingText; }
            set
            {
                _loadingText = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Max value of progress bar on status bar.
        /// </summary>
        public int LoadingProgMax
        {
            get { return _loadingProgMax; }
            set
            {
                _loadingProgMax = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Current value of progress bar on status bar.
        /// </summary>
        public int LoadingProgValue
        {
            get { return _loadingProgValue; }
            set
            {
                _loadingProgValue = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Whether the path selection button on status bar is enabled.
        /// </summary>
        public bool IsPathBtnEnabled
        {
            get { return _isPathBtnEnabled; }
            set
            {
                _isPathBtnEnabled = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Details StackPanel
        string _currentNodeType;
        string _currentFileName;
        int _currentFileSize;
        int _currentFileCompressedSize;
        string _currentFileHash;

        public bool IsCurrentNodeFile;

        /// <summary>
        /// Type of current TreeView node: 
        /// can be a directory or a specific file type.
        /// </summary>
        public string CurrentNodeType
        {
            get { return _currentNodeType; }
            set
            {
                _currentNodeType = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Name of current TreeView node:
        /// only applicable to file nodes.
        /// </summary>
        public string CurrentFileName
        {
            get
            {
                if (IsCurrentNodeFile)
                {
                    // A single underscore won't be displayed; "access key" feature of Label
                    if (_currentFileName != null) return _currentFileName.Replace("_", "__");
                    else return _currentFileName;
                }
                else return "";
            }
            set
            {
                _currentFileName = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Size of current TreeView node: 
        /// only applicable to file nodes.
        /// </summary>
        public string CurrentFileSize
        {
            get { return (IsCurrentNodeFile ? FileHelper.ToFileSize(_currentFileSize) : ""); }
            set
            {
                Int32.TryParse(value, out _currentFileSize);
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Compressed size of current TreeView node: 
        /// only applicable to file nodes.
        /// </summary>
        public string CurrentFileCompressedSize
        {
            get { return (IsCurrentNodeFile ? FileHelper.ToFileSize(_currentFileCompressedSize) : ""); }
            set
            {
                Int32.TryParse(value, out _currentFileCompressedSize);
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Hash of current TreeView node: 
        /// only applicable to file nodes.
        /// </summary>
        public string CurrentFileHash
        {
            get { return (IsCurrentNodeFile ? _currentFileHash : ""); }
            set
            {
                _currentFileHash = value;
                OnPropertyChanged();
            }
        }

        public Visibility PathButtonVisibility
        {
            get { return (IsCurrentNodeFile ? Visibility.Visible : Visibility.Collapsed); }
        }
        #endregion
    }
}
