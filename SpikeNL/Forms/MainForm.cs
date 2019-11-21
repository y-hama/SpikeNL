using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpikeNL.Forms
{
    public partial class MainForm : Form
    {
        private bool isTerminate { get; set; }

        public MainForm()
        {
            InitializeComponent();

            for (int i = 0; i < 10; i++)
            {
                Environment.Core.AddUnit(new Environment.Body.FreeRunner());
            }

            new System.Threading.Tasks.Task(() =>
            {
                while (!isTerminate)
                {
                    DateTime time = DateTime.Now;
                    Environment.Core.Update();
                    double wait = 10 - (DateTime.Now - time).TotalMilliseconds;
                    System.Threading.Thread.Sleep((int)Math.Max(wait, 0));
                }
            }).Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (this.WindowState != FormWindowState.Minimized)
            {
                if (Environment.Core.ShowImage != null)
                {
                    pictureBox1.Image = Environment.Core.ShowImage;
                }

                if (Environment.Core.RepresentView != null)
                {
                    Bitmap resizeBmp = new Bitmap(pictureBox2.Width, pictureBox2.Height);
                    Graphics g = Graphics.FromImage(resizeBmp);
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                    g.DrawImage(Environment.Core.RepresentView, 0, 0, pictureBox2.Width, pictureBox2.Height);
                    g.Dispose();
                    pictureBox2.Image = resizeBmp;
                }
            }
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            //double acc = 0.25;
            //switch (e.KeyData)
            //{
            //    case Keys.Up:
            //        tracker[0].SetOptionalWheel(0.25, acc);
            //        break;
            //    case Keys.Down:
            //        tracker[0].SetOptionalWheel(-acc, -acc);
            //        break;
            //    case Keys.Left:
            //        tracker[0].SetOptionalWheel(-acc, acc);
            //        break;
            //    case Keys.Right:
            //        tracker[0].SetOptionalWheel(acc, -acc);
            //        break;
            //    case Keys.Space:
            //        tracker[0].SetOptionalWheel(0, 0);
            //        break;
            //    default:
            //        break;
            //}
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            isTerminate = true;
        }
    }
}
