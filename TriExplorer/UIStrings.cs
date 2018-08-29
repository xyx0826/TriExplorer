using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        #region Status bar
        string _loadingText;
        int _loadingProgMax;
        int _loadingProgValue;
        bool _isPathBtnEnabled;

        public string LoadingText
        {
            get { return _loadingText; }
            set
            {
                _loadingText = value;
                OnPropertyChanged();
            }
        }

        public int LoadingProgMax
        {
            get { return _loadingProgMax; }
            set
            {
                _loadingProgMax = value;
                OnPropertyChanged();
            }
        }

        public int LoadingProgValue
        {
            get { return _loadingProgValue; }
            set
            {
                _loadingProgValue = value;
                OnPropertyChanged();
            }
        }

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

        #region Details
        string _currentNodeType;
        string _currentFileProps;
        int _currentFileSize;
        string _currentFileName;

        public string CurrentNodeType
        {
            get { return _currentNodeType; }
            set
            {
                _currentNodeType = value;
                OnPropertyChanged();
            }
        }
        public string CurrentFileProps
        {
            get { return _currentFileProps; }
            set
            {
                _currentFileProps = value;
                OnPropertyChanged();
            }
        }
        public string CurrentFileSize
        {
            get
            {
                if (_currentFileSize < 1024)    // less than 1kb
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
        public string CurrentFileName
        {
            get { return _currentFileName; }
            set
            {
                _currentFileName = value;
                OnPropertyChanged();
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
