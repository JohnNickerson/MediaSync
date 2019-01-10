using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using AssimilationSoftware.MediaSync.Core.FileManagement;
using AssimilationSoftware.MediaSync.Core.FileManagement.Hashing;
using AssimilationSoftware.MediaSync.Core.Mappers.XML;
using AssimilationSoftware.MediaSync.Core.Model;
using AssimilationSoftware.MediaSync.WpfGui.Properties;
using AssimilationSoftware.TodoSort.WpfGui;

namespace AssimilationSoftware.MediaSync.WpfGui
{
    public class MainViewModel : ViewModelBase
    {
        private RelayCommand _runAllCommand;
        private RelayCommand _closeCommand;
        private RelayCommand _configCommand;
        private string _outputText;

        public MainViewModel()
        {
            // Check whether we are configured yet.
            if (!Settings.Default.Configured)
            {
                ConfigExecute();
            }
            // Load the profiles for display.
            var api = new Core.ViewModel(new XmlSyncSetMapper(Settings.Default.DataFile), ThisMachine, new SimpleFileManager(new Sha1Calculator()));
            Profiles = new List<SyncSet>();
        }

        public void RunAllExecute()
        {
            // Run the profiles in another thread.
        }

        public void CloseExecute()
        {
            Application.Current.Shutdown();
        }

        public void ConfigExecute()
        {
            // Open the configuration window.
        }

        public List<Core.Model.SyncSet> Profiles { get; set; }

        public string OutputText
        {
            get => _outputText;
            set
            {
                if (_outputText == value) return;
                _outputText = value;
                OnPropertyChanged();
            }
        }

        public ICommand RunAllCommand => _runAllCommand ?? (_runAllCommand = new RelayCommand(RunAllExecute));

        public ICommand CloseCommand => _closeCommand ?? (_closeCommand = new RelayCommand(CloseExecute));

        public ICommand ConfigCommand => _configCommand ?? (_configCommand = new RelayCommand(ConfigExecute));
    }
}
