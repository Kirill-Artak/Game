using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms.VisualStyles;
using System.Windows.Media;

namespace Game
{
    public class MainForm : Form
    {
        private Size menuButtonSize = new Size(180, 60);
        private Font menuButtonFont = new Font("Arial", 14);
        private Image backgroundImage = Image.FromFile(@"..\..\assets\a.png");
        
        private MediaPlayer mediaPlayer = new MediaPlayer();
        private Timer timer = new Timer();

        private TableLayoutPanel MenuTable;
        private TableLayoutPanel SettingsTable;
        private TableLayoutPanel PauseTable;
        
        public MainForm()
        {
            Size = new Size(1280, 720);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            KeyPreview = true;

            SetStyle(ControlStyles.OptimizedDoubleBuffer 
                     | ControlStyles.AllPaintingInWmPaint 
                     | ControlStyles.UserPaint, true);
            
            UpdateStyles();

            timer.Interval = 5;
            timer.Tick += (sender, args) => Invalidate();

            MenuTable = MenuBuilder(
                ButtonBuilder("Новая игра", (s, e) => {
                    Controls.Remove(MenuTable);
                    //Paint += RunGame;
                    
                    //mediaPlayer.Stop();
                    //mediaPlayer.Open(new Uri(@"..\..\assets\game.mp3", UriKind.Relative));
                    //mediaPlayer.Play();
                    
                    RunGame();
                    
                }),
                ButtonBuilder("Настройки", (s, e) =>
                {
                    Controls.Remove(MenuTable);
                    Controls.Add(SettingsTable);
                }),
                ButtonBuilder("Выход", (s, e) => Application.Exit())
            );

            SettingsTable = MenuBuilder(
                CheckBoxBuilder(
                    "Полный экран", 
                    (s, e) =>
                    {
                        TopMost = true;
                        FormBorderStyle = ((CheckBox) s).Checked
                            ? FormBorderStyle.None
                            : FormBorderStyle.FixedDialog;
                        WindowState = ((CheckBox) s).Checked
                            ? FormWindowState.Maximized
                            : FormWindowState.Normal;
                    }),
                TrackBarBuilder("Громкость", (s, e) =>
                {
                    mediaPlayer.Volume = ((TrackBar) s).Value / 100.0;
                }),
                ButtonBuilder("Назад", (s, e) =>
                {
                    Controls.Remove(SettingsTable);
                    Controls.Add(MenuTable);
                }));
            
            Controls.Add(MenuTable);
            
            mediaPlayer.Open(new Uri(@"..\..\assets\menu.mp3", UriKind.Relative));
            mediaPlayer.Volume = 0.5;
            mediaPlayer.Play();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams handleParam = base.CreateParams;
                handleParam.ExStyle |= 0x02000000;
                return handleParam;
            }
        }
        
        

        private void RunGame()
        {
            timer.Start();
            
            var engine = new Engine(Invalidate, () => { });

            KeyDown += engine.KeyPressed;
            Paint += engine.Paint;
        }

        private TableLayoutPanel MenuBuilder(params Control[] buttons)
        {
            var table = new TableLayoutPanel();
            
            table.BackgroundImage = backgroundImage;
            
            table.Anchor = AnchorStyles.Top;
            table.Dock = DockStyle.Fill;
            
            table.RowStyles.Clear();
            
            for (int i = 0; i < 4; i++)
                table.RowStyles.Add(new RowStyle(SizeType.Percent, 16));
            
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 85));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));

            var placeHolder = new Panel() { BackColor = System.Drawing.Color.Transparent };
            
            table.Controls.Add(placeHolder, 1, 0);
            
            for (int i = 0; i < buttons.Length; i++)
                table.Controls.Add(buttons[i], 0, i + 1);

            return table;
        }
        
        private Button ButtonBuilder(string name, EventHandler action)
        {
            var button = new Button()
            {
                Text = name,
                Size = menuButtonSize,
                Font = menuButtonFont,
                ForeColor = System.Drawing.Color.DarkGoldenrod,
                BackColor = System.Drawing.Color.Transparent,
                Anchor = AnchorStyles.Right,
                FlatStyle = FlatStyle.Flat,
            };
            
            button.GotFocus += (sender, args) => button.BackColor = System.Drawing.Color.DarkSlateBlue;
            button.MouseEnter += (sender, args) => button.BackColor = System.Drawing.Color.DarkSlateBlue;

            button.LostFocus += (sender, args) => button.BackColor = System.Drawing.Color.Transparent;
            button.MouseLeave += (sender, args) => button.BackColor = System.Drawing.Color.Transparent;

            button.Click += action;

            return button;
        }

        private TrackBar TrackBarBuilder(string name, EventHandler scroll)
        {
            var trackBar = new TrackBar()
            {
                Text = name,
                Size = menuButtonSize,
                Font = menuButtonFont,
                Anchor = AnchorStyles.Right,
                //BackColor = Color.Black,
                //ForeColor = Color.DarkGoldenrod,
                Minimum = 0,
                Maximum = 100,
                TickFrequency = 25,
                Value = 50,
            };
            
            trackBar.Scroll += scroll;

            return trackBar;
        }

        private CheckBox CheckBoxBuilder(string name, EventHandler checkedChanged)
        {
            var checkBox = new CheckBox()
            {
                Text = name,
                Size = menuButtonSize,
                Font = menuButtonFont,
                ForeColor = System.Drawing.Color.DarkGoldenrod,
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Right,
                BackColor = System.Drawing.Color.Transparent,
                Checked = false,
            };
            
            checkBox.GotFocus += (sender, args) => checkBox.BackColor = System.Drawing.Color.DarkSlateBlue;
            checkBox.MouseEnter += (sender, args) => checkBox.BackColor = System.Drawing.Color.DarkSlateBlue;

            checkBox.LostFocus += (sender, args) => checkBox.BackColor = System.Drawing.Color.Transparent;
            checkBox.MouseLeave += (sender, args) => checkBox.BackColor = System.Drawing.Color.Transparent;

            checkBox.CheckedChanged += checkedChanged;

            return checkBox;
        }
    }
}