using System.Security.Cryptography.X509Certificates;

namespace Game
{
    public interface ILevel
    {
        bool Check(int x, int y, bool isPlayer);
    }
}