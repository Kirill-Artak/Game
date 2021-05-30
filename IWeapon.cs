namespace Game
{
    public interface IWeapon
    {
        int BulletCount { get; set; }
        Level Level { get; }
        void Use(Side side);
        void PickupBullet();
    }
}