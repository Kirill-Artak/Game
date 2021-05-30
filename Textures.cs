using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;

namespace Game
{
    public class Textures
    {
        public static ReadOnlyDictionary<Cells, Image> TexturesDictionary;
        public static ReadOnlyDictionary<ItemType, Image> BonusTexturesDictionary;

        static Textures()
        {
            var texDictionary = new Dictionary<Cells, Image>();
            texDictionary.Add(Cells.Ground, Image.FromFile(@"assets\textures\gnd.jpg"));
            texDictionary.Add(Cells.Ground2, Image.FromFile(@"assets\textures\gnd2.jpg"));

            texDictionary.Add(Cells.Space, null);
            texDictionary.Add(Cells.Enemy, null);
            texDictionary.Add(Cells.Health, null);
            texDictionary.Add(Cells.Bonus, null);
            texDictionary.Add(Cells.Final, Image.FromFile(@"assets\textures\gnd.jpg"));
            texDictionary.Add(Cells.Conditioner, Image.FromFile(@"assets\textures\cond.png"));
            
            var bonusDictionary = new Dictionary<ItemType, Image>();
            bonusDictionary.Add(ItemType.Health, Image.FromFile(@"assets\textures\bottle.png"));
            bonusDictionary.Add(ItemType.Bonus, Image.FromFile(@"assets\textures\bonus.png"));

            TexturesDictionary = new ReadOnlyDictionary<Cells, Image>(texDictionary);
            BonusTexturesDictionary = new ReadOnlyDictionary<ItemType, Image>(bonusDictionary);
        }
    }
}