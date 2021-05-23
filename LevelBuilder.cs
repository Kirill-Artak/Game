using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class LevelBuilder
    {
        public const int LevelHeight = 10;
        public const int LevelWidth = 32;
        
        public Level BuildFromString(string source)
        {
            var lines = source.Split(new[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            
            //if (lines.Length != 10) throw new ArgumentOutOfRangeException("Слишком много строк в уровне");
            //if (lines.Any(x => x.Length != 10)) throw new ArgumentOutOfRangeException("Слишком много символов в строке");
            
            return FromLines(lines);
        }

        public static Level FromLines(string[] lines)
        {
            var level = new Level();
            
            var dungeon = new LevelCell[32, 10];
            var enemies = new List<Enemy>();
            //var initialPosition = Point.Empty;
            //var exit = Point.Empty;
            //var chests = new List<Point>();
            for (var y = 0; y < 10; y++)
            {
                for (var x = 0; x < 32; x++)
                {
                    var type = (Cells) lines[y][x];
                    if (type == Cells.Enemy)
                        enemies.Add(new Enemy(level, x * 72, y * 72));
                    dungeon[x, y] = new LevelCell(type, x, y, Textures.TexturesDictionary[type]);
                }
            }
            
            level.AddMash(dungeon);
            level.AddEnemies(enemies.ToArray());
            
            return level;
        }

        public static string TestLevel;

        static LevelBuilder()
        {
            /*
            var skyLine = new string(' ', 32);
            var sky = "";

            for (int i = 0; i < 7; i++)
                sky += skyLine + '\n';
            
            var platformLine = new string('#', 32);
            var platform = "";

            for (int i = 0; i < 2; i++)
                platform += platformLine + '\n';

            TestLevel = sky + platform + platformLine;
            */
            
            var sb = new StringBuilder();


            for (var i = 0; i < 5; i++)
                sb.Append(new string(' ', 32) + '\n');

            sb.Append(new string(' ', 19));
            sb.Append('E');
            sb.Append(new string(' ', 12) + '\n');

            for (int i = 0; i < 3; i++)
            {
                sb.Append(new string(' ', 7 - i));
                sb.Append(new string('#', 27 + i) + '\n');
            }

            sb.Append(new string('#', 32));

            TestLevel = sb.ToString();
        }
    }
}