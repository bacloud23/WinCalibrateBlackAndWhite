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
    }
}
