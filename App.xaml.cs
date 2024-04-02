using TCPTesting.Services;

namespace TCPTesting
{
    public partial class App : Application
    {
        private ITCPService _tcpService;
        public App(ITCPService tcpService)
        {
            InitializeComponent();
            _tcpService = tcpService;

            MainPage = new MainPage(_tcpService);
        }
    }
}
