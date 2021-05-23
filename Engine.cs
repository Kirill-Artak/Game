﻿using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using Timer = System.Threading.Timer;

namespace Game
{
    public class Engine
    {
        public Player Player { get; }
        public Level Level { get; }
        public EnemiesController EnemiesController { get; }
        public System.Windows.Forms.Timer InvalidationTimer { get; }
        public System.Windows.Forms.Timer ActionTimer { get; }
        public System.Windows.Forms.Timer EnemiesActionTimer { get; }
        public MediaPlayer MediaPlayer { get; }
        public Action<string> ChangeBackground { get; }

        public KeyEventHandler KeyPressed { get; }
        public KeyEventHandler KeyUnpressed { get; }
        public PaintEventHandler Paint { get; }

        //private readonly ConcurrentQueue<Movement> actionsOnKeys = new ConcurrentQueue<Movement>();
        
        private readonly MovingController movingController;

        private bool isPaused = false;
        
        
        private Image RightPlayer = Image.FromFile(@"assets\playerSource.bmp");
        private Image LeftPlayer = Image.FromFile(@"assets\playerSource.bmp");
        
        public Engine(System.Windows.Forms.Timer timer, MediaPlayer mediaPlayer, Action gameStopped, Action<string> changeBackground)
        {
            KeyPressed = OnPressKey;
            KeyUnpressed = OnUpKey;

            ChangeBackground = changeBackground;

            InvalidationTimer = timer;
            ActionTimer = new System.Windows.Forms.Timer();
            ActionTimer.Interval = 5;
            EnemiesActionTimer = new System.Windows.Forms.Timer();
            EnemiesActionTimer.Interval = 10;
            
            //var levelBuilder = new LevelBuilder();
            Level = new LevelBuilder().BuildFromString(LevelBuilder.TestLevel);
            Level.SetBackground(@"assets\background1.jpg");
            Level.SetWall(@"assets\wall.png");
            
            Player = new Player(Level);
            
            EnemiesController = new EnemiesController(Level.Enemies, Player, Level);
            
            MediaPlayer = mediaPlayer;
            movingController = new MovingController(Player);
            
            LeftPlayer.RotateFlip(RotateFlipType.RotateNoneFlipX);

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
                

                //g.TranslateTransform(-Player.X + 300, 0);
                
                foreach (var e in Level.Enemies)
                {
                    g.DrawImage(e.Side == Side.Left ? e.ImageLeft : e.ImageRight, e.X, e.Y);
                }
                
                //g.TranslateTransform(Player.X + 300, 0);
                

                g.DrawImage(Player.Side == Side.Left ? Player.ImageLeft : Player.ImageRight, Player.X, Player.Y);
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
            
            Player.SetCoordinate(300, 400);

            //ChangeBackground(@"assets\a.png");
            
            MediaPlayer.Open(new Uri(@"assets\game.mp3", UriKind.Relative));
            MediaPlayer.Play();
            
            InvalidationTimer.Start();
            ActionTimer.Start();
            EnemiesActionTimer.Start();
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