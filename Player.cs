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
        public Image ImageRight { get; }
        public Image ImageLeft { get; }
        
        public int Health => health;
        public int X => x;
        public int Y => y;
        public Side Side => (Side) side;
        public int Line => (y + Height / 2) / 72;
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
            ImageRight = System.Drawing.Image.FromFile(@"assets\playerSource.bmp");
            ImageLeft = (Image) ImageRight.Clone();
            
            ImageLeft.RotateFlip(RotateFlipType.RotateNoneFlipX);

            Height = 72;
            Width = 38;
            
            Level = level;
            this.onDeathAction = () => MessageBox.Show("1");
        }

        public void SetCoordinate(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Task MoveRight() => new Task(() =>
        {
            Interlocked.Exchange(ref side, 1);
            if (Level.Check(x + Width + 15, y + Height - 10, true) 
                && Level.Check(x + Width + 15, y + 10, true))
                Interlocked.Add(ref x, 4);
            if (Level.Check(x + Width + 15, y + Height - 10, true) 
                && Level.Check(x + Width + 15, y + 10, true))
                Interlocked.Add(ref x, 4);
            //if (Level.CheckDown(x, y))
            //    Fall();
        });

        public Task MoveLeft() => new Task(() =>
        {
            Interlocked.Exchange(ref side, -1);
            if (Level.Check(x - 15, y + 10, true) 
                && Level.Check(x - 15, y + Height - 10, true))
                Interlocked.Add(ref x, -4);
            if (Level.Check(x - 15, y + 10, true) 
                && Level.Check(x - 15, y + Height - 10, true))
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
            while (Level.Check(x + 10, y + Height, true) 
                   && Level.Check(x + Width - 10, y + Height, true))
            {
                IsFalling = true;
                Interlocked.Add(ref y, 10);
                if (force > 1)
                    force--;
                Thread.Sleep(force);
            }

            IsFalling = false;
        });

        public void GetDamage(Side direction) => Task.Run(() =>
        {
            var kb = KnockBack(direction);
            kb.Start();
            kb.ContinueWith(t => Fall().Start());
            
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
        
        private Task KnockBack(Side direction) => new Task(() =>
        {
            var power = 20 * (int) direction;
            var distance = x + 18 + 20 *(int) direction;
            
            if (Level.Check(x - 15, y + 10, true) 
                && Level.Check(distance, y + Height - 10, true))
                Interlocked.Add(ref x, power);
        });
    }
}