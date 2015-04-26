using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using RxStreamManagement.Server.DataGenerator;

namespace RxStreamManagement.Server
{
    public partial class MainWindow
    {
        private readonly DummyMarginUpdateSource _source;

        public MainWindow()
        {
            InitializeComponent();

            AllMargins.TextChanged += (_, __) => AllMargins.ScrollToEnd();

            Observable.Interval(TimeSpan.FromSeconds(1))
                .ObserveOnDispatcher()
                .Subscribe(_ => Clock.Text = DateTime.Now.ToString("HH:mm:ss"));

            _source = new DummyMarginUpdateSource();

            Run();

            var b = new Bootstrapper();
            b.Run();
        }

        private void Run()
        {
            var source = _source.MarginUpdateStream().Publish();

            source
                .ObserveOnDispatcher()
                .Subscribe(i => AllMargins.Text += i.Margin + Environment.NewLine);

            source
                .MaxValueRollingBuffer(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(2), Scheduler.Default)
                .ObserveOnDispatcher()
                .Subscribe(maxValue => HighMargin.Text = maxValue.ToString());

            source.Connect();
        }
    }
}
