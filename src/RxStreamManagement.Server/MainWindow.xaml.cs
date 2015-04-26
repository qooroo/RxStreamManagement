using System;
using System.Reactive.Linq;
using RxStreamManagement.Server.DataGenerator;

namespace RxStreamManagement.Server
{
    public partial class MainWindow
    {
        private MarginUpdatetGenerator _generator;

        public MainWindow()
        {
            InitializeComponent();

            AllMargins.TextChanged += (_, __) => AllMargins.ScrollToEnd();

            Observable.Interval(TimeSpan.FromSeconds(1))
                .ObserveOnDispatcher()
                .Subscribe(_ => Clock.Text = DateTime.Now.ToString("HH:mm:ss"));

            _generator = new MarginUpdatetGenerator();

            Run();
        }

        private void Run()
        {
            var source = _generator.MarginUpdateStream().Publish();

            source.ObserveOnDispatcher().Subscribe(i => AllMargins.Text += i.Margin + Environment.NewLine);

            source
                .HighestRolling(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(2))
                .ObserveOnDispatcher()
                .Subscribe(maxValue => HighMargin.Text = maxValue.ToString());

            source.Connect();
        }
    }
}
