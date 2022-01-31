using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day20Cs
{
    internal static class Input
    {
        public static Tile[] ReadImagesFromFile(string path)
        {
            var totalImgs = File.ReadLines(path).Where(s => s.Contains("Tile")).Count();
            var images = new Tile[totalImgs];

            var state = InputParseState.TileId;
            int id = 0;
            string[] imgContent = new string[10];
            int imgContentInd = 0;
            int i = 0;
            foreach (var str in File.ReadLines(path))
            {
                switch (state)
                {
                    case InputParseState.TileId:
                        var tileRemoved = str.Replace("Tile ", "");
                        id = int.Parse(tileRemoved.Replace(":", ""));
                        state = InputParseState.ImgContent;
                        break;
                    case InputParseState.ImgContent:
                        imgContent[imgContentInd++] = str;
                        if (imgContentInd == 10)
                        {
                            state = InputParseState.Newline;
                        }
                        break;
                    case InputParseState.Newline:
                        imgContentInd = 0;
                        images[i++] = new Tile(id, imgContent);
                        imgContent = new string[10];
                        state = InputParseState.TileId;
                        break;
                    default:
                        break;
                }
            }
            if (state == InputParseState.Newline)
            {
                images[i++] = new Tile(id, imgContent);
            }

            return images;
        }
        enum InputParseState
        {
            TileId, ImgContent, Newline
        }
    }
}
