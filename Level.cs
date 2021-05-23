using System.Drawing;

namespace Game
{
    public class Level : ILevel
    {
        public Image Background { get; private set; }
        public Image Wall { get; private set; }
        
        public LevelCell[,] LevelMash { get; private set; }
        public Enemy[] Enemies { get; private set; }
        public int CellLength { get; }

        public Level()
        {
            CellLength = 72;
        }
        
        public Level(LevelCell[,] levelMash)
        {
            LevelMash = levelMash;
            CellLength = 72;
        }

        public void AddMash(LevelCell[,] levelMash)
        {
            LevelMash = levelMash;
        }

        public void AddEnemies(Enemy[] enemies)
        {
            Enemies = enemies;
        }

        public bool Check(int x, int y, bool isPlayer)
        {
            var result = true;
            
            switch (LevelMash[x / CellLength, y / CellLength].Type)
            {
                case Cells.Space:
                case Cells.Ammo:
                case Cells.Health:
                    result =  true;
                    break;
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