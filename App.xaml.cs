using LukeMauiFilePicker;
using TCPTesting.Services;

namespace TCPTesting
{
    public partial class App : Application
    {
        private ITCPService _tcpService;
        public App(ITCPService tcpService, IFilePickerService filePicker)
        {
            InitializeComponent();
            _tcpService = tcpService;

            MainPage = new MainPage(_tcpService, filePicker);
        }
    }
}
