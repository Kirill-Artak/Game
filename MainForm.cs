using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Game
{
    public class MainForm : Form
    {
        private static Size menuButtonSize = new Size(180, 60);
        private static Font menuButtonFont = new Font("Arial", 16);
        private static Image backgroundImage = Image.FromFile(@"C:\Users\razzerone\Desktop\рисунки\a.png");
        
        private static Button NewGameButton = new Button()
        {
            Text = "Новая игра",
            Size = menuButtonSize,
            Font = menuButtonFont,
            Anchor = AnchorStyles.Right,
            FlatStyle = FlatStyle.Flat,
        };
        
        private static Button SettingsButton = new Button()
        {
            Text = "Настройки",
            Size = menuButtonSize,
            Font = menuButtonFont,
            Anchor = AnchorStyles.Right,
            FlatStyle = FlatStyle.Flat,
        };
        
        private static Button ExitButton = new Button()
        {
            Text = "Выход",
            Size = menuButtonSize,
            Font = menuButtonFont,
            Anchor = AnchorStyles.Right,
            FlatStyle = FlatStyle.Flat,
        };
        
        public MainForm()
        {
            Size = new Size(1280, 720);
            FormBorderStyle = FormBorderStyle.FixedDialog;

            var table = MenuBuilder();
            

            NewGameButton.Click += (sender, args) =>
            {
                Controls.Remove(table);
                
                Paint += OnPaint;
            };
            SettingsButton.Click += (sender, args) => Controls.Remove(table);
            ExitButton.Click += (sender, args) => Application.Exit();
            
            Controls.Add(table);
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.DrawImage(Image.FromFile(@"C:\Users\razzerone\Desktop\рисунки\playerSource.bmp"), new Point(0, 0));
        }

        private static TableLayoutPanel MenuBuilder()
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
            table.Controls.Add(NewGameButton, 0, 1);
            table.Controls.Add(SettingsButton, 0, 2);
            table.Controls.Add(ExitButton, 0, 3);

            return table;
        }
    }
}