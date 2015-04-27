using System.Linq;
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

            _hubProxy.On("update", t => Dispatcher.Invoke(() =>
            {
                foreach (var widget in Widgets.Items.Cast<Widget>().Where(item => item.WidgetId.Text == (string)t.Item1))
                {
                    widget.Result.Text = t.Item2;
                }
            }));
        }

        private void AddWidget(object sender, RoutedEventArgs e)
        {
            var w = new Widget {WidgetParent = this, HubProxy = _hubProxy};
            Widgets.Items.Add(w);
        }
    }
}
