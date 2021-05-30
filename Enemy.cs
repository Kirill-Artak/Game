using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace Game
{
    public class Enemy
    {
        public Image ImageRight { get; }
        public Image ImageLeft { get; }

        public Image ImageRightFocused { get; }
        public Image ImageLeftFocused { get; }

        public int Width { get; }
        public int Height { get; }

        public int X => x;
        public int Y => y;

        public int Health => Health;
        
        public int Line => (y + Height / 2) / 72;

        public Side Side => (Side) side;

        public bool IsTriggered { get; set; }
        public bool IsWalk { get; set; }
        public bool IsFocused { get; set; }

        public bool IsDead { get; set; }

        private int side = 1;

        public int x;
        public int y;
        public int health;

        public Level Level { get; }

        public Enemy(Level level, int x, int y)
        {
            ImageRight = Image.FromFile(@"assets\Enemies\Enemy.png");
            ImageLeft = (Image) ImageRight.Clone();
            
            ImageLeft.RotateFlip(RotateFlipType.RotateNoneFlipX);
            
            ImageRightFocused = Image.FromFile(@"assets\Enemies\enemyFocused.png");
            ImageLeftFocused = (Image) ImageRightFocused.Clone();
            
            ImageLeftFocused.RotateFlip(RotateFlipType.RotateNoneFlipX);
            
            Level = level;

            this.x = x;
            this.y = y;
            
            Height = 54; //72
            Width = 28;  //38
            health = 5;
        }
        
        public Task MoveRight() => new Task(() =>
        {
            IsWalk = true;
            
            Interlocked.Exchange(ref side, 1);
            
            if (Level.Check(x + Width + 15, y + Height - 10, false) 
                && Level.Check(x + Width + 15, y + 10, false))
                Interlocked.Add(ref x, 2);
            if (Level.Check(x + Width + 15, y + Height - 10, false) 
                && Level.Check(x + Width + 15, y + 10, false))
                Interlocked.Add(ref x, 2);

            IsWalk = false;
        });
        
        public Task MoveLeft() => new Task(() =>
        {
            IsWalk = true;
            
            Interlocked.Exchange(ref side, -1);
            
            if (Level.Check(x - 15, y + 10, false) 
                && Level.Check(x - 15, y + Height - 10, false))
                Interlocked.Add(ref x, -2);
            if (Level.Check(x - 15, y + 10, false) 
                && Level.Check(x - 15, y + Height - 10, false))
                Interlocked.Add(ref x, -2);

            IsWalk = false;
        });
        
        public Task Fall() => new Task(() =>
        {
            //if (IsFalling) return;
            
            var force = 13;
            while (Level.Check(x + 10, y + Height, true) 
                   && Level.Check(x + Width - 10, y + Height, true))
            {
                //IsFalling = true;
                Interlocked.Add(ref y, 10);
                if (force > 1)
                    force--;
                Thread.Sleep(force);
            }

            //IsFalling = false;
        });
        
        public Task GetDamage() => new Task(() =>
        {
            Interlocked.Decrement(ref health);
            IsDead = health <= 0;
        });

        public int Distance(int x) => Math.Abs(this.x - x);
    }
}