using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using AssimilationSoftware.MediaSync.Core;
using AssimilationSoftware.MediaSync.Core.FileManagement;
using AssimilationSoftware.MediaSync.Core.FileManagement.Hashing;
using AssimilationSoftware.MediaSync.Core.Interfaces;
using AssimilationSoftware.MediaSync.Core.Mappers.LiteDb;
using AssimilationSoftware.MediaSync.Core.Model;
using AssimilationSoftware.MediaSync.WpfGui.Properties;

namespace AssimilationSoftware.MediaSync.WpfGui
{
    public class MainViewModel : ViewModelBase
    {
        private RelayCommand _runAllCommand;
        private RelayCommand _cancelRunCommand;
        private RelayCommand _closeCommand;
        private RelayCommand _configCommand;
        private string _outputText;
        private ViewModel _api;
        private List<SyncSet> _profiles;
        private bool _isRunning;

        public MainViewModel()
        {
            // Check whether we are configured yet.
            if (!Settings.Default.Configured)
            {
                ConfigExecute();
            }

            ThisMachine = Settings.Default.ThisMachine;
            // Load the profiles for display.
            var mapper = new LiteDbSyncSetMapper(Settings.Default.DataFile);
            _api = new ViewModel(mapper, ThisMachine, new SimpleFileManager(new Sha1Calculator()));
            Profiles = _api.Profiles;
        }

        public string ThisMachine { get; set; }

        public void RunAllExecute()
        {
            // Run the profiles in another thread.
            var windowLogger = new WindowLogger(this);
            IsRunning = true;
            Task.Run(() => _api.RunSync(false, windowLogger));
            {
                var flasher = new FlashWindowHelper(Application.Current);
                flasher.FlashApplicationWindow();
            }
        }

        private void CancelRunExecute()
        {
            IsRunning = false;
            _api.StopSync();
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
            var configVm = new ConfigViewModel();
            configVm.ThisMachine = Settings.Default.ThisMachine.Replace("{System.Environment.MachineName}", Environment.MachineName);
            configVm.DataFile = Settings.Default.DataFile;
            configView.DataContext = configVm;
            //TODO: configView.Owner = this.Window;
            var result = configView.ShowDialog();
            if (result.HasValue && result.Value)
            {
                Settings.Default.ThisMachine = configVm.ThisMachine;
                Settings.Default.DataFile = configVm.DataFile;
                Settings.Default.Save();
            }
            else
            {
                // Just to be safe.
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

        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                if (_isRunning == value) return;
                _isRunning = value;
                OnPropertyChanged();
            }
        }

        public ICommand RunAllCommand => _runAllCommand ?? (_runAllCommand = new RelayCommand(RunAllExecute));

        public ICommand CancelRunCommand => _cancelRunCommand ?? (_cancelRunCommand = new RelayCommand(CancelRunExecute, () => IsRunning));

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
