using System.Net.Sockets;
using System.Net;
using System.Text;
using TCPTesting.Services;

namespace TCPTesting
{
    public partial class MainPage : ContentPage
    {
        private ITCPService _TCPService;
        private Socket? _socket;
        private bool isOpen = false;
        public MainPage(ITCPService service)
        {
            InitializeComponent();
            _TCPService = service;
        }

        private async void OpenSocket_Clicked(object sender, EventArgs e)
        {
            string formipAddress = IpEntry.Text.Trim();
            errorLabel.Text = string.Empty;

            var data = formipAddress.Split(":").ToList();

            if(data.Count != 2)
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
                    await _socket!.ConnectAsync(_TCPService.getEndPoint()!);
                    isOpen = true;
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
            string formMessage = MessageEntry.Text.Trim();
            infoLabel.Text = "";

            if (isOpen)
            {

                var messageBytes = Encoding.UTF8.GetBytes(formMessage);
                _ = await _socket!.SendAsync(messageBytes, SocketFlags.None);

                infoLabel.Text = $"Socket client sent message: \"{formMessage}\"";


                var buffer = new byte[1_024];
                var received = await _socket!.ReceiveAsync(buffer, SocketFlags.None);
                var response = Encoding.UTF8.GetString(buffer, 0, received);
                Thread.Sleep(1000);

                receiveLabel.Text = $"Socket client received: \"{response}\"";
            }
        }
    }
}
