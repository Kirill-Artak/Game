namespace Game
{
    public class TestLevel : ILevel
    {
        public int LeftBorder { get; }
        public int RightBorder { get; }
        public int Platform { get; }

        public TestLevel(int leftBorder = 10, int rightBorder = 500, int platform = 500)
        {
            LeftBorder = leftBorder;
            RightBorder = rightBorder;
            Platform = platform;
        }

        public bool CheckRight(int x, int y) => x < RightBorder;
        public bool CheckLeft(int x, int y) => x > LeftBorder;
        public bool CheckUp(int x, int y) => true;
        public bool CheckDown(int x, int y) => y < Platform;
    }
}