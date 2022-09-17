using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    internal class libs
    {
        public static Bitmap TakeScreenshot(string filePath = null)
        {
            var bounds = Screen.PrimaryScreen.Bounds;
            var bmp = new Bitmap(bounds.Width, bounds.Height);

            using (var g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(0, 0, 0, 0, bounds.Size);
            }

            if (filePath != null) bmp.Save(filePath);

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
