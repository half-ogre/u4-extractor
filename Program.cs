using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace u4_extractor
{
    class Program
    {
        static ColorPalette palette;
        
        static void Main(string[] args)
        {
            if (args.Length < 1 || args.Length > 2)
            {
                Console.WriteLine();
                Console.WriteLine("Usage: u4-extractor.exe {path-to-ultima-iv-folder} [output-folder-for-tiles]");
                Console.WriteLine();
                return;
            }
            
            var u4Path = args[0];

            var shapesPath = Path.Combine(u4Path, "shapes.ega");

            var outputPath = @".\tiles";

            if (args.Length == 2)
                outputPath = args[1];

            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            var shapesBytes = File.ReadAllBytes(shapesPath);

            for (var n = 0; n < 256; n++)
            {
                var tileBytes = shapesBytes
                    .Skip(8*16*n)
                    .Take(8*16)
                    .ToArray();
                
                var tile = MakeTile(tileBytes);

                tile.Save(Path.Combine(outputPath, String.Concat("tile-", n, ".bmp")));
            }
        }

        static Bitmap MakeTile(byte[] tileBytes)
        {
            var handle = GCHandle.Alloc(tileBytes, GCHandleType.Pinned);

            var tile = new Bitmap(16, 16, 8, PixelFormat.Format4bppIndexed, handle.AddrOfPinnedObject())
            {
                Palette = MakePalette()
            };

            handle.Free();

            return tile;
        }

        static ColorPalette MakePalette()
        {
            if (palette != null)
                return palette;

            var bmp = new Bitmap(1, 1, PixelFormat.Format4bppIndexed);
            palette = bmp.Palette;
            palette.Entries[0] = Color.FromArgb(0, 0, 0);
            palette.Entries[1] = Color.FromArgb(0, 0, 162);
            palette.Entries[2] = Color.FromArgb(0, 162, 0);
            palette.Entries[3] = Color.FromArgb(0, 162, 162);
            palette.Entries[4] = Color.FromArgb(162, 0, 0);
            palette.Entries[5] = Color.FromArgb(162, 0, 162);
            palette.Entries[6] = Color.FromArgb(170, 85, 0);
            palette.Entries[7] = Color.FromArgb(168, 168, 168);
            palette.Entries[8] = Color.FromArgb(82, 82, 82);
            palette.Entries[9] = Color.FromArgb(80, 80, 255);
            palette.Entries[10] = Color.FromArgb(80, 255, 80);
            palette.Entries[11] = Color.FromArgb(80, 255, 255);
            palette.Entries[12] = Color.FromArgb(255, 80, 80);
            palette.Entries[13] = Color.FromArgb(255, 80, 255);
            palette.Entries[14] = Color.FromArgb(255, 255, 80);
            palette.Entries[15] = Color.FromArgb(255, 255, 255);

            return palette;
        }
    }
}