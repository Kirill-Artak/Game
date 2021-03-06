using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NUnit.Framework;

namespace Game
{
    public class Player
    {
        public Image ImageRight { get; }
        public Image ImageLeft { get; }
        
        public int Health => health;
        public int Bonus => bonus;

        public int X => x;
        public int Y => y;
        public Side Side => (Side) side;
        public int Line => (y + Height / 2) / 72;
        public IWeapon Weapon { get; private set; }
        public Level Level { get; private set; }

        public bool IsFalling { get; private set; }
        public bool IsJumping { get; private set; }

        public bool IsDamage { get; private set; }
        
        public bool IsDead { get; private set; }

        public bool CanAttack { get; set; }
        
        public int Height { get; }
        public int Width { get; }

        public readonly Action OnDeathAction;
        private readonly Action onDamage;

        private int health = 5;
        private int bonus;
        private int x;
        private int y;
        private int side = 1;
        
        public bool IsRight { get; private set; }
        public bool IsLeft { get; private set; }

        public Enemy FocusedEnemy { get; private set; }

        private HashSet<Enemy> focusedEnemies = new HashSet<Enemy>();

        

        public Player(Action onDeathAction = null, Action onDamage = null)
        {
            ImageRight = System.Drawing.Image.FromFile(@"assets\Player\player.png");
            ImageLeft = (Image) ImageRight.Clone();
            
            ImageLeft.RotateFlip(RotateFlipType.RotateNoneFlipX);

            Height = 54; //72
            Width = 28;  //38
            
            OnDeathAction = onDeathAction;
            this.onDamage = onDamage;
        }

        public void SetCoordinate(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public void SetLevel(Level level) => Level = level;

        public Task MoveRight() => new Task(() =>
        {
            IsRight = true;
            Interlocked.Exchange(ref side, 1);
            if (Level.Check(x + Width + 15, y + Height - 10, true) 
                && Level.Check(x + Width + 15, y + 10, true))
                Interlocked.Add(ref x, 4);
            if (Level.Check(x + Width + 15, y + Height - 10, true) 
                && Level.Check(x + Width + 15, y + 10, true))
                Interlocked.Add(ref x, 4);
            //if (Level.CheckDown(x, y))
            //    Fall();
            IsRight = false;
        });

        public Task MoveLeft() => new Task(() =>
        {
            IsLeft = true;
            Interlocked.Exchange(ref side, -1);
            if (Level.Check(x - 15, y + 10, true) 
                && Level.Check(x - 15, y + Height - 10, true))
                Interlocked.Add(ref x, -4);
            if (Level.Check(x - 15, y + 10, true) 
                && Level.Check(x - 15, y + Height - 10, true))
                Interlocked.Add(ref x, -4);
            //if (Level.CheckDown(x, y))
            //    Fall();

            IsLeft = false;
        });
        
        public Task Jump() => new Task(() =>
        {
            if (IsFalling) return;
            if (IsJumping) return;
            
            IsJumping = true;
            
            var force = 25;
            while (Level.Check(x, y, true) && Level.Check(x + Width, y, true) && force > 0)
            {
                Interlocked.Add(ref y, -force);
                force -= 1;
                Thread.Sleep(10);
            }

            IsJumping = false;
        });

        public Task Fall() => new Task(() =>
        {
            if (IsFalling) return;
            
            var force = 13;
            while (Level.Check(x, y + Height, true) 
                   && Level.Check(x + Width, y + Height, true))
            {
                IsFalling = true;
                Interlocked.Add(ref y, 10);
                if (force > 1)
                    force--;
                Thread.Sleep(force);
            }

            IsFalling = false;
        });

        public void GetDamage(Side direction)
        {
            if (IsDead) return;
            
            onDamage();
            
            var kb = KnockBack(direction);
            kb.Start();
            kb.ContinueWith(t => Fall().Start());
            
            Interlocked.Decrement(ref health);
            if (health <= 0)
            {
                IsDead = true;
                //onDeathAction();
            }
        }

        public Task Damage() => new Task(() =>
        {
            if (!CanAttack) return;
            if (FocusedEnemy == null) return;
            if (FocusedEnemy.Distance(x) < Level.CellLength * 2)
            {
                IsDamage = true;

                FocusedEnemy.GetDamage().Start();
            
                Thread.Sleep(750);
                IsDamage = false;
            }
        });

        public void Focus(Enemy enemy)
        {
            //focusedEnemies.Add(enemy);

            //lock (enemy)
            {
                if (FocusedEnemy == null || FocusedEnemy.IsDead)
                {
                    enemy.IsFocused = true;
                    CanAttack = true;
                    FocusedEnemy = enemy;
                }
            }
        }

        public void Unfocus(Enemy enemy)
        {
            //focusedEnemies.Remove(enemy);
            if (enemy != FocusedEnemy) return;
            
            enemy.IsFocused = false;
            CanAttack = false;
            FocusedEnemy = null;
        }

        public void PickupBullet() => Weapon.PickupBullet();

        public bool PickupItem(ItemType type)
        {
            switch (type)
            {
                case ItemType.Health:
                    if (health <= 9)
                    {
                        Interlocked.Increment(ref health);
                        return true;
                    }
                    break;
                case ItemType.Bonus:
                    Interlocked.Increment(ref bonus);
                    return true;
            }

            return false;
        }

        public void HealthUp()
        {
            health = 5;
            bonus = 0;
            IsDead = false;
            FocusedEnemy = null;
        }

        private Task KnockBack(Side direction) => new Task(() =>
        {
            var power = 20 * (int) direction;
            var distance = x + 18 + 20 *(int) direction;
            
            if (Level.Check(x - 15, y + 10, true) 
                && Level.Check(distance, y + Height - 10, true))
                Interlocked.Add(ref x, power);
        });
    }
}