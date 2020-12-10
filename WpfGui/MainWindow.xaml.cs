namespace AssimilationSoftware.MediaSync.WpfGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel();
        }

        public MainViewModel ViewModel
        {
            get => (MainViewModel) DataContext;
            set => DataContext = value;
        }
    }
}
