using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;

namespace Game
{
    public class EnemyController
    {
        public Enemy Enemy { get; }
        
        public Player Player { get; }
        
        public Level Level { get; }

        public bool IsPaused { get; set; }

        public EnemyController(Enemy enemy, Player player, Level level)
        {
            Enemy = enemy;
            Player = player;
            Level = level;
        }
        
        public Task Focus() => new Task(() =>
        {
            if (Math.Abs(Enemy.X - Player.X) <= Level.CellLength * 2 
                && Enemy.Line == Player.Line)
            {
                Player.Focus(Enemy);
            }
            else if (Player.FocusedEnemy == Enemy)
            {
                Player.Unfocus(Enemy);
            }
        });

        public Task Move() => new Task(() =>
        {
            if (CheckPlayer() && !Enemy.IsTriggered)
            {
                Enemy.IsTriggered = true;
                
                //Thread.Sleep(500);
                
                while (Math.Abs(Player.X - Enemy.X) > Level.CellLength
                        && !IsPaused)
                {
                    if (Enemy.IsWalk) continue; 
                    
                    if (Player.X - Enemy.X < 0)
                        Enemy.MoveLeft().Start();
                    else
                        Enemy.MoveRight().Start();
                    
                    Thread.Sleep(40);
                }

                Thread.Sleep(200);

                while (!Enemy.IsDead
                       && Math.Abs(Player.X - Enemy.X) <= Level.CellLength + 10 
                       && Enemy.Line == Player.Line 
                       && !IsPaused)
                {
                    Player.GetDamage(Enemy.Side);
                    Thread.Sleep(1000);
                }
                
                Enemy.IsTriggered = false;
            }
        });
        

        private bool CheckPlayer()
        {
            return Enemy.Y / Level.CellLength == Player.Y / Level.CellLength
                   && Math.Abs(Player.X - Enemy.X) < Level.CellLength * 6;
        }
    }
}
