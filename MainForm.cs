using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms.VisualStyles;
using System.Windows.Media;
using Color = System.Drawing.Color;

namespace Game
{
    public class MainForm : Form
    {
        private Size menuButtonSize = new Size(180, 60);
        private Font menuButtonFont = new Font("Arial", 14);
        private Image backgroundImage = Image.FromFile(@"assets\Background\a.png");
        
        private Timer timer = new Timer();
        private MediaPlayer mediaPlayer = new MediaPlayer();
        private Engine engine;

        private TableLayoutPanel MenuTable;
        private TableLayoutPanel SettingsTable;
        private TableLayoutPanel PauseTable;

        public MainForm()
        {
            BackgroundImage = backgroundImage;
            
            Size = new Size(1280, 720);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            KeyPreview = true;

            SetStyle(ControlStyles.OptimizedDoubleBuffer 
                     | ControlStyles.AllPaintingInWmPaint 
                     | ControlStyles.UserPaint, true);
            
            UpdateStyles();
            
            timer.Interval = 20;
            timer.Tick += (sender, args) => Invalidate();

            engine = new Engine(timer, mediaPlayer, () => { Controls.Add(PauseTable); }, ChangeBackground);
            
            
            MenuTable = MenuBuilder(
                ButtonBuilder("Новая игра", (s, e) => {
                    Controls.Remove(MenuTable);
                    mediaPlayer.Stop();
                    
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

            PauseTable = MenuBuilder(ButtonBuilder("Продолжить", (s, e) => {
                    Controls.Remove(PauseTable);
                    engine.ContinueGame();
                    Focus();
                }),
                ButtonBuilder("Выход", (s, e) => Application.Exit()));

            Controls.Add(MenuTable);
            
            mediaPlayer.Open(new Uri(@"assets\Audio\menu.mp3", UriKind.Relative));
            mediaPlayer.MediaEnded += (sender, args) =>
            {
                mediaPlayer.Stop();
                mediaPlayer.Play();
            };
            mediaPlayer.Volume = 0.1;
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

        public void ChangeBackground(string source)
        {
            BackgroundImage = Image.FromFile(source);
        }

        private void RunGame()
        {
            BackgroundImage = null;
            backgroundImage = null;
            MenuTable = null;
            
            

            KeyDown += (o, e) => engine.KeyPressed.Invoke(o, e);
            KeyUp += (sender, args) => engine.KeyUnpressed.Invoke(sender, args);
            Paint += (sender, args) => engine.Paint.Invoke(sender, args);
            
            engine.StartGame();
            Focus();
        }

        private TableLayoutPanel MenuBuilder(params Control[] buttons)
        {
            var table = new TableLayoutPanel();
            
            //table.BackgroundImage = backgroundImage;
            table.BackColor = Color.Transparent;

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