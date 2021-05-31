using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Game
{
    public class PlayerTests
    {
        private Level Level;
        private Player Player;

        [SetUp]
        public void SetUp()
        {
            Level = LevelBuilder.BuildFromFiles("Level1.txt")[0];
            Player = new Player();
            Player.SetLevel(Level);
            Player.SetCoordinate(4000, 300);
        }

        [Test]
        public void MoveRight()
        {
            var t = Player.MoveRight();
            t.Start();
            t.Wait();
            
            Assert.AreEqual(4008, Player.X);
            Assert.AreEqual(Side.Right, Player.Side);
        }

        [Test]
        public void MoveLeft()
        {
            var t = Player.MoveLeft();
            t.Start();
            t.Wait();
            
            Assert.AreEqual(3992, Player.X);
            Assert.AreEqual(Side.Left, Player.Side);
        }

        [Test]
        public void MoveManyTimes()
        {
            var tasks = new ConcurrentBag<Task>();

            for (int i = 0; i < 100; i++)
            {
                tasks.Add(new Task(() => Player.MoveLeft()));
                tasks.Add(new Task(() => Player.MoveRight()));
            }

            foreach (var e in tasks)
            {
                e.Start();
            }

            Task.WaitAll(tasks.ToArray());
            
            Assert.AreEqual(4000, Player.X);
        }

        [Test]
        public void Fall()
        {
            Player.Fall().Start();
            
            Assert.AreEqual(4000, Player.X);
            Assert.AreNotEqual(300, Player.Y);
        }

        [Test]
        public void FallOnGround()
        {
            var t = Player.Fall();
            t.Start();
            t.Wait();

            var y = Player.Y;
            
            Player.Fall().Start();
            
            Assert.AreEqual(y, Player.Y);
        }

        [Test]
        public void Jump()
        {
            var t = Player.Fall();
            t.Start();
            t.Wait();

            var y = Player.Y;

            Player.Jump().Start();
            
            Thread.Sleep(5);
            Assert.AreNotEqual(y, Player.Y);
            Assert.AreEqual(4000, Player.X);
        }

        [Test]
        public void JumpInSamePoint()
        {
            var t = Player.Fall();
            t.Start();
            t.Wait();

            var y = Player.Y;
            
            var t1 = Player.Jump();
            t1.Start();
            t1.Wait();

            var t2 = Player.Fall();
            t2.Start();
            t2.Wait();
            
            
            Assert.AreEqual(y, Player.Y, 5);
        }
    }
}