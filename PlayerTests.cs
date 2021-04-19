using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Game
{
    public class PlayerTests
    {
        [Test]
        public void MoveManyTimes()
        {
            var player = new Player(new TestLevel());
            
            player.MoveRight();
            
            var tasks = new ConcurrentBag<Task>();

            for (int i = 0; i < 10; i++)
            {
                tasks.Add(new Task(() => player.MoveLeft()));
                tasks.Add(new Task(() => player.MoveRight()));
            }

            foreach (var e in tasks)
            {
                e.Start();
            }

            Task.WaitAll(tasks.ToArray());
            
            Assert.AreEqual(1, player.X);
        }
    }

    class TestLevel : ILevel
    {
        public bool CheckRight(int x, int y)
        {
            return true;
        }

        public bool CheckLeft(int x, int y)
        {
            return true;
        }

        public bool CheckUp(int x, int y)
        {
            return true;
        }

        public bool CheckDown(int x, int y)
        {
            return y > 0;
        }
    }
}