using System.Security.Cryptography.X509Certificates;

namespace Game
{
    public interface ILevel
    {
        bool CheckRight(int x, int y);
        bool CheckLeft(int x, int y);
        bool CheckUp(int x, int y);
        bool CheckDown(int x, int y);
    }
}