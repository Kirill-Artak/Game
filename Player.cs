using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Game
{
    public class Player
    {
        public int Health => health;
        public int X => x;
        public int Y => y;
        public Side Side => (Side) side;
        public IWeapon Weapon { get; private set; }
        public ILevel Level { get; }

        public bool IsFalling => isFalling;

        private readonly Action onDeathAction;

        private int health = 10;
        private int x;
        private int y;
        private int side = 1;

        private bool isFalling;

        public Player(ILevel level = null, Action onDeathAction = null)
        {
            Level = level;
            this.onDeathAction = onDeathAction;
        }

        public void MoveRight() => Move(() =>
        {
            //Interlocked.Exchange(ref side, 1);
            //if (Level.CheckRight(x, y))
                Interlocked.Increment(ref x);
            //if (Level.CheckDown(x, y))
            //    Fall();
        });

        public void MoveLeft() => Move(() =>
        {
            Interlocked.Exchange(ref side, -1);
            if (Level.CheckLeft(x, y))
                Interlocked.Decrement(ref x);
            if (Level.CheckDown(x, y))
                Fall();
        });
        
        public void Jump()
        {
            Move(() =>
            {
                var force = 8;
                while (Level.CheckUp(x, y) && force > 0)
                {
                    Interlocked.Add(ref y, -force);
                    force -= 2;
                    Task.Delay(10);
                }
            }).ContinueWith(task1 => Fall());
        }

        public void Fall()
        {
            Move(() =>
            {
                var force = 11;
                while (Level.CheckDown(x, y))
                {
                    isFalling = true;
                    Interlocked.Decrement(ref y);
                    if (force >= 3)
                        force -= 2;
                    Task.Delay(force);
                }

                isFalling = false;
            });
        }
        
        public void GetDamage() => Task.Run(() =>
        {
            Interlocked.Decrement(ref health);
            if (health <= 0) 
                onDeathAction();
        });

        public void Damage() => Weapon.Use(Side);

        public void PickupBullet() => Weapon.PickupBullet();

        public void PickupHealth()
        {
            if (health <= 9)
                Interlocked.Increment(ref health);
        }

        private Task Move(Action action) => Task.Run(action);

        
        
        
    }
}