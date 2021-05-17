using System.Drawing;

namespace Game
{
    public class LevelCell
    {
        public const int CellHeight = 10;
        
        public Cells Type { get; }
        public int X { get; }
        public int Y { get; }
        public Image Texture { get; }

        public LevelCell(Cells type, int x, int y, Image texture = null)
        {
            Type = type;
            X = x;
            Y = y;
            Texture = texture;
        }
    }
}