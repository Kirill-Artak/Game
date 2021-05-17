namespace Game
{
    public class Level : ILevel
    {
        public LevelCell[,] LevelMash { get; }
        public int CellLength { get; }

        public Level(LevelCell[,] levelMash)
        {
            LevelMash = levelMash;
            CellLength = 72;
        }

        public bool CheckRight(int x, int y)
        {
            switch (LevelMash[x / CellLength, y / CellLength].Type)
            {
                case Cells.Space:
                case Cells.Ammo:
                case Cells.Health:
                    return true;
                case Cells.Platform:
                    return false;
            }

            return true;
        }

        public bool CheckLeft(int x, int y)
        {
            switch (LevelMash[x / CellLength, y / CellLength].Type)
            {
                case Cells.Space:
                case Cells.Ammo:
                case Cells.Health:
                    return true;
                case Cells.Platform:
                    return false;
            }

            return true;
        }

        public bool CheckUp(int x, int y)
        {
            switch (LevelMash[x / CellLength, y / CellLength].Type)
            {
                case Cells.Space:
                case Cells.Ammo:
                case Cells.Health:
                    return true;
                case Cells.Platform:
                    return false;
            }

            return true;
        }

        public bool CheckDown(int x, int y)
        {
            switch (LevelMash[x / CellLength, y / CellLength].Type)
            {
                case Cells.Space:
                case Cells.Ammo:
                case Cells.Health:
                    return true;
                case Cells.Platform:
                    return false;
            }

            return true;
        }
    }
}