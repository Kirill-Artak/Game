using System.Drawing;
using System.Windows.Forms;

namespace Game
{
    public class Level
    {
        public int EndGameX { get; private set; }
        public int EndGameY { get; private set; }
        
        public Image Background { get; private set; }
        public Image Wall { get; private set; }
        public Image Border { get; private set; }
        public Image Start { get; private set; }
        public string Audio { get; private set; }
        
        public LevelCell[,] LevelMash { get; private set; }
        public Enemy[] Enemies { get; private set; }
        public Item[] Items { get; private set; }
        public int CellLength { get; }
        
        public bool IsBackgroundMoving { get; set; }
        public bool IsOutdoor { get; set; }
        public bool HasWall { get; set; }
        
        public Level()
        {
            CellLength = 72;
        }
        
        public Level(LevelCell[,] levelMash)
        {
            LevelMash = levelMash;
            CellLength = 72;
        }

        public void Rebuild()
        {
            foreach (var e in Enemies)
            {
                e.ToStart();
            }

            foreach (var e in Items)
            {
                e.IsUsed = false;
            }
        }

        public void AddMash(LevelCell[,] levelMash)
        {
            LevelMash = levelMash;
        }

        public void AddEnemies(Enemy[] enemies)
        {
            Enemies = enemies;
        }

        public void AddItems(Item[] items)
        {
            Items = items;
        }

        public void AddAudio(string path)
        {
            Audio = path;
        }

        public void AddEndOfLevel(int x, int y)
        {
            EndGameX = x;
            EndGameY = y;
        }

        public void SetStart(string path)
        {
            Start = Image.FromFile(path);
        }

        public void AddBorder(string path)
        {
            Border = Image.FromFile(path);
        }

        public bool Check(int x, int y, bool isPlayer)
        {
            var result = true;

            if (x <= 0 || x >= 8432)
                return false;
            
            switch (LevelMash[x / CellLength, y / CellLength].Type)
            {
                case Cells.Space:
                case Cells.Ammo:
                case Cells.Health:
                    result =  true;
                    break;
                case Cells.Ground:
                case Cells.Conditioner:
                case Cells.Platform:
                    result =  false;
                    break;
                default:
                    result =  true;
                    break;
            }

            if (!isPlayer && LevelMash[x / CellLength, y / CellLength + 1].Type == Cells.Space)
                result = false;

            return result;
        }

        public void SetBackground(string background)
        {
            Background = Image.FromFile(background);
        }

        public void SetWall(string wall)
        {
            Wall = Image.FromFile(wall);
        }
    }
}