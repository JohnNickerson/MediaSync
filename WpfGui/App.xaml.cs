using System.IO;
using System.Windows;
using AssimilationSoftware.MediaSync.WpfGui.Properties;

namespace AssimilationSoftware.MediaSync.WpfGui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public void OnStartup(object sender, StartupEventArgs e)
        {
            // Note: WPF closes the application if a single window is created, shown, then closed before any other windows are created.
            // To work around this, we need to create the main window first, before the configuration window is shown.
            var view = new MainWindow();
            if (!Settings.Default.Configured || !Directory.Exists(Settings.Default.SharedPath))
            {
                var o = new ConfigWindow() { DataContext = Settings.Default };
                var result = o.ShowDialog();
                if (result ?? false)
                {
                    Settings.Default.Configured = true;
                    Settings.Default.Save();
                }
                else
                {
                    Current.Shutdown();
                }
            }

            if (!Directory.Exists(Settings.Default.SharedPath))
            {
                var r = MessageBox.Show("Shared folder does not exist. Create it now?", "Create shared folder?", MessageBoxButton.YesNo);
                if (r == MessageBoxResult.Yes)
                {
                    Directory.CreateDirectory(Settings.Default.SharedPath);
                }
                else
                {
                    Settings.Default.SharedPath = ".";
                    Settings.Default.Configured = false; // To reconfigure on next start.
                    Settings.Default.Save();
                }
            }

            var vm = new MainViewModel();
            view.ViewModel = vm;
            vm.View = view;
            view.Show();
        }

        public void OnQuit(object sender, ExitEventArgs e)
        {
            Settings.Default.Save();
        }
    }
}
