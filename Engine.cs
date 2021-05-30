using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Media;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Windows.Media;
using Color = System.Drawing.Color;
using Pen = System.Windows.Media.Pen;
using Timer = System.Threading.Timer;

namespace Game
{
    public class Engine
    {
        public Player Player { get; }
        public Level Level { get; }
        public EnemiesController EnemiesController { get; private set; }
        public ItemsController ItemsController { get; private set; }
        public System.Windows.Forms.Timer InvalidationTimer { get; }
        public System.Windows.Forms.Timer ActionTimer { get; }
        public System.Windows.Forms.Timer EnemiesActionTimer { get; }
        public System.Windows.Forms.Timer ItemTimer { get; }
        public MediaPlayer MediaPlayer { get; }
        
        public SoundPlayer BreakGlassPlayer { get; }
        public Action<string> ChangeBackground { get; }

        public KeyEventHandler KeyPressed { get; }
        public KeyEventHandler KeyUnpressed { get; }
        public PaintEventHandler Paint { get; }

        //private readonly ConcurrentQueue<Movement> actionsOnKeys = new ConcurrentQueue<Movement>();
        
        private readonly MovingController movingController;

        private bool isPaused = false;

        private Action onPause;
        
        private Image bottle = Textures.BonusTexturesDictionary[ItemType.Health];
        private Image bonus = Textures.BonusTexturesDictionary[ItemType.Bonus];


        private System.Drawing.Pen hpPen = new System.Drawing.Pen(Color.LightCoral, 5);
        
        
        public Engine(System.Windows.Forms.Timer timer, MediaPlayer mediaPlayer, 
            Action gameStopped, Action<string> changeBackground)
        {
            var l = File.ReadAllText(@"C:\UrFU\game\Game\Level1.txt");
            
            KeyPressed = OnPressKey;
            KeyUnpressed = OnUpKey;

            onPause = gameStopped;

            ChangeBackground = changeBackground;

            InvalidationTimer = timer;
            ActionTimer = new System.Windows.Forms.Timer();
            ActionTimer.Interval = 5;
            EnemiesActionTimer = new System.Windows.Forms.Timer();
            EnemiesActionTimer.Interval = 10;
            ItemTimer = new System.Windows.Forms.Timer();
            ItemTimer.Interval = 20;
            
            BreakGlassPlayer = new SoundPlayer();
            BreakGlassPlayer.SoundLocation = @"assets\Audio\glass.wav";
            
            //var levelBuilder = new LevelBuilder();
            //Level = new LevelBuilder().BuildFromString(LevelBuilder.TestLevel);
            Level = new LevelBuilder().BuildFromString(l);
            Level.SetBackground(@"assets\Background\background1.jpg");
            Level.SetWall(@"assets\Background\wall.png");
            
            Player = new Player(Level, () => { }, () => {BreakGlassPlayer.Play();});
            
            EnemiesController = new EnemiesController(Level.Enemies, Player, Level);
            ItemsController = new ItemsController(Level.Items, Player);
            
            MediaPlayer = mediaPlayer;
            
            movingController = new MovingController(Player);

            Paint += (sender, args) =>
            {
                var g = args.Graphics;
                g.SmoothingMode = SmoothingMode.HighSpeed;
                
                g.TranslateTransform(-Player.X * 0.25f, 0);
                
                g.DrawImage(Level.Background, 0, 0);
                //g.DrawImage(Image.FromFile(@"assets\unknown.bmp"), );
                
                g.TranslateTransform(-Player.X * 0.25f, 0);
                
                g.DrawImage(Level.Wall, 0, 300);

                g.TranslateTransform(Player.X * 0.5f, 0);
                
                
                g.TranslateTransform(-Player.X + 300, 0);

                for (int i = 0; i < 128; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        if (Level.LevelMash[i, j].Type == Cells.Ground
                            || Level.LevelMash[i, j].Type == Cells.Ground2)
                            g.DrawImage(Level.LevelMash[i, j].Texture, 72 * i, 72 * j);
                    }
                }
                
                //g.TranslateTransform(Player.X + 300, 0);
                

                //g.TranslateTransform(-Player.X + 300, 0);
                
                foreach (var e in Level.Enemies)
                {
                    if (!e.IsDead)
                    {
                        if (e.IsFocused)
                        {
                            g.DrawImage(e.Side == Side.Left
                                ? e.ImageLeftFocused
                                : e.ImageRightFocused, e.X, e.Y);
                        }
                        else
                        {
                            g.DrawImage(e.Side == Side.Left 
                                ? e.ImageLeft 
                                : e.ImageRight, e.X, e.Y);
                        }
                        
                        g.DrawLine(hpPen, 
                            e.x, e.y - 7, e.x + e.Width / 5 * e.health, e.y - 7);
                    }
                }

                foreach (var e in Level.Items)
                {
                    if (!e.IsUsed)
                    {
                        g.DrawImage(Textures.BonusTexturesDictionary[e.ItemType], e.X, e.Y);
                    }
                }
                
                //g.TranslateTransform(Player.X + 300, 0);
                

                g.DrawImage(Player.Side == Side.Left ? Player.ImageLeft : Player.ImageRight, Player.X, Player.Y);
                
                g.TranslateTransform(Player.X - 300, 0);

                for (var i = 0; i < Player.Health; i++)
                {
                    g.DrawImage(bottle, 20 + 40 * i, 20);
                }

                for (var i = 0; i < Player.Bonus; i++)
                {
                    g.DrawImage(bonus, 700 - 40 * i, 20);
                }
            }; 
            
            
            /*
            Paint += (sender, args) =>
            {
                var g = args.Graphics;
                g.SmoothingMode = SmoothingMode.HighSpeed;
                
                g.TranslateTransform(-Player.X * 0.25f, 0);
                
                g.DrawImage(Level.Background, 0, 0);
                //g.DrawImage(Image.FromFile(@"assets\unknown.bmp"), );
                
                g.TranslateTransform(Player.X * 0.25f, 0);
            };
            
            Paint += (sender, args) =>
            {
                var g = args.Graphics;
                g.SmoothingMode = SmoothingMode.HighSpeed;
                
                g.TranslateTransform(-Player.X * 0.5f, 0);
                
                g.DrawImage(Level.Wall, 0, 300);

                g.TranslateTransform(Player.X * 0.5f, 0);
            };
            
            Paint += (sender, args) =>
            {
                var g = args.Graphics;
                g.SmoothingMode = SmoothingMode.HighSpeed;
                
                g.TranslateTransform(-Player.X + 300, 0);

                for (int i = 0; i < 32; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        if (Level.LevelMash[i, j].Type != Cells.Space 
                            && Level.LevelMash[i, j].Type != Cells.Enemy)
                            g.DrawImage(Level.LevelMash[i, j].Texture, 72 * i, 72 * j);
                    }
                }
                
                //g.TranslateTransform(Player.X + 300, 0);
            };

            Paint += (sender, args) =>
            {
                var g = args.Graphics;
                g.SmoothingMode = SmoothingMode.HighSpeed;

                //g.TranslateTransform(-Player.X + 300, 0);
                
                foreach (var e in Level.Enemies)
                {
                    g.DrawImage(e.Side == Side.Left ? e.ImageLeft : e.ImageRight, e.X, e.Y);
                }
                
                //g.TranslateTransform(Player.X + 300, 0);
            };

            Paint += (o, args) =>
            {
                var g = args.Graphics;
                g.SmoothingMode = SmoothingMode.HighSpeed;

                g.DrawImage(Player.Side == Side.Left ? Player.ImageLeft : Player.ImageRight, Player.X, Player.Y);
            };
            */

        }

        
        public void StartGame()
        {
            ActionTimer.Tick += ActionOnTick;
            EnemiesActionTimer.Tick += EnemiesController.OnTick;
            ItemTimer.Tick += ItemsController.OnTick;
            
            Player.SetCoordinate(300, 400);

            //ChangeBackground(@"assets\a.png");
            
            MediaPlayer.Open(new Uri(@"assets\Audio\game.mp3", UriKind.Relative));
            MediaPlayer.Play();
            
            InvalidationTimer.Start();
            ActionTimer.Start();
            EnemiesActionTimer.Start();
            ItemTimer.Start();
            
            Player.Fall().Start();

            foreach (var e in Level.Enemies)
            {
                e.Fall().Start();
            }
        }

        public void Pause()
        {
            InvalidationTimer.Stop();
            ActionTimer.Stop();
            EnemiesActionTimer.Stop();
            
            MediaPlayer.Pause();
            
            movingController.Abort();
            EnemiesController.Pause();

            onPause();
        }

        public void ContinueGame()
        {
            EnemiesController.Continue();
            
            ActionTimer.Start();
            InvalidationTimer.Start();
            EnemiesActionTimer.Start();
            
            MediaPlayer.Play();
        }

        private void ActionOnTick(object s, EventArgs eventArgs)
        {
            movingController.Move();
        }

        private void OnPressKey(object sender, KeyEventArgs e)
        {
            if (!isPaused && e.KeyCode != Keys.Escape)
            {
                movingController.AddMovement(e.KeyCode);
                return;
            }
            
            Pause();
        }

        private void OnUpKey(object s, KeyEventArgs e)
        {
            movingController.RemoveMovement(e.KeyCode);
        }
        
    }
}