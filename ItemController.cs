using System;
using System.Threading.Tasks;

namespace Game
{
    public class ItemController
    {
        public Item Item { get; }
        public Player Player { get; }
        public bool IsUsed => Item.IsUsed;

        public ItemController(Item item, Player player)
        {
            Item = item;
            Player = player;
        }

        public Task Check() => new Task(() =>
        {
            if (Math.Abs(Player.X - Item.X) < 40 && Math.Abs(Player.Y - Item.Y) < 40)
            {
                Item.IsUsed = Player.PickupItem(Item.ItemType);
            }
        });
    }
}