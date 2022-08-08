using System;
using System.IO;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;

namespace SR_ImpEx.Helpers
{
    class ImageConverter
    {
        // https://stackoverflow.com/a/8843188/6788126
        private const int bytesPerPixel = 4;
        public static void DDSToPNG(string sourcePath, string targetPath)
        {
            if (!sourcePath.Contains(".dds")) sourcePath += ".dds";

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
                bitmap.Save(targetPath + ".png", ImageFormat.Png);
            }
            finally
            {
                handle.Free();
            }
        }
        public static Bitmap SwapRedAndBlueChannels(Bitmap bitmap)
        {
            var imageAttr = new ImageAttributes();

            imageAttr.SetColorMatrix(new ColorMatrix(
                new[]
                {
                    new[] {0.0F, 0.0F, 1.0F, 0.0F, 0.0F},
                    new[] {0.0F, 1.0F, 0.0F, 0.0F, 0.0F},
                    new[] {1.0F, 0.0F, 0.0F, 0.0F, 0.0F},
                    new[] {0.0F, 0.0F, 0.0F, 1.0F, 0.0F},
                    new[] {0.0F, 0.0F, 0.0F, 0.0F, 1.0F}
                }
            ));

            var temp = new Bitmap(bitmap.Width, bitmap.Height);

            GraphicsUnit pixel = GraphicsUnit.Pixel;
            using (Graphics g = Graphics.FromImage(temp))
            {
                g.DrawImage(bitmap, Rectangle.Round(bitmap.GetBounds(ref pixel)), 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, imageAttr);
            }

            return temp;
        }
        public static Bitmap SwapRedAndAlphaChannels(Bitmap bitmap)
        {
            var imageAttr = new ImageAttributes();

            imageAttr.SetColorMatrix(new ColorMatrix(
                new[]
                {
                    new[] {0.0F, 0.0F, 0.0F, 1.0F, 0.0F},
                    new[] {0.0F, 1.0F, 0.0F, 0.0F, 0.0F},
                    new[] {0.0F, 0.0F, 1.0F, 0.0F, 0.0F},
                    new[] {1.0F, 0.0F, 0.0F, 0.0F, 0.0F},
                    new[] {0.0F, 0.0F, 0.0F, 0.0F, 1.0F}
                }
            ));

            var temp = new Bitmap(bitmap.Width, bitmap.Height);

            GraphicsUnit pixel = GraphicsUnit.Pixel;
            using (Graphics g = Graphics.FromImage(temp))
            {
                g.DrawImage(bitmap, Rectangle.Round(bitmap.GetBounds(ref pixel)), 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, imageAttr);
            }

            return temp;
        }
        public static Bitmap ApplyAlphaChannel(Bitmap bitmap)
        {
            Bitmap temp = (Bitmap)bitmap.Clone();
            PixelFormat pixelFormat = bitmap.PixelFormat;
            Rectangle rectangle = new Rectangle(0, 0, temp.Width, temp.Height);
            BitmapData bitmapData = temp.LockBits(rectangle, ImageLockMode.ReadWrite, pixelFormat);
            IntPtr intPtr = bitmapData.Scan0;
            int numBytes = Math.Abs(bitmapData.Stride) * temp.Height;
            byte[] argbValues = new byte[numBytes];
            Marshal.Copy(intPtr, argbValues, 0, numBytes);

            for (int counter = 0; counter < argbValues.Length; counter += bytesPerPixel)
            {
                argbValues[counter + 3] = (byte)(255 - argbValues[counter + 3] * 0.9);
            }

            Marshal.Copy(argbValues, 0, intPtr, numBytes);
            temp.UnlockBits(bitmapData);

            return temp;
        }
        public static Bitmap SwapRedAndGreenChannels(Bitmap bitmap)
        {
            var imageAttr = new ImageAttributes();

            imageAttr.SetColorMatrix(new ColorMatrix(
                new[]
                {
                    new[] {0.0F, 1.0F, 0.0F, 0.0F, 0.0F},
                    new[] {1.0F, 0.0F, 0.0F, 0.0F, 0.0F},
                    new[] {0.0F, 0.0F, 1.0F, 0.0F, 0.0F},
                    new[] {0.0F, 0.0F, 0.0F, 1.0F, 0.0F},
                    new[] {0.0F, 0.0F, 0.0F, 0.0F, 1.0F}
                }
            ));

            var temp = new Bitmap(bitmap.Width, bitmap.Height);

            GraphicsUnit pixel = GraphicsUnit.Pixel;
            using (Graphics g = Graphics.FromImage(temp))
            {
                g.DrawImage(bitmap, Rectangle.Round(bitmap.GetBounds(ref pixel)), 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, imageAttr);
            }

            return temp;
        }
        
        internal static int NextPowerOfTwo(int width)
        {
            // Get the next highest power of two
            if (width < 0)
                throw new ArgumentOutOfRangeException("width");
            --width;
            width |= width >> 1;
            width |= width >> 2;
            width |= width >> 4;
            width |= width >> 8;
            width |= width >> 16;
            return width + 1;
        }

        internal static bool IsPowerOfTwo(int width)
        {
            // return if width is a power of two
            return (width & (width - 1)) == 0;
        }

        public static Bitmap DropBlueChannel(Bitmap bitmap)
        {
            var imageAttr = new ImageAttributes();

            imageAttr.SetColorMatrix(new ColorMatrix(
                new[]
                {
                    new[] {1.0F, 0.0F, 0.0F, 0.0F, 0.0F},
                    new[] {0.0F, 1.0F, 0.0F, 0.0F, 0.0F},
                    new[] {0.0F, 0.0F, 0.0F, 0.0F, 0.0F},
                    new[] {0.0F, 0.0F, 0.0F, 1.0F, 0.0F},
                    new[] {0.0F, 0.0F, 0.0F, 0.0F, 1.0F}
                }
            ));

            var temp = new Bitmap(bitmap.Width, bitmap.Height);

            GraphicsUnit pixel = GraphicsUnit.Pixel;
            using (Graphics g = Graphics.FromImage(temp))
            {
                g.DrawImage(bitmap, Rectangle.Round(bitmap.GetBounds(ref pixel)), 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, imageAttr);
            }

            return temp;
        }
        public static Bitmap ApplyAlphaChannel(Bitmap bitmap, Bitmap emissivityMap)
        {
            // Source Map
            Bitmap temp = (Bitmap)bitmap.Clone();
            PixelFormat pixelFormat = bitmap.PixelFormat;
            Rectangle rectangle = new Rectangle(0, 0, temp.Width, temp.Height);
            BitmapData bitmapData = temp.LockBits(rectangle, ImageLockMode.ReadWrite, pixelFormat);
            IntPtr intPtr = bitmapData.Scan0;
            int numBytes = Math.Abs(bitmapData.Stride) * temp.Height;
            byte[] argbValues = new byte[numBytes];
            Marshal.Copy(intPtr, argbValues, 0, numBytes);
            // Emissivity Map
            Bitmap emi = (Bitmap)emissivityMap.Clone();
            PixelFormat emiPixelFormat = emissivityMap.PixelFormat;
            Rectangle emiRectangle = new Rectangle(0, 0, emi.Width, emi.Height);
            BitmapData emiBitmapData = emi.LockBits(emiRectangle, ImageLockMode.ReadWrite, pixelFormat);
            IntPtr emiIntPtr = emiBitmapData.Scan0;
            int emiNumBytes = Math.Abs(emiBitmapData.Stride) * emi.Height;
            byte[] emiArgbValues = new byte[emiNumBytes];
            Marshal.Copy(emiIntPtr, emiArgbValues, 0, emiNumBytes);
            // Change the Alpha Channel on the Parameter Map in regards to Emission Map
            for (int counter = 0; counter < argbValues.Length; counter += bytesPerPixel)
            {
                // argbValues is in format BGRA (Blue, Green, Red, Alpha)

                // If 100% transparent, skip pixel
                if (argbValues[counter + bytesPerPixel - 1] == 0)
                    continue;

                int Sum = (emiArgbValues[counter + 0] + emiArgbValues[counter + 1] + emiArgbValues[counter + 2]) / 3;

                argbValues[counter + 3] = (byte)Sum;
            }

            Marshal.Copy(argbValues, 0, intPtr, numBytes);
            temp.UnlockBits(bitmapData);
            emi.UnlockBits(emiBitmapData);

            return temp;
        }
        public static Bitmap DropAlphaChannel(Bitmap bitmap, Boolean isEmissive = false)
        {
            Bitmap temp = (Bitmap)bitmap.Clone();
            PixelFormat pixelFormat = bitmap.PixelFormat;
            Rectangle rectangle = new Rectangle(0, 0, temp.Width, temp.Height);
            BitmapData bitmapData = temp.LockBits(rectangle, ImageLockMode.ReadWrite, pixelFormat);
            IntPtr intPtr = bitmapData.Scan0;
            int numBytes = temp.Width * temp.Height * bytesPerPixel;
            byte[] argbValues = new byte[numBytes];
            Marshal.Copy(intPtr, argbValues, 0, numBytes);

            for (int counter = 0; counter < argbValues.Length; counter += bytesPerPixel)
            {
                if (argbValues[counter + bytesPerPixel - 1] == 0)
                    continue;

                if ((isEmissive && (argbValues[counter + 0] + argbValues[counter + 1] + argbValues[counter + 2]) == 0) || !isEmissive)
                {
                    argbValues[counter + 3] = 0;
                }
            }

            Marshal.Copy(argbValues, 0, intPtr, numBytes);
            temp.UnlockBits(bitmapData);

            return temp;
        }
        public static Bitmap SwapGreenAndBlueChannels(Bitmap bitmap)
        {
            var imageAttr = new ImageAttributes();

            imageAttr.SetColorMatrix(new ColorMatrix(
                new[]
                {
                    new[] {1.0F, 0.0F, 0.0F, 0.0F, 0.0F},
                    new[] {0.0F, 0.0F, 1.0F, 0.0F, 0.0F},
                    new[] {0.0F, 1.0F, 0.0F, 0.0F, 0.0F},
                    new[] {0.0F, 0.0F, 0.0F, 1.0F, 0.0F},
                    new[] {0.0F, 0.0F, 0.0F, 0.0F, 1.0F}
                }
            ));

            var temp = new Bitmap(bitmap.Width, bitmap.Height);

            GraphicsUnit pixel = GraphicsUnit.Pixel;
            using (Graphics g = Graphics.FromImage(temp))
            {
                g.DrawImage(bitmap, Rectangle.Round(bitmap.GetBounds(ref pixel)), 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, imageAttr);
            }

            return temp;
        }
        public static byte[] ImageToByte(Image img)
        {
            System.Drawing.ImageConverter converter = new System.Drawing.ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }
        public static string ToHexString(Color c)
        {
           return $"#{c.R:X2}{c.G:X2}{c.B:X2}";
        }
        public static Bitmap SetImageOpacity(Bitmap bitmap, float opacity)
        {
            var imageAttr = new ImageAttributes();

            imageAttr.SetColorMatrix(new ColorMatrix(
                new[]
                {
                    new[] { -1f, 0, 0, 0, 0},
                    new[] { 0, -1f, 0, 0, 0},
                    new[] { 0, 0, -1f, 0, 0},
                    new[] { 0, 0, 0, 1f, 0},
                    new[] { 0, 0, 0, 0, 1f}
                }
            ));

            var temp = new Bitmap(bitmap.Width, bitmap.Height);

            GraphicsUnit pixel = GraphicsUnit.Pixel;
            using (Graphics g = Graphics.FromImage(temp))
            {
                g.DrawImage(bitmap, Rectangle.Round(bitmap.GetBounds(ref pixel)), 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, imageAttr);
            }

            return temp;
        }
        public static Bitmap MergeImages(Bitmap source, Bitmap target)
        {
            Bitmap temp = new Bitmap(source.Width, source.Height);

            using (Graphics g = Graphics.FromImage(temp))
            {
                g.DrawImage(source, Point.Empty);
                g.DrawImage(target, Point.Empty);
            }

            return temp;
        }
    }
}
