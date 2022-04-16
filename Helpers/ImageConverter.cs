using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace SR_ImpEx.Helpers
{
    class ImageConverter
    {
        public static void DDSToPNG(string sourcePath, string targetPath)
        {
            var image = Pfim.Pfim.FromFile(sourcePath);

            PixelFormat format;

            switch (image.Format)
            {
                case Pfim.ImageFormat.Rgba32:
                    format = PixelFormat.Format32bppArgb;
                    break;
                default:
                    throw new NotImplementedException();
            }

            var handle = GCHandle.Alloc(image.Data, GCHandleType.Pinned);
            try
            {
                var data = Marshal.UnsafeAddrOfPinnedArrayElement(image.Data, 0);
                var bitmap = new Bitmap(image.Width, image.Height, image.Stride, format, data);
                bitmap.Save(Path.ChangeExtension(targetPath, ".png"), ImageFormat.Png);
            }
            finally
            {
                handle.Free();
            }
        }
    }
}
