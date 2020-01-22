using System.Windows.Input;

namespace AssimilationSoftware.MediaSync.WpfGui
{
    public class ConfigViewModel : ViewModelBase
    {
        private RelayCommand _browseCommand;

        private string _dataFile;
        private string _thisMachine;

        public ICommand BrowseCommand => _browseCommand ?? (_browseCommand = new RelayCommand(BrowseExecute));

        public string DataFile
        {
            get => _dataFile;
            set
            {
                if (_dataFile == value) return;
                _dataFile = value;
                OnPropertyChanged();
            }
        }

        public string ThisMachine
        {
            get => _thisMachine;
            set
            {
                if (_thisMachine == value) return;
                _thisMachine = value;
                OnPropertyChanged();
            }
        }

        public void BrowseExecute()
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                FileName = "SyncData",
                DefaultExt = ".db",
                Filter = "MediaSync data file (.db)|*.db|All documents (*.*)|*.*",
                Title = "Data File Location",
                CheckFileExists = false
            };

            // Show open file dialog box
            var result = dlg.ShowDialog();
            if (result == true)
            {
                DataFile = dlg.FileName;
            }
        }


    }
}
