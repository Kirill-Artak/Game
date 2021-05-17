using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;

namespace Game
{
    public class Textures
    {
        public static ReadOnlyDictionary<Cells, Image> TexturesDictionary;

        static Textures()
        {
            var dictionary = new Dictionary<Cells, Image>();
            dictionary.Add(Cells.Platform, Image.FromFile(@"assets\textures\Platform.bmp"));
            dictionary.Add(Cells.Space, Image.FromFile(@"assets\textures\sky.bmp"));

            TexturesDictionary = new ReadOnlyDictionary<Cells, Image>(dictionary);
        }
    }
}