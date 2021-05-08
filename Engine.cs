using System;
using System.Collections.Concurrent;
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
        public System.Windows.Forms.Timer ActionTimer { get; }
        public MediaPlayer MediaPlayer { get; }

        public KeyEventHandler KeyPressed { get; }
        public KeyEventHandler KeyUnpressed { get; }
        public PaintEventHandler Paint { get; }
        
        //private readonly ConcurrentQueue<Movement> actionsOnKeys = new ConcurrentQueue<Movement>();
        
        private readonly MovingController movingController;

        private bool isPaused = false;
        
        public Engine(System.Windows.Forms.Timer timer, MediaPlayer mediaPlayer, Action gameStopped)
        {
            KeyPressed = OnPressKey;
            KeyUnpressed = OnUpKey;

            InvalidationTimer = timer;
            ActionTimer = new System.Windows.Forms.Timer();
            ActionTimer.Interval = 10;
            Level = new TestLevel();
            Player = new Player(Level);
            MediaPlayer = mediaPlayer;
            movingController = new MovingController(Player);
            
            Paint = (o, args) =>
            {
                var g = args.Graphics;
                g.SmoothingMode = SmoothingMode.HighSpeed;

                g.DrawImage(Image.FromFile(@"assets\playerSource.bmp"), Player.X, Player.Y);
            };

        }

        public void StartGame()
        {
            ActionTimer.Tick += ActionOnTick;
            
            Player.SetCoordinate(100, 500);
            
            MediaPlayer.Open(new Uri(@"assets\game.mp3", UriKind.Relative));
            MediaPlayer.Play();
            
            InvalidationTimer.Start();
            ActionTimer.Start();
        }

        private void ActionOnTick(object s, EventArgs eventArgs)
        {
            movingController.Move();
        }

        private void OnPressKey(object sender, KeyEventArgs e)
        {
            movingController.AddMovement(e.KeyCode);
        }

        private void OnUpKey(object s, KeyEventArgs e)
        {
            movingController.RemoveMovement(e.KeyCode);
        }
        
    }
}