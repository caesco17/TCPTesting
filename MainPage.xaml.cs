using System.Net.Sockets;
using System.Net;
using System.Text;
using TCPTesting.Services;
using System.IO;

namespace TCPTesting
{
    public partial class MainPage : ContentPage
    {
        private ITCPService _TCPService;
        private Socket? _socket;
        private bool isOpen = false;
        private string? _resultfile = string.Empty;

        public MainPage(ITCPService service)
        {
            InitializeComponent();
            _TCPService = service;
        }

        private void OpenSocket_Clicked(object sender, EventArgs e)
        {
            string formipAddress = IpEntry.Text.Trim();
            errorLabel.Text = string.Empty;

            var data = formipAddress.Split(":").ToList();

            if (data.Count != 2)
            {
                errorLabel.Text = "Invalid IP.";
                return;
            }

            string baseIp = data[0];
            int basePort= Convert.ToInt32(data[1].ToString());

            try
            {
                if (isOpen)
                {
                    _socket!.Shutdown(SocketShutdown.Both);
                    isOpen = false;
                }
                else
                {
                    _socket = _TCPService.GetSocket(baseIp, basePort);
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
            isOpen = true;

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
                    _socket.Disconnect(true);
                }
                else if (formMessage.Length > 0)
                {
                    var messageBytes = Encoding.UTF8.GetBytes(formMessage);
                    _ = await _socket!.SendAsync(messageBytes, SocketFlags.None);
                    infoLabel.Text = $"Socket client sent message: \"{messageBytes}\"";
                    await progressBar.ProgressTo(0.75, 500, Easing.Linear);
                    _socket.Disconnect(true);
                }
                else
                {
                    _socket.Disconnect(true);
                    return;
                }


                //var buffer = new byte[1_024];
                //var received = await _socket!.ReceiveAsync(buffer, SocketFlags.None);
                //var response = Encoding.UTF8.GetString(buffer, 0, received);
                //Thread.Sleep(1000);

                //receiveLabel.Text = $"Socket client received: \"{response}\"";
                await progressBar.ProgressTo(1, 500, Easing.Linear);
                isOpen = false;
            }
        }

        private async void  Button_Clicked(object sender, EventArgs e)
        {
            var files = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Pick file"
            });

            var _source = await files!.OpenReadAsync();

            StreamReader reader = new StreamReader(_source, System.Text.Encoding.UTF8);
            _resultfile = reader.ReadToEnd();
        }
    }
}
