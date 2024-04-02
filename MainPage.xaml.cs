using System.Net.Sockets;
using System.Net;
using System.Text;

namespace TCPTesting
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            string formipAddress = IpEntry.Text.Trim();
            string formMessage = MessageEntry.Text.Trim();


            var data = formipAddress.Split(":").ToList();

            string baseIp = data[0];
            int basePort= Convert.ToInt32(data[1].ToString());

            try
            {
                IPAddress ipAddress = System.Net.IPAddress.Parse(baseIp);
                IPEndPoint ipEndPoint = new(ipAddress, basePort);

                using Socket client = new(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                await client.ConnectAsync(ipEndPoint);
                while (true)
                {
                    var messageBytes = Encoding.UTF8.GetBytes(formMessage);
                    _ = await client.SendAsync(messageBytes, SocketFlags.None);

                    infoLabel.Text = $"Socket client sent message: \"{formMessage}\"";

                    var buffer = new byte[1_024];
                    var received = await client.ReceiveAsync(buffer, SocketFlags.None);
                    var response = Encoding.UTF8.GetString(buffer, 0, received);
                    Thread.Sleep(1000);

                    receiveLabel.Text = $"Socket client received acknowledgment: \"{response}\"";
                    break;
                    
                }

                client.Shutdown(SocketShutdown.Both);
            }
            catch (Exception ex)
            {
                errorLabel.Text = ex.Message.ToString();
                throw;
            }          
        }



    }

}
