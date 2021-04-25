namespace Game
{
    public interface IWeapon
    {
        int BulletCount { get; set; }
        ILevel Level { get; }
        void Use(Side side);
        void PickupBullet();
    }
}