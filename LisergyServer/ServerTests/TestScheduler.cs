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
            public TestTask(TimeSpan delay) : base(delay) {}
            public bool Ran = false;
            public override void Execute()
            {
                if (Ran)
                    throw new Exception("Tryng to run task twice");
                Ran = true;
            }
        }

        [SetUp]
        public void Setup()
        {
            GameScheduler.SetLogicalTime(DateTime.UtcNow);
            GameScheduler.Clear();
        }

        [Test]
        public void TestOrdering()
        {
            GameScheduler.SetLogicalTime(DateTime.UnixEpoch);

            var t1 = new TestTask(TimeSpan.FromSeconds(1));
            var t2 = new TestTask(TimeSpan.FromSeconds(2));
            var t3 = new TestTask(TimeSpan.FromSeconds(3));

            var minute = (long)GameScheduler.NowTimespan.TotalMinutes;
            var queue = GameScheduler.GetMinuteQueue(minute);

            Assert.AreEqual(t1, queue.First());
            Assert.AreEqual(t3, queue.Last());
            Assert.AreEqual(3, GameScheduler.PendingTasks);
        }

        [Test]
        public void TestMinuteQueues()
        {
            var time = DateTime.UnixEpoch;
            GameScheduler.SetLogicalTime(time);

            var t1 = new TestTask(TimeSpan.FromSeconds(1));
            var t2 = new TestTask(TimeSpan.FromSeconds(1 + 60));
            var t3 = new TestTask(TimeSpan.FromSeconds(1 + 120));

            var minute = (long)GameScheduler.NowTimespan.TotalMinutes;

            Assert.IsTrue(GameScheduler.GetMinuteQueue(minute).Contains(t1));
            Assert.IsTrue(GameScheduler.GetMinuteQueue(minute + 1).Contains(t2));
            Assert.IsTrue(GameScheduler.GetMinuteQueue(minute + 2).Contains(t3));
        }

        [Test]
        public void TestTickNotRunningTasks()
        {
            var time = DateTime.UnixEpoch;
            GameScheduler.SetLogicalTime(time);

            var t1 = new TestTask(TimeSpan.FromSeconds(1));
            var t2 = new TestTask(TimeSpan.FromSeconds(1 + 60));
            var t3 = new TestTask(TimeSpan.FromSeconds(1 + 120));

            GameScheduler.Tick(time);

            Assert.IsFalse(t1.Ran);
            Assert.IsFalse(t2.Ran);
            Assert.IsFalse(t3.Ran);
            Assert.AreEqual(t1, GameScheduler.NextTask);
        }

        [Test]
        public void TestRunningTask()
        {
            var time = DateTime.UnixEpoch;
            GameScheduler.SetLogicalTime(time);
            GameScheduler.Tick(time);

            var t1 = new TestTask(TimeSpan.FromSeconds(1));
            var t2 = new TestTask(TimeSpan.FromSeconds(1 + 60));
            var t3 = new TestTask(TimeSpan.FromSeconds(1 + 120));

            GameScheduler.Tick(time + TimeSpan.FromMinutes(1));

            Assert.IsTrue(t1.Ran);
            Assert.IsFalse(t2.Ran);
            Assert.IsFalse(t3.Ran);
            Assert.AreEqual(t2, GameScheduler.NextTask);
            Assert.AreEqual(2, GameScheduler.PendingTasks);
        }

        [Test]
        public void TestRunningTwoTasks()
        {
            var time = DateTime.UnixEpoch;
            GameScheduler.SetLogicalTime(time);

            var t1 = new TestTask(TimeSpan.FromSeconds(1));
            var t2 = new TestTask(TimeSpan.FromSeconds(5));
            var t3 = new TestTask(TimeSpan.FromSeconds(7));

            GameScheduler.Tick(time + TimeSpan.FromSeconds(5));

            Assert.IsTrue(t1.Ran);
            Assert.IsTrue(t2.Ran);
            Assert.IsFalse(t3.Ran);
            Assert.AreEqual(t3, GameScheduler.NextTask);
            Assert.AreEqual(1, GameScheduler.PendingTasks);
        }

        [Test]
        public void TestRunningVeryOldTasks()
        {
            var time = DateTime.UnixEpoch;
            GameScheduler.SetLogicalTime(time);

            var t1 = new TestTask(TimeSpan.FromSeconds(1));
            var t2 = new TestTask(TimeSpan.FromHours(12));
            var t3 = new TestTask(TimeSpan.FromDays(2));

            GameScheduler.Tick(time + TimeSpan.FromDays(1));

            Assert.IsTrue(t1.Ran);
            Assert.IsTrue(t2.Ran);
            Assert.IsFalse(t3.Ran);
            Assert.AreEqual(null, GameScheduler.NextTask);
            Assert.AreEqual(1, GameScheduler.PendingTasks);
        }
    }
}