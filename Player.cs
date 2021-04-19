using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Game
{
    public class Player
    {
        public int Health { get; private set; }
        public int X => x;
        public int Y => y;
        public IWeapon Weapon { get; private set; }
        public ILevel Level { get; }

        private int x;
        private int y;

        public Player()
        {
            Level = null;
        }
        
        public Player(ILevel level)
        {
            Level = level;
        }

        public void MoveRight() => Move(() =>
        {
            if (Level.CheckRight(x, y))
                Interlocked.Increment(ref x);
            Fall();
        });

        public void MoveLeft() => Move(() =>
        {
            if (Level.CheckLeft(x, y))
                Interlocked.Decrement(ref x);
            Fall();
        });
        
        public void Jump()
        {
            var task = Move(() =>
            {
                var force = 8;
                while (Level.CheckUp(x, y) && force > 0)
                {
                    Interlocked.Add(ref y, -force);
                    force -= 2;
                    Task.Delay(10);
                }
            });
            task.ContinueWith(task1 => Fall());
        }

        public void Fall()
        {
            var task = Move(() =>
            {
                var force = 11;
                while (Level.CheckDown(x, y))
                {
                    Interlocked.Add(ref y, force);
                    if (force >= 3)
                        force -= 2;
                    Task.Delay(force);
                }
            });
        }

        private Task Move(Action action) => Task.Run(action);

        public void Damage()
        {
            throw new NotImplementedException("пока не умеет пиздиться");
        }
    }
}