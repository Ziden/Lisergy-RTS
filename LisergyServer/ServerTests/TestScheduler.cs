using Game.Scheduler;
using Game.World;
using GameDataTest;
using NUnit.Framework;
using ServerTests;
using System;
using System.Linq;

namespace Tests
{
    public class TestScheduler
    {
        private class TestTask : GameTask
        {
            public TestTask(TimeSpan delay) : base(delay) {
                ID = Guid.NewGuid();
            }
            public bool Ran = false;
            public override void Execute()
            {
                if (Ran)
                    throw new Exception("Tryng to run task twice");
                Ran = true;
            }
        }
        private GameScheduler _scheduler;

        [SetUp]
        public void Setup()
        {
            _scheduler = new GameScheduler();
            _scheduler.SetLogicalTime(DateTime.UtcNow);
        }

        [Test]
        public void TestOrdering()
        {
            _scheduler.SetLogicalTime(DateTime.UnixEpoch);

            var t1 = new TestTask(TimeSpan.FromSeconds(1));
            var t2 = new TestTask(TimeSpan.FromSeconds(2));
            var t3 = new TestTask(TimeSpan.FromSeconds(3));
            _scheduler.Add(t2);
            _scheduler.Add(t1);
            _scheduler.Add(t3);

            var minute = (long)GameScheduler.NowTimespan.TotalMinutes;
            var queue = _scheduler.GetMinuteQueue(minute);

            Assert.AreEqual(t1, queue.First());
            Assert.AreEqual(t3, queue.Last());
        }

        [Test]
        public void TestMinuteQueues()
        {
            var time = DateTime.UnixEpoch;
            _scheduler.SetLogicalTime(time);

            var t1 = new TestTask(TimeSpan.FromSeconds(1));
            var t2 = new TestTask(TimeSpan.FromSeconds(1 + 60));
            var t3 = new TestTask(TimeSpan.FromSeconds(1 + 120));
            _scheduler.Add(t2);
            _scheduler.Add(t1);
            _scheduler.Add(t3);

            var minute = (long)GameScheduler.NowTimespan.TotalMinutes;

            Assert.IsTrue(_scheduler.GetMinuteQueue(minute).Contains(t1));
            Assert.IsTrue(_scheduler.GetMinuteQueue(minute + 1).Contains(t2));
            Assert.IsTrue(_scheduler.GetMinuteQueue(minute + 2).Contains(t3));
        }

        [Test]
        public void TestTickNotRunningTasks()
        {
            var time = DateTime.UnixEpoch;
            _scheduler.SetLogicalTime(time);

            var t1 = new TestTask(TimeSpan.FromSeconds(1));
            var t2 = new TestTask(TimeSpan.FromSeconds(1 + 60));
            var t3 = new TestTask(TimeSpan.FromSeconds(1 + 120));
            _scheduler.Add(t2);
            _scheduler.Add(t1);
            _scheduler.Add(t3);

            _scheduler.Tick();

            Assert.IsFalse(t1.Ran);
            Assert.IsFalse(t2.Ran);
            Assert.IsFalse(t3.Ran);
            Assert.AreEqual(t1, _scheduler.NextTask);
        }

        [Test]
        public void TestRunningTask()
        {
            var time = DateTime.UnixEpoch;
            _scheduler.SetLogicalTime(time);
            _scheduler.Tick();

            var t1 = new TestTask(TimeSpan.FromSeconds(1));
            var t2 = new TestTask(TimeSpan.FromSeconds(1 + 60));
            var t3 = new TestTask(TimeSpan.FromSeconds(1 + 120));
            _scheduler.Add(t2);
            _scheduler.Add(t1);
            _scheduler.Add(t3);

            _scheduler.SetLogicalTime(time + TimeSpan.FromMinutes(1));
            _scheduler.Tick();

            Assert.IsTrue(t1.Ran);
            Assert.IsFalse(t2.Ran);
            Assert.IsFalse(t3.Ran);
            Assert.AreEqual(t2, _scheduler.NextTask);
        }

        [Test]
        public void TestRunningTwoTasks()
        {
            var time = DateTime.UnixEpoch;
            _scheduler.SetLogicalTime(time);
            //_scheduler.Tick();

            var t1 = new TestTask(TimeSpan.FromSeconds(1));
            var t2 = new TestTask(TimeSpan.FromSeconds(5));
            var t3 = new TestTask(TimeSpan.FromSeconds(7));
            _scheduler.Add(t2);
            _scheduler.Add(t1);
            _scheduler.Add(t3);

            _scheduler.SetLogicalTime(time + TimeSpan.FromSeconds(5));
            _scheduler.Tick();

            Assert.IsTrue(t1.Ran);
            Assert.IsTrue(t2.Ran);
            Assert.IsFalse(t3.Ran);
            Assert.AreEqual(t3, _scheduler.NextTask);
        }
    }
}