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
            var player = new Player();
            
            player.MoveRight();
            
            var tasks = new ConcurrentBag<Task>();

            for (int i = 0; i < 1000; i++)
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
}