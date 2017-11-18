using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace TurboJpegWrapper
{
    public static class ConsoleTest
    {
        public static void Main(string[] args)
        {
            TurboJpegImport.Load();
            var i1 = new Bitmap(100, 100);
            var g = Graphics.FromImage(i1);
            var os = GetBuffer(i1).Length;
            string hw = "Hello World!", osi = "RawSize: " + os.ToString("n0");
            g.DrawString(hw, new Font(FontFamily.GenericSansSerif,10, FontStyle.Regular), new SolidBrush(Color.Yellow), 0, 35);
            g.DrawString(osi, new Font(FontFamily.GenericSansSerif, 8, FontStyle.Regular), new SolidBrush(Color.Yellow), 0, 50);
            //TurboJpegWrapper.TJCompressor c = new TJCompressor();
            TurboJpegWrapper.TJCompressor c = new TJCompressor(true);
            var data = c.Compress(i1, TJSubsamplingOptions.TJSAMP_420, 50, TJFlags.FASTDCT);
            TurboJpegWrapper.TJDecompressor d = new TJDecompressor();
            var i2 = d.Decompress(data, System.Drawing.Imaging.PixelFormat.Format32bppArgb, TJFlags.FASTDCT);
            g = Graphics.FromImage(i2);
            var ns = data.Length;
            var nsi = "JpegSize: " + ns.ToString("n0");
            g.DrawString(nsi, new Font(FontFamily.GenericSansSerif, 8, FontStyle.Regular), new SolidBrush(Color.Red), 0, 65);
            i1.Save(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) , "TurboJpegWrapper.Test.1.bmp"));
            i2.Save(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "TurboJpegWrapper.Test.2.bmp"));
            foreach (var s in new[] { hw, osi, nsi }) Console.WriteLine(s);
            if (os <= ns)
                throw new Exception("No jpeg compression occurred.");
            else if ((os != 40000) || (ns != 1790))
                throw new Exception("Compression occurred but either Original image bytes: " + os + " or Compressed image bytes: " + ns + " were not as expected.");
            else
                Console.WriteLine("Compression succeeded.");
            Console.WriteLine("enter to quit...");
            Console.ReadLine();

        }

        /// <summary>
        /// convert bitmap to byte array
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static byte[] GetBuffer(Bitmap img)
        {
            //via: SLICO.cs (also: http://stackoverflow.com/questions/7350679/convert-a-bitmap-into-a-byte-array)
            // Get total locked pixel count
            // int pixelCount = width*height;

            // Get source bitmap pixel format size
            var depth = Bitmap.GetPixelFormatSize(img.PixelFormat);
            //int bytesPerPixel = depth/8;

            // Lock the bitmap's bits
            Rectangle rect = new Rectangle(0, 0, img.Width, img.Height);
            BitmapData imgBitmapData = img.LockBits(rect,
                                                            ImageLockMode.ReadOnly,
                                                            img.PixelFormat);

            // Get the address of the first line
            IntPtr ptr = imgBitmapData.Scan0;

            // Allocate size of the array to hold the bytes of the bitmap
            // in byteCount = imgBitmapData.Stride * imgBitmapData.Height;
            int byteCount = img.Width * img.Height * BytePerPixel(depth);// sw.depth / 8;
            var imgBuff = new byte[byteCount];

            // copy the RGB value into the array
            System.Runtime.InteropServices.Marshal.Copy(ptr, imgBuff, 0, byteCount);

            img.UnlockBits(imgBitmapData);
            return imgBuff;
        }

        private static int BytePerPixel(int m_depth)
        {
            if (m_depth == 8 || m_depth == 24 || m_depth == 32)
            {
                return m_depth / 8;
            }
            else
            {
                throw new ArgumentException("The bbp of the image should be 8, 24 or 32!");
            }
        }

    }
}
