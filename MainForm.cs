using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Game
{
    public class MainForm : Form
    {
        private static Size menuButtonSize = new Size(180, 60);
        private static Font menuButtonFont = new Font("Arial", 16);
        private static Image backgroundImage = Image.FromFile(@"..\..\assets\a.png");

        private static TableLayoutPanel MenuTable;
        private static TableLayoutPanel SettingsTable;
        private static TableLayoutPanel PauseTable;
        
        public MainForm()
        {
            Size = new Size(1280, 720);
            FormBorderStyle = FormBorderStyle.FixedDialog;

            MenuTable = MenuBuilder(
                ButtonBuilder("Новая игра", (s, e) => {
                    Controls.Remove(MenuTable);
                    Paint += RunGame;
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
                        WindowState = ((CheckBox)s).Checked 
                            ? FormWindowState.Maximized 
                            : FormWindowState.Normal),
                TrackBarBuilder("Громкость", (s, e) => {}),
                ButtonBuilder("Назад", (s, e) =>
                {
                    Controls.Remove(SettingsTable);
                    Controls.Add(MenuTable);
                }));
            
            Controls.Add(MenuTable);
        }

        private void RunGame(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            
            g.DrawImage(Image.FromFile(@"..\..\assets\playerSource.bmp"), new Point(100, 500));
            BackgroundImage = backgroundImage;
            
            
        }

        private static TableLayoutPanel MenuBuilder(params Control[] buttons)
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

            var placeHolder = new Panel() { BackColor = Color.Transparent };
            
            table.Controls.Add(placeHolder, 1, 0);
            
            for (int i = 0; i < buttons.Length; i++)
                table.Controls.Add(buttons[i], 0, i + 1);

            return table;
        }
        
        private static Button ButtonBuilder(string name, EventHandler action)
        {
            var button = new Button()
            {
                Text = name,
                Size = menuButtonSize,
                Font = menuButtonFont,
                ForeColor = Color.DarkGoldenrod,
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Right,
                FlatStyle = FlatStyle.Flat,
            };
            
            button.GotFocus += (sender, args) => button.BackColor = Color.DarkSlateBlue;
            button.MouseEnter += (sender, args) => button.BackColor = Color.DarkSlateBlue;

            button.LostFocus += (sender, args) => button.BackColor = Color.Transparent;
            button.MouseLeave += (sender, args) => button.BackColor = Color.Transparent;

            button.Click += action;

            return button;
        }

        private static TrackBar TrackBarBuilder(string name, EventHandler scroll)
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
                Maximum = 4,
                TickFrequency = 1,
            };
            
            trackBar.Scroll += scroll;

            return trackBar;
        }

        private static CheckBox CheckBoxBuilder(string name, EventHandler checkedChanged)
        {
            var checkBox = new CheckBox()
            {
                Text = name,
                Size = menuButtonSize,
                Font = menuButtonFont,
                ForeColor = Color.DarkGoldenrod,
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Right,
                BackColor = Color.Transparent,
                Checked = false,
            };
            
            checkBox.GotFocus += (sender, args) => checkBox.BackColor = Color.DarkSlateBlue;
            checkBox.MouseEnter += (sender, args) => checkBox.BackColor = Color.DarkSlateBlue;

            checkBox.LostFocus += (sender, args) => checkBox.BackColor = Color.Transparent;
            checkBox.MouseLeave += (sender, args) => checkBox.BackColor = Color.Transparent;

            checkBox.CheckedChanged += checkedChanged;

            return checkBox;
        }
    }
}