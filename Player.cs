using System.Drawing;

namespace Game
{
    public class Player
    {
        public int Health { get; private set; }
        public Point DeltaPosition { get; private set; }
        public IWeapon Weapon { get; private set; }

        private Player()
        {
            
        }
        
        public Player MoveRight()
        {
            return new Player()
            {
                Health = Health,
                DeltaPosition = DeltaPosition,
                Weapon = Weapon
            };
        }

        public void MoveLeft()
        {
            
        }

        public void Jump()
        {
            
        }

        public void Damage()
        {
            
        }
    }
}