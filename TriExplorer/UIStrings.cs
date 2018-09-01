using System;
using System.ComponentModel;

namespace TriExplorer
{
    class UIStrings : INotifyPropertyChanged
    {
        #region Singleton
        static UIStrings _singleInstance;

        private UIStrings()
        {
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
        int _currentFileSize;
        string _currentFileName;

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
        /// only applicablke to file nodes.
        /// </summary>
        public string CurrentFileName
        {
            get { return _currentFileName; }
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
            get
            {
                if (_currentFileSize == 0) return "N/A";    // not a file
                else if (_currentFileSize < 1024)    // less than 1kb
                {
                    return $"{_currentFileSize} B";
                }
                else if (_currentFileSize < 1024 * 1024)    // less than 1mb
                {
                    return $"{_currentFileSize / 1024} KB";
                }
                else if (_currentFileSize < 1024 * 1024 * 1024) // less than 1gb
                {
                    return $"{_currentFileSize / 1024 / 1024} MB";
                }
                else return _currentFileSize.ToString();
            }
            set
            {
                Int32.TryParse(value, out _currentFileSize);
                OnPropertyChanged();
            }
        }
        #endregion
    }
}
