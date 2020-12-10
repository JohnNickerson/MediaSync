using System.Windows.Input;

namespace AssimilationSoftware.MediaSync.WpfGui
{
    public class ConfigViewModel : ViewModelBase
    {
        private RelayCommand _browseCommand;

        private string _sharedFolder;
        private string _thisMachine;

        public ICommand BrowseCommand => _browseCommand ?? (_browseCommand = new RelayCommand(BrowseExecute));

        public string SharedFolder
        {
            get => _sharedFolder;
            set
            {
                if (_sharedFolder == value) return;
                _sharedFolder = value;
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
            var fdlg = new System.Windows.Forms.FolderBrowserDialog();
            fdlg.SelectedPath = SharedFolder;
            fdlg.ShowNewFolderButton = true;
            fdlg.Description = "Shared data folder";
            var answer = fdlg.ShowDialog();
            if (answer == System.Windows.Forms.DialogResult.OK)
            {
                SharedFolder = fdlg.SelectedPath;
            }
        }
    }
}
