using System.Net.Sockets;
using System.Net;
using System.Text;
using TCPTesting.Services;
using System.IO;
using LukeMauiFilePicker;

namespace TCPTesting
{
    public partial class MainPage : ContentPage
    {
        private ITCPService _TCPService;
        private Socket? _socket;
        public bool isOpen = false;
        private string? _resultfile = string.Empty;
        private IFilePickerService _filePickerService;

        public MainPage(ITCPService service, IFilePickerService filePickerService)
        {
            InitializeComponent();
            _TCPService = service;
            _filePickerService = filePickerService;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        private void OpenSocket_Clicked(object sender, EventArgs e)
        {
            string baseIp = string.Empty;
            int basePort = 0;
            string formipAddress = IpEntry.Text.Trim();

            errorLabel.Text = string.Empty;

            var data = formipAddress.Split(":").ToList();

            if (data.Count != 2)
            {
                errorLabel.Text = "Invalid IP.";
                return;
            }

            if (Int32.TryParse(data[1].ToString(), out basePort))
            {
                baseIp = data[0];
            }
            else
            {
                errorLabel.Text = "Invalid Port.";
                return;
            }

            try
            {
                if (isOpen)
                {
                    _socket!.Shutdown(SocketShutdown.Both);
                    isOpen = false;
                    IsSocketOpen.IsChecked = false;
                    btnSocket.Text = "Open Socket";
                }
                else
                {
                    _socket = _TCPService.GetSocket(baseIp, basePort);
                    isOpen = true;
                    IsSocketOpen.IsChecked = true;
                    btnSocket.Text = "Close Socket";
                }

            }
            catch (Exception ex)
            {
                errorLabel.Text = ex.Message.ToString();
                throw;
            }
        }

        private async void SendMessage_Clicked(object sender, EventArgs e)
        {
            await _socket!.ConnectAsync(_TCPService.getEndPoint()!);
            //StartSocketListener(_socket);
            await progressBar.ProgressTo(0, 500, Easing.Linear);

            string formMessage = MessageEntry.Text;
            infoLabel.Text = "";

            if (isOpen)
            {
                if (_resultfile!.Length > 0)
                {
                    var messageBytes = Encoding.UTF8.GetBytes(_resultfile);
                    _ = await _socket!.SendAsync(messageBytes, SocketFlags.None);
                    infoLabel.Text = $"Socket client sent message: \"{messageBytes}\"";
                    _resultfile = string.Empty;
                    await progressBar.ProgressTo(0.75, 500, Easing.Linear);
                    //Task.Delay(5000);
                    _socket.Disconnect(true);
                }
                else if (formMessage.Length > 0)
                {
                    var messageBytes = Encoding.UTF8.GetBytes(formMessage);
                    _ = await _socket!.SendAsync(messageBytes, SocketFlags.None);
                    infoLabel.Text = $"Socket client sent message: \"{formMessage}\"";
                    await progressBar.ProgressTo(0.75, 500, Easing.Linear);
                    _socket.Disconnect(true);
                }
                else
                {
                    _socket.Disconnect(true);
                    return;
                }
                await progressBar.ProgressTo(1, 500, Easing.Linear);
            }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            var files = await _filePickerService.PickFileAsync("Select File", null);

            var _source = await files!.OpenReadAsync();

            StreamReader reader = new StreamReader(_source, System.Text.Encoding.UTF8);
            _resultfile = reader.ReadToEnd();
        }

        private async void StartSocketListener(Socket OpenedSocket)
        {
            // Start listening for messages
            while (OpenedSocket.Connected)
            {
                byte[] buffer = new byte[1024];
                int bytesRead = await OpenedSocket.ReceiveAsync(buffer, SocketFlags.None);
                if (bytesRead > 0)
                {
                    string message = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    // Update UI on the UI thread
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        // Update the TextBox with the received message
                        ListenerText.Text += message + Environment.NewLine;
                    });
                }
            }
        }
    }
}
