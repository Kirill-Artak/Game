using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Media;
using Timer = System.Threading.Timer;

namespace Game
{
    public class Engine
    {
        public Player Player { get; }
        public ILevel Level { get; }
        public System.Windows.Forms.Timer InvalidationTimer { get; }
        public MediaPlayer MediaPlayer { get; }

        public KeyEventHandler KeyPressed { get; }
        public PaintEventHandler Paint { get; }

        private bool isPaused = false;
        
        public Engine(System.Windows.Forms.Timer timer, MediaPlayer mediaPlayer, Action gameStopped)
        {
            KeyPressed = new KeyEventHandler(ControlMoving);

            InvalidationTimer = timer;
            Level = new TestLevel();
            Player = new Player(Level);
            MediaPlayer = mediaPlayer;
            
            
            Paint = (o, args) =>
            {
                var g = args.Graphics;
                g.SmoothingMode = SmoothingMode.HighSpeed;

                g.DrawImage(Image.FromFile(@"assets\playerSource.bmp"), Player.X + 100, Player.Y + 500);
            };

        }

        public void StartGame()
        {
            MediaPlayer.Open(new Uri(@"assets\game.mp3", UriKind.Relative));
            MediaPlayer.Play();
            InvalidationTimer.Start();// добавить в таймер обработчик нажатий(на другой таймер)
        }

        private void ControlMoving(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    isPaused = !isPaused;
                    if (!isPaused)
                    {
                        MediaPlayer.Pause();
                        InvalidationTimer.Stop();
                    }
                    else
                    {
                        MediaPlayer.Play();
                        InvalidationTimer.Start();
                    }
                    break;
                case Keys.D:
                    Player.MoveRight();
                    break;
                case Keys.A:
                    Player.MoveLeft();
                    break;
                case Keys.W:
                case Keys.Space:
                    Player.Jump();
                    break;
            }
        }
        
        
        
    }
}