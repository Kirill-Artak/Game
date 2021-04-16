using System.Drawing;

namespace Game
{
    public class Player
    {
        public int Health { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public IWeapon Weapon { get; private set; }
        
        public ILevel Level { get; }

        private Player(ILevel level)
        {
            Level = level;
        }
        
        public void MoveRight()
        {
            if (Level.CheckCoordinate(X + 5, Y))
                X += 5;
        }

        public void MoveLeft()
        {
            if (Level.CheckCoordinate(X - 5, Y))
                X -= 5;
        }

        public void Jump()
        {
            MoveUp();
            MoveDown();
        }

        private void MoveUp()
        {
            for (int i = 0; i < 10; i++)
                if (Level.CheckCoordinate(X, Y + 5))
                    Y += 5;
                else
                    return;
        }

        private void MoveDown()
        {
            for (int i = 0; i < 10; i++)
                if (Level.CheckCoordinate(X, Y - 5))
                    Y -= 5;
                else
                    return;
        }

        public void Damage()
        {
            
        }
    }
}