using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;

namespace RxStreamManagement.TestClient
{
    public partial class Widget
    {
        public MainWindow WidgetParent { get; set; }
        public IHubProxy HubProxy { get; set; }
        public Widget()
        {
            InitializeComponent();
        }

        private void SubscribeWidget(object sender, RoutedEventArgs e)
        {
            HubProxy.Invoke("GetMarginUpdates", Tuple.Create(WidgetId.Text, GetAccountNumbers()));
        }

        private List<int> GetAccountNumbers()
        {
            return AccountNumbers.Text.Split(',').Select(int.Parse).ToList();
        }
    }
}
