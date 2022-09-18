﻿using System;
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

            string ss = string.Join(" ", color.Select(x => x.ToString()).ToArray());
            TextBox1.Text = ss;
            Color target = new Color();
            target = Color.FromArgb((int)color[0], (int)color[1], (int)color[2]);
            Panel1.BackColor = target;
            textBox2.Text = target.GetBrightness().ToString();

            Console.WriteLine("\nDone! Press any key to exit...");
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
