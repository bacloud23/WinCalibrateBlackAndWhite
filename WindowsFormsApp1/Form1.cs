using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
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


        private void OnTimedEvent(object state, System.Timers.ElapsedEventArgs e)
        {
            // do some work here
            Thread.Sleep(500);

            var reportProgress = new Action(() =>
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
                float brightnessFromScreenshot = target.GetBrightness();
                TextBox2.Text = brightnessFromScreenshot.ToString();

                double brightnessFromScreen = libs.getScreenColor();
                Console.WriteLine(brightnessFromScreen.ToString());
                /*gamma <= 256 && gamma >= 1*/
                int factor = 256 - ((int)(brightnessFromScreenshot * 256));
                /*Console.WriteLine("brightnessFromScreenshot");
                Console.WriteLine(brightnessFromScreenshot);
                Console.WriteLine(factor);*/
                libs.SetGamma(factor);
                Console.WriteLine("\nDone! Press any key to exit...");

                physicalMonitors = DisplayConfiguration.GetPhysicalMonitors(DisplayConfiguration.GetCurrentMonitor());
                double brightnessFromMonitor = DisplayConfiguration.GetMonitorBrightness(physicalMonitors[0]) * 100;
                TextBox3.Text = brightnessFromMonitor.ToString();

                Console.WriteLine(string.Format("State refreshed. {0}\n", DateTime.Now));
            });
            Invoke(reportProgress);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 500;
            aTimer.Enabled = true;

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
