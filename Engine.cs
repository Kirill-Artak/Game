using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Media;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Windows.Media;
using Brushes = System.Drawing.Brushes;
using Color = System.Drawing.Color;
using Pen = System.Windows.Media.Pen;
using Timer = System.Windows.Forms.Timer;

namespace Game
{
    public class Engine
    {
        public int TotalBonus { get; private set; }
        
        public Player Player { get; }
        public Level Level { get; private set; }

        public LevelBuilder LevelBuilder { get; }

        //
        public EnemiesController EnemiesController { get; private set; }
        public ItemsController ItemsController { get; private set; }
        //
        
        public System.Windows.Forms.Timer InvalidationTimer { get; private set; }
        public System.Windows.Forms.Timer ActionTimer { get; private set; }
        public Timer PlayerDeathTimer { get; private set; }
        public Timer EndOfLevelTimer { get; private set; }
        
        //
        public System.Windows.Forms.Timer EnemiesActionTimer { get; private set; }
        //
        
        //
        public System.Windows.Forms.Timer ItemTimer { get; private set; }
        //
        
        public MediaPlayer MediaPlayer { get; }
        
        public SoundPlayer BreakGlassPlayer { get; }
        public Action<string> ChangeBackground { get; }

        public KeyEventHandler KeyPressed { get; }
        public KeyEventHandler KeyUnpressed { get; }
        public PaintEventHandler Paint { get; }

        //private readonly ConcurrentQueue<Movement> actionsOnKeys = new ConcurrentQueue<Movement>();
        
        private readonly MovingController movingController;

        private bool isPaused = false;
        private bool isGameEnded = false;
        private bool isBonus = true;

        private int currentLevel = 0;

        private Action onPause;
        
        private Image bottle = Textures.BonusTexturesDictionary[ItemType.Health];
        private Image bonus = Textures.BonusTexturesDictionary[ItemType.Bonus];


        private System.Drawing.Pen hpPen = new System.Drawing.Pen(Color.LightCoral, 5);
        private Image EndBackground;
        private Font endFont = new Font("Arial", 18);

        public Engine(System.Windows.Forms.Timer timer, MediaPlayer mediaPlayer, 
            Action gameStopped, Action<string> changeBackground)
        {
            //Levels = LevelBuilder.BuildFromFiles(@"Level1.txt", @"Level2.txt");
            LevelBuilder = new LevelBuilder(@"Level1.txt", @"Level2.txt");
            
            KeyPressed = OnPressKey;
            KeyUnpressed = OnUpKey;

            onPause = gameStopped;

            ChangeBackground = changeBackground;

            InvalidationTimer = timer;
            
            PlayerDeathTimer = new Timer();
            PlayerDeathTimer.Interval = 60;
            PlayerDeathTimer.Tick += (sender, args) =>
            {
                if (Player.IsDead)
                {
                    isBonus = false;
                    PlayerDeathTimer.Stop();
                    Restart();
                }
            };
            
            EndOfLevelTimer = new Timer();
            EndOfLevelTimer.Interval = 60;
            EndOfLevelTimer.Tick += (sender, args) =>
            {
                if (Math.Abs(Player.X - Level.EndGameX) < 100 
                    && Math.Abs(Player.Y - Level.EndGameY) < 100)
                {
                    EndOfLevelTimer.Stop();
                    NextLevel();
                }
            };
            
            BreakGlassPlayer = new SoundPlayer();
            BreakGlassPlayer.SoundLocation = @"assets\Audio\glass.wav";

            Player = new Player(Restart, () => {BreakGlassPlayer.Play();});

            MediaPlayer = mediaPlayer;
            
            movingController = new MovingController(Player);

            Paint += (sender, args) =>
            {
                var g = args.Graphics;
                g.SmoothingMode = SmoothingMode.HighSpeed;

                if (isGameEnded)
                {
                    g.DrawImage(EndBackground, 0, 0);
                    g.DrawString(TotalBonus.ToString(),endFont,Brushes.PaleVioletRed, 932, 282);
                    return;
                }
                
                
                if (Level.IsOutdoor)
                {
                    g.TranslateTransform(-Player.X * 0.25f, 0);

                    g.DrawImage(Level.Background, 0, 0);
                    //g.DrawImage(Image.FromFile(@"assets\unknown.bmp"), );
                    g.TranslateTransform(Player.X * 0.25f, 0);
                }


                if (Level.HasWall)
                {
                    g.TranslateTransform(Level.IsBackgroundMoving ? -Player.X * 0.5f : -Player.X + 300, 0);

                    g.DrawImage(Level.Wall, 0, 300);

                    g.TranslateTransform(Level.IsBackgroundMoving ? Player.X * 0.5f : Player.X - 300, 0);
                }
                

                g.TranslateTransform(-Player.X + 300, 0);

                for (int i = 0; i < Level.LevelMash.GetLength(0); i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        if (Level.LevelMash[i, j].Type == Cells.Ground
                            || Level.LevelMash[i, j].Type == Cells.Ground2
                            || Level.LevelMash[i, j].Type == Cells.Conditioner
                            || Level.LevelMash[i, j].Type == Cells.Platform
                            || Level.LevelMash[i, j].Type == Cells.Box
                            || Level.LevelMash[i, j].Type == Cells.Final
                            || Level.LevelMash[i, j].Type == Cells.Brick
                            || Level.LevelMash[i, j].Type == Cells.BBrick)
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
                
                g.DrawImage(Level.Start, -100, 278);

                
                g.DrawImage(Level.Border, -300, 0);
                g.DrawImage(Level.Border, 9200, 0);

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
        }

        
        public void StartGame()
        {
            Level = LevelBuilder.Build(currentLevel);
            Player.SetLevel(Level);
            
            EnemiesController = new EnemiesController(
                Level.Enemies, 
                Player, 
                Level);
            ItemsController = new ItemsController(Level.Items, Player);
            
            ActionTimer = new System.Windows.Forms.Timer();
            ActionTimer.Interval = 10;
            
            EnemiesActionTimer = new System.Windows.Forms.Timer();
            EnemiesActionTimer.Interval = 10;
            
            ItemTimer = new System.Windows.Forms.Timer();
            ItemTimer.Interval = 20;
            
            ActionTimer.Tick += ActionOnTick;
            EnemiesActionTimer.Tick += EnemiesController.OnTick;
            ItemTimer.Tick += ItemsController.OnTick;
            
            Player.HealthUp();
            Player.SetCoordinate(300, 400);
//////////////////////////////////////////////////////////////////
            //ChangeBackground(@"assets\a.png");
            
            MediaPlayer.Stop();
            MediaPlayer.Open(new Uri(Level.Audio, UriKind.Relative));
            MediaPlayer.Play();
            
            InvalidationTimer.Start();
            ActionTimer.Start();
            EnemiesActionTimer.Start();
            ItemTimer.Start();
            PlayerDeathTimer.Start();
            EndOfLevelTimer.Start();
            
            Player.Fall().Start();
            

            foreach (var e in Level.Enemies)
            {
                e.Fall().Start();
            }
        }

        private void NextLevel()
        {
            //InvalidationTimer.Stop();
            ActionTimer.Stop();
            EnemiesActionTimer.Stop();
            ItemTimer.Stop();
            
            //st
            
            MediaPlayer.Stop();

            TotalBonus += Player.Bonus;

            currentLevel++;

            if (currentLevel + 1 > LevelBuilder.LevelCount)
            {
                if (isBonus) TotalBonus++;
                EndBackground = Image.FromFile(@"assets\Background\end.jpg");
                isGameEnded = true;
                MediaPlayer.Open(new Uri(@"assets\Audio\fin.mp3", UriKind.Relative));
                MediaPlayer.Play();
                return;
            }
            
            StartGame();
        }

        private void Restart()
        {
            EnemiesController.Pause();
            movingController.Abort();
            
            InvalidationTimer.Stop();
            ActionTimer.Stop();
            EnemiesActionTimer.Stop();
            
            //MediaPlayer.Pause();
            
            Level.Rebuild();
            
            StartGame();
        }

        private void Pause()
        {
            EnemiesController.Pause();
            
            InvalidationTimer.Stop();
            ActionTimer.Stop();
            EnemiesActionTimer.Stop();
            
            MediaPlayer.Pause();
            
            movingController.Abort();

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