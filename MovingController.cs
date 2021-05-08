using System.Windows.Forms;

namespace Game
{
    public class MovingController
    {
        public Player Player { get; }
        
        private bool right;
        private bool left;
        private bool jump;

        private bool isSpacePressed;

        public MovingController(Player player)
        {
            Player = player;
        }

        public void AddMovement(Keys key)
        {
            switch (key)
            {
                case Keys.D:
                    right = true;
                    break;
                case Keys.A:
                    left = true;
                    break;
                case Keys.W:
                case Keys.Space:
                    if (!Player.IsFalling && !Player.IsJumping && !isSpacePressed) 
                        jump = true;
                    isSpacePressed = true;
                    break;
            }
        }

        public void RemoveMovement(Keys key)
        {
            switch (key)
            {
                case Keys.D:
                    right = false;
                    break;
                case Keys.A:
                    left = false;
                    break;
                case Keys.Space:
                    isSpacePressed = false;
                    break;
            }
        }

        public void Move()
        {
            if (right)
            {
                Player.MoveRight().Start();
            }
            if (left)
            {
                Player.MoveLeft().Start();
            }
            if (jump && !Player.IsFalling)
            {
                jump = false;
                var task = Player.Jump();
                task.Start();
                task.ContinueWith(task1 => Player.Fall().Start());
            }
        }
    }
}