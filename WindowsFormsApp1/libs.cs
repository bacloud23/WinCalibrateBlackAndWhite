using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static WindowsFormsApp1.libs;

namespace WindowsFormsApp1
{
    internal class libs
    {
        /*private bool IsBlankImage(Bitmap bmp)
        {
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);
            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;
            // Declare an array to hold the bytes of the bitmap.
            int bytes = bmpData.Stride * bmp.Height;
            byte[] rgbValues = new byte[bytes];

            // Copy the RGB values into the array.
            Marshal.Copy(ptr, rgbValues, 0, bytes);

            // Scanning for non-zero bytes
            bool allBlack = true;
            for (int index = 0; index < rgbValues.Length; index++)
                if (rgbValues[index] != 0)
                {
                    allBlack = false;
                    break;
                }
            // Unlock the bits.
            bmp.UnlockBits(bmpData);
            return allBlack;
        }*/

        public unsafe static uint[] GetAverageColorInRegion(Bitmap source)
        {

            var rectangle = new Rectangle(0, 0, source.Width, source.Height);
            var region = rectangle;
            var bitmapData = source.LockBits(rectangle, ImageLockMode.ReadOnly, source.PixelFormat);
            var pixelCount = region.Width * region.Height;
            var scanWidth = bitmapData.Stride / 4;

            uint[] totals = { 0, 0, 0 };
            int flushCounter = 0;
            uint sumRR00BB = 0;
            uint sum00GG00 = 0;

            for (var y = region.Y; y < region.Y + region.Height; y++)
            {
                uint* row = (uint*)bitmapData.Scan0 + y * scanWidth;

                for (var x = region.X; x < region.X + region.Width; x++)
                {
                    sumRR00BB += row[x] & 0xff00ff;
                    sum00GG00 += row[x] & 0x00ff00;

                    // Flush before overflow occurs.
                    if (flushCounter++ == 0xff)
                    {
                        totals[0] += sumRR00BB >> 16;
                        totals[1] += sum00GG00 >> 8;
                        totals[2] += sumRR00BB & 0xffff;

                        sumRR00BB = 0;
                        sum00GG00 = 0;

                        flushCounter = 0;
                    }
                }
            }

            source.UnlockBits(bitmapData);

            // Flush left-over's.
            totals[0] += sumRR00BB >> 16;
            totals[1] += sum00GG00 >> 8;
            totals[2] += sumRR00BB & 0xffff;

            // Average the totals.
            totals[0] /= (uint)pixelCount;
            totals[1] /= (uint)pixelCount;
            totals[2] /= (uint)pixelCount;

            return totals;
        }

        /*public Color getDominantColor(System.Drawing.Bitmap bmp)
        {
            BitmapData srcData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            int stride = srcData.Stride;

            IntPtr Scan0 = srcData.Scan0;

            int[] totals = new int[] { 0, 0, 0 };

            int width = bmp.Width;
            int height = bmp.Height;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        for (int color = 0; color < 3; color++)
                        {
                            int idx = (y * stride) + x * 4 + color;
                            totals[color] += p[idx];
                        }
                    }
                }
            }

            int avgB = totals[0] / (width * height);
            int avgG = totals[1] / (width * height);
            int avgR = totals[2] / (width * height);

            bmp.UnlockBits(srcData);

            return Color.FromArgb(avgR, avgG, avgB);
        }*/


        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetDC(IntPtr hWnd);

        /*
         * [DllImport("user32.dll", EntryPoint = "GetDesktopWindow")]
         * static extern IntPtr GetDesktopWindow();
        */

        [DllImport("gdi32.dll")]
        public static extern bool SetDeviceGammaRamp(IntPtr hDC, ref RAMP lpRamp);

        [DllImport("gdi32.dll")]
        public static extern bool GetDeviceGammaRamp(IntPtr hDC, ref RAMP lpRamp);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct RAMP
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public UInt16[] Red;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public UInt16[] Green;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public UInt16[] Blue;
        }

        public static void SetGamma(int gamma)
        {
            if (gamma <= 256 && gamma >= 1)
            {
                RAMP ramp = new RAMP();
                ramp.Red = new ushort[256];
                ramp.Green = new ushort[256];
                ramp.Blue = new ushort[256];
                for (int i = 1; i < 256; i++)
                {
                    int iArrayValue = i * (gamma + 128);

                    if (iArrayValue > 65535)
                        iArrayValue = 65535;
                    ramp.Red[i] = ramp.Blue[i] = ramp.Green[i] = (ushort)iArrayValue;
                }
                SetDeviceGammaRamp(GetDC(IntPtr.Zero), ref ramp);
            }
        }

        public static void SetGammaByRamp(RAMP ramp)
        {
            SetDeviceGammaRamp(GetDC(IntPtr.Zero), ref ramp);
        }

        public static RAMP _GetDeviceGammaRamp()
        {
            RAMP ramp = new RAMP();
            bool success = GetDeviceGammaRamp(GetDC(IntPtr.Zero), ref ramp);
            Console.WriteLine(success);

            return ramp;
        }

        public static double getScreenColor()
        {
            RAMP ramp = new RAMP();
            int overflow = 2;
            bool success = GetDeviceGammaRamp(GetDC(IntPtr.Zero), ref ramp);
            Console.WriteLine(success);

            return (double)ramp.Red[1] - 128;
        }


        public static Bitmap TakeScreenshot(string filePath = null)
        {
            var bounds = Screen.PrimaryScreen.Bounds;
            var bmp = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format32bppArgb);

            using (var g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(0, 0, 0, 0, bounds.Size);

            }


            /*if (filePath != null) bmp.Save(filePath);*/

            return bmp;
        }

        public static Bitmap TakeScreenshot2(string filePath = null)
        {
            var rect = new Rectangle();

            // Take a screenshot that includes all screens by
            // creating a rectangle that captures all monitors
            foreach (var screen in Screen.AllScreens)
            {
                var bounds = screen.Bounds;
                var width = bounds.X + bounds.Width;
                var height = bounds.Y + bounds.Height;

                if (width > rect.Width) rect.Width = width;
                if (height > rect.Height) rect.Height = height;
            }

            var bmp = new Bitmap(rect.Width, rect.Height);

            using (var g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(0, 0, 0, 0, rect.Size);
            }

            if (filePath != null) bmp.Save(filePath);

            return bmp;
        }
    }
}
