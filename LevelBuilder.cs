using System;
using System.Linq;

namespace Game
{
    public class LevelBuilder
    {
        public const int LevelHeight = 10;
        public const int LevelWidth = 32;
        
        public Level BuildFromString(string source)
        {
            var mash = new Cells[10, 32];

            var lines = source.Split(new[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            
            //if (lines.Length != 10) throw new ArgumentOutOfRangeException("Слишком много строк в уровне");
            //if (lines.Any(x => x.Length != 10)) throw new ArgumentOutOfRangeException("Слишком много символов в строке");
            
            return FromLines(lines);
        }

        public static Level FromLines(string[] lines)
        {
            var dungeon = new LevelCell[32, 10];
            //var initialPosition = Point.Empty;
            //var exit = Point.Empty;
            //var chests = new List<Point>();
            for (var y = 0; y < 10; y++)
            {
                for (var x = 0; x < 32; x++)
                {
                    var type = (Cells) lines[y][x];
                    dungeon[x, y] = new LevelCell(type, x, y, Textures.TexturesDictionary[type]);
                }
            }
            
            return new Level(dungeon);
        }

        public static string TestLevel;

        static LevelBuilder()
        {
            var skyLine = new string(' ', 32);
            var sky = "";

            for (int i = 0; i < 7; i++)
                sky += skyLine + '\n';
            
            var platformLine = new string('#', 32);
            var platform = "";

            for (int i = 0; i < 2; i++)
                platform += platformLine + '\n';

            TestLevel = sky + platform + platformLine;
        }
    }
}