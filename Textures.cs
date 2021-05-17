using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Game
{
    public class Textures
    {
        public static ReadOnlyDictionary<Cells, string> TexturesDictionary;

        static Textures()
        {
            var dictionary = new Dictionary<Cells, string>();
            dictionary.Add(Cells.Platform, @"assets\textures\Platform.bmp");
            dictionary.Add(Cells.Space, @"assets\textures\sky.bmp");

            TexturesDictionary = new ReadOnlyDictionary<Cells, string>(dictionary);
        }
    }
}