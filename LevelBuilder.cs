using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Game
{
    public class LevelBuilder
    {
        public const int LevelHeight = 10;
        public const int LevelWidth = 32;

        public static Level[] BuildFromFiles(params string[] files)
        {
            var result = new List<Level>();
            
            foreach (var e in files)
            {
                var level = File.ReadAllText(e);
                
                result.Add(BuildFromString(level));
            }

            return result.ToArray();
        }
        
        public static Level BuildFromString(string source)
        {
            var lines = source.Split(new[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            
            //if (lines.Length != 10) throw new ArgumentOutOfRangeException("Слишком много строк в уровне");
            //if (lines.Any(x => x.Length != 10)) throw new ArgumentOutOfRangeException("Слишком много символов в строке");
            
            return FromLines(lines);
        }

        public static Level FromLines(string[] lines)
        {
            var level = new Level();
            
            var dungeon = new LevelCell[128, 10];
            var enemies = new List<Enemy>();
            var items = new List<Item>();

            var endX = -100;
            var endY = -100;
            //var initialPosition = Point.Empty;
            //var exit = Point.Empty;
            //var chests = new List<Point>();

            for (var y = 0; y < 10; y++)
            {
                for (var x = 0; x < 128; x++)
                {
                    var type = (Cells) lines[y][x];
                    if (type == Cells.Enemy)
                        enemies.Add(new Enemy(level, x * 72, y * 72));
                    if (type == Cells.Bonus)
                        items.Add(new Item(ItemType.Bonus, x * 72 + 36, y * 72 + 40));
                    if (type == Cells.Health)
                        items.Add(new Item(ItemType.Health, x * 72 + 36, y * 72 + 40));
                    if (type == Cells.Final)
                    {
                        endX = x * 72 + 36;
                        endY = y * 72 + 36;
                    }
                    dungeon[x, y] = new LevelCell(type, x, y, Textures.TexturesDictionary[type]);
                }
            }

            level.IsBackgroundMoving = lines[10][0] == 'a';
            level.IsOutdoor = lines[10][1] == 'a';
            level.HasWall = lines[10][2] == 'a';
            
            level.AddMash(dungeon);
            level.AddEnemies(enemies.ToArray());
            level.AddItems(items.ToArray());
            level.AddEndOfLevel(endX, endY);
            level.AddBorder(lines[13]);
            
            level.SetBackground(lines[11]);
            if (level.HasWall)
                level.SetWall(lines[12]);
            
            level.AddAudio(lines[14]);
            
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
            sb.Append('E');
            sb.Append(new string(' ', 11) + '\n');

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