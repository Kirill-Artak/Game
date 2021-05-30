using System;
using System.Threading.Tasks;

namespace Game
{
    public class Item
    {
        public ItemType ItemType { get; set; }

        public int X => x;
        public int Y => y;

        public bool IsUsed { get; set; }

        private int x;
        private int y;

        public Item(ItemType itemType, int x, int y)
        {
            this.x = x;
            this.y = y;

            ItemType = itemType;

            IsUsed = false;
        }
    }
}