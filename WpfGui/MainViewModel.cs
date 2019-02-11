using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using AssimilationSoftware.MediaSync.Core;
using AssimilationSoftware.MediaSync.Core.FileManagement;
using AssimilationSoftware.MediaSync.Core.FileManagement.Hashing;
using AssimilationSoftware.MediaSync.Core.Interfaces;
using AssimilationSoftware.MediaSync.Core.Mappers.XML;
using AssimilationSoftware.MediaSync.Core.Model;
using AssimilationSoftware.MediaSync.WpfGui.Properties;

namespace AssimilationSoftware.MediaSync.WpfGui
{
    public class MainViewModel : ViewModelBase
    {
        private RelayCommand _runAllCommand;
        private RelayCommand _closeCommand;
        private RelayCommand _configCommand;
        private string _outputText;
        private ViewModel _api;
        private List<SyncSet> _profiles;

        public MainViewModel()
        {
            // Check whether we are configured yet.
            if (!Settings.Default.Configured)
            {
                ConfigExecute();
            }
            // Load the profiles for display.
            _api = new ViewModel(new XmlSyncSetMapper(Settings.Default.DataFile), ThisMachine, new SimpleFileManager(new Sha1Calculator()));
            Profiles = _api.Profiles;
        }

        public string ThisMachine { get; set; }

        public void RunAllExecute()
        {
            // Run the profiles in another thread.
            var windowLogger = new WindowLogger(this);
            Task.Run(() => _api.RunSync(false, windowLogger)).Start();
        }

        public void CloseExecute()
        {
            _api.Save();
            Application.Current.Shutdown();
        }

        public void ConfigExecute()
        {
            // Open the configuration window.
            var configView = new ConfigWindow();
            configView.DataContext = Settings.Default;
            //TODO: configView.Owner = this.Window;
            var result = configView.ShowDialog();
            if (result.HasValue && result.Value)
            {
                Settings.Default.Save();
            }
            else
            {
                Settings.Default.Reload();
            }
        }

        public List<SyncSet> Profiles
        {
            get => _profiles;
            set
            {
                if (Equals(_profiles, value)) return;
                _profiles = value;
                OnPropertyChanged();
            }
        }

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

    public class WindowLogger : IStatusLogger
    {
        private readonly MainViewModel _mainViewModel;

        public WindowLogger(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }

        public int LogLevel { get; set; }
        public void Log(int level, string status, params object[] args)
        {
            _mainViewModel.OutputText += string.Format(status, args);
            _mainViewModel.OutputText += Environment.NewLine;
        }

        public void Line(int level)
        {
            _mainViewModel.OutputText += Environment.NewLine;
        }
    }
}
