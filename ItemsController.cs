using System;
using System.Collections;
using System.Collections.Generic;

namespace Game
{
    public class ItemsController
    {
        private List<ItemController> Controllers { get; }

        public ItemsController(IEnumerable<Item> items, Player player)
        {
            Controllers = new List<ItemController>();

            foreach (var e in items)
            {
                Controllers.Add(new ItemController(e, player));
            }
        }

        public void OnTick(object s, EventArgs args)
        {
            foreach (var e in Controllers)
            {
                if (!e.IsUsed)
                    e.Check().Start();
            }
        }
    }
}