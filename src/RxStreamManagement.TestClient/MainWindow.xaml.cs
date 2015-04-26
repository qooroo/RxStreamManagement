using System.Windows;
using Microsoft.AspNet.SignalR.Client;

namespace RxStreamManagement.TestClient
{
    public partial class MainWindow
    {
        private HubConnection _hubConnection;
        private IHubProxy _hubProxy;

        public MainWindow()
        {
            InitializeComponent();

            ConnectSignalR();
        }

        private void ConnectSignalR()
        {
            _hubConnection = new HubConnection("http://localhost:8888");
            _hubProxy = _hubConnection.CreateHubProxy("MarginHub");
            _hubConnection.Start();

            _hubProxy.On("update", i => Dispatcher.Invoke(() => Result.Text = i.ToString()));
        }

        private void GetAccounts(object sender, RoutedEventArgs e)
        {
            _hubProxy.Invoke("GetMarginUpdates", new[] {1, 2});
            GetAccountsButton.IsEnabled = false;
        }
    }
}
