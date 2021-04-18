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
        public int X { get; private set; }
        public int Y { get; private set; }
        public IWeapon Weapon { get; private set; }
        
        public ILevel Level { get; }

        public Player()
        {
            Level = null;
        }
        
        public Player(ILevel level)
        {
            Level = level;
        }
        
        public void MoveRight() => Move(() => X += 1);

        public void MoveLeft() => Move(() => X -= 1);
        
        public void Jump()
        {
            var task = Move(() =>
            {
                Y -= 8;
                Thread.Sleep(10);
            });
            task.ContinueWith(task1 => Y -= 8);
        }
        
        private Task Move(Action action)
        {
            var task = new Task(action);
            task.Start();
            return task;
        }

        private Task MoveUp()
        {
            var task = new Task(() => X -= 1);
            task.Start();
            return task;
        }

        private void MoveDown()
        {
            for (int i = 0; i < 10; i++)
                if (Level.CheckCoordinate(X, Y - 5))
                    Y -= 5;
                else
                    return;
        }

        public void Damage()
        {
            
        }
    }
}