using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        static TimeSpan HIDING_DELAY = TimeSpan.FromSeconds(5);
        static TimeSpan CHANGE_DELAY = TimeSpan.FromMilliseconds(250);

        DisplayConfiguration.PHYSICAL_MONITOR[] physicalMonitors;
        System.Windows.Forms.NotifyIcon notifyIcon;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Taking a screenshot now...");
            Bitmap bmp = libs.TakeScreenshot("screenshot.bmp");
            uint[] color = libs.GetAverageColorInRegion(bmp);
            bmp.Dispose();
            string ss = string.Join(" ", color.Select(x => x.ToString()).ToArray());
            TextBox1.Text = ss;
            Color target = new Color();
            target = Color.FromArgb((int)color[0], (int)color[1], (int)color[2]);
            Panel1.BackColor = target;
            textBox2.Text = target.GetBrightness().ToString();

            /*Color c = libs.getScreenColor();
            Console.WriteLine(c.ToString());*/
            libs.SetGamma(10);
            Console.WriteLine("\nDone! Press any key to exit...");

            physicalMonitors = DisplayConfiguration.GetPhysicalMonitors(DisplayConfiguration.GetCurrentMonitor());
            double qq = DisplayConfiguration.GetMonitorBrightness(physicalMonitors[0]) * 100;
            Console.WriteLine(qq);

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
