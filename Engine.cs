using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Media;

namespace Game
{
    public class Engine
    {
        MediaPlayer mediaPlayer = new MediaPlayer();
        private int x = 0;
        public Player Player { get; }
        public ILevel Level { get; }

        public KeyEventHandler KeyPressed { get; }
        public PaintEventHandler Paint { get; }

        private readonly Action Invalidate;
        
        public Engine(Action invalidate, Action gameStopped)
        {
            KeyPressed = new KeyEventHandler(ControlMoving);
            Invalidate = invalidate;
            
            Player = new Player(Level);
            Level = new TestLevel();
            
            
            Paint = (o, args) =>
            {
                var g = args.Graphics;
                g.SmoothingMode = SmoothingMode.HighSpeed;

                g.DrawImage(Image.FromFile(@"..\..\assets\playerSource.bmp"), Player.X, Player.Y);
            };

        }

        private void ControlMoving(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    break;
                case Keys.Right:
                    Player.MoveRight();
                    break;
            }
        }
        
        
        
    }
}