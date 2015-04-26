using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using NUnit.Framework;
using RxStreamManagement.Server.DataGenerator;

namespace RxStreamManagement.Server.Tests
{
    [TestFixture]
    public class MarginUpdateProcessorTests
    {
        private Subject<MarginUpdate> _source;
        private TestScheduler _scheduler;
        private List<int> _results;

        [SetUp]
        public void Setup()
        {
            _source = new Subject<MarginUpdate>();
            _scheduler = new TestScheduler();

            _results = new List<int>();
            var bufferLength = TimeSpan.FromTicks(50);
            var processFrequency = TimeSpan.FromTicks(20);

            _source
                .HighestRolling(bufferLength, processFrequency, _scheduler)
                .Subscribe(_results.Add);

            _scheduler.ScheduleRelative(10, () => _source.OnNext(MarginUpdatetGenerator.WithMargin(1)));
            // onnexts result @ 20
            _scheduler.ScheduleRelative(30, () => _source.OnNext(MarginUpdatetGenerator.WithMargin(2)));
            // onnexts result @ 40
            _scheduler.ScheduleRelative(50, () => _source.OnNext(MarginUpdatetGenerator.WithMargin(1)));
            // onnexts result @ 60
            _scheduler.ScheduleRelative(70, () => _source.OnNext(MarginUpdatetGenerator.WithMargin(4)));
            // onnexts result @ 80
            _scheduler.ScheduleRelative(70, () => _source.OnNext(MarginUpdatetGenerator.WithMargin(1)));
            // onnexts result @ 100
            _scheduler.ScheduleRelative(110, () => _source.OnNext(MarginUpdatetGenerator.WithMargin(3)));
            // onnexts result @ 120
            _scheduler.ScheduleRelative(130, () => _source.OnNext(MarginUpdatetGenerator.WithMargin(2)));
            // onnexts result @ 140
        }

        [Test]
        public void InitialValuesAreNotPropagatedBeforeProcessFrequencyTime()
        {
            _scheduler.AdvanceTo(15);
            _results.Should().BeEmpty();
        }

        [Test]
        public void ValuesAreBufferedCorrectly()
        {
            // buffered value flushed
            _scheduler.AdvanceTo(25);
            _results.Count.ShouldBeEquivalentTo(1);
            _results.Last().ShouldBeEquivalentTo(1);

            // source has onnext-ed, but output held by buffer
            _scheduler.AdvanceTo(35);
            _results.Count.ShouldBeEquivalentTo(1);

            // hits buffer flush
            _scheduler.AdvanceTo(45);
            _results.Count.ShouldBeEquivalentTo(2);
            _results.Last().ShouldBeEquivalentTo(2);
        }

        [Test]
        public void HighestValueInBufferIsRetained()
        {
            // even thought @t=50 onnext(1) occurs, the highest
            // value in the buffer (2) is onext-ed
            _scheduler.AdvanceTo(65);
            _results.Last().ShouldBeEquivalentTo(2);
        }

        [Test]
        public void RollingBufferDropsHighValueOnceExpired()
        {
            // check current highest value
            _scheduler.AdvanceTo(90);
            _results.Last().ShouldBeEquivalentTo(4);

            // previous high value expires
            _scheduler.AdvanceTo(145);
            _results.Last().ShouldBeEquivalentTo(3);
        }
    }
}
