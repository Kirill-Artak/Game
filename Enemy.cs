using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace Game
{
    public class Enemy
    {
        public Image ImageRight { get; }
        public Image ImageLeft { get; }
        
        public int Width { get; }
        public int Height { get; }

        public int X => x;
        public int Y => y;
        
        public int Line => (y + Height / 2) / 72;

        public Side Side => (Side) side;

        public bool IsTriggered { get; set; }
        public bool IsWalk { get; set; }

        private int side = 1;

        private int x;
        private int y;

        public ILevel Level { get; }

        public Enemy(ILevel level, int x, int y)
        {
            ImageRight = Image.FromFile(@"assets\enemy.bmp");
            ImageLeft = (Image) ImageRight.Clone();
            
            ImageLeft.RotateFlip(RotateFlipType.RotateNoneFlipX);
            
            Level = level;

            this.x = x;
            this.y = y;
            
            Height = 72;
            Width = 38;
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
    }
}