using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game
{
    public class Player
    {
        public int Health => health;
        public int X => x;
        public int Y => y;
        public Side Side => (Side) side;
        public IWeapon Weapon { get; private set; }
        public ILevel Level { get; }

        public bool IsFalling { get; private set; }
        public bool IsJumping { get; private set; }
        
        public int Height { get; }
        public int Width { get; }

        private readonly Action onDeathAction;

        private int health = 10;
        private int x;
        private int y;
        private int side = 1;

        

        public Player(ILevel level = null, Action onDeathAction = null)
        {
            Height = 72;
            Width = 38;
            
            Level = level;
            this.onDeathAction = onDeathAction;
        }

        public void SetCoordinate(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Task MoveRight() => new Task(() =>
        {
            Interlocked.Exchange(ref side, 1);
            if (Level.CheckRight(x + Width + 15, y + Height - 10) && Level.CheckRight(x + Width + 15, y + 10))
                Interlocked.Add(ref x, 4);
            if (Level.CheckRight(x + Width + 15, y + Height - 10) && Level.CheckRight(x + Width + 15, y + 10))
                Interlocked.Add(ref x, 4);
            //if (Level.CheckDown(x, y))
            //    Fall();
        });

        public Task MoveLeft() => new Task(() =>
        {
            Interlocked.Exchange(ref side, -1);
            if (Level.CheckLeft(x - 15, y + 10) && Level.CheckLeft(x - 15, y + Height - 10))
                Interlocked.Add(ref x, -4);
            if (Level.CheckLeft(x - 15, y + 10) && Level.CheckLeft(x - 15, y + Height - 10))
                Interlocked.Add(ref x, -4);
            //if (Level.CheckDown(x, y))
            //    Fall();
        });
        
        public Task Jump() => new Task(() =>
        {
            if (IsFalling) return;
            if (IsJumping) return;
            
            IsJumping = true;
            
            var force = 25;
            while (force > 0)
            {
                Interlocked.Add(ref y, -force);
                force -= 1;
                Thread.Sleep(10);
            }

            IsJumping = false;
        });

        public Task Fall() => new Task(() =>
        {
            if (IsFalling) return;
            
            var force = 13;
            while (Level.CheckDown(x + 10, y + Height) && Level.CheckDown(x + Width - 10, y + Height))
            {
                IsFalling = true;
                Interlocked.Add(ref y, 10);
                if (force > 1)
                    force--;
                Thread.Sleep(force);
            }

            IsFalling = false;
        });

        public void GetDamage() => Task.Run(() =>
        {
            Interlocked.Decrement(ref health);
            if (health <= 0) 
                onDeathAction();
        });

        public void Damage() => Weapon.Use(Side);

        public void PickupBullet() => Weapon.PickupBullet();

        public void PickupHealth()
        {
            if (health <= 9)
                Interlocked.Increment(ref health);
        }

        private Task Move(Action action) => Task.Run(action);

        
        
        
    }
}