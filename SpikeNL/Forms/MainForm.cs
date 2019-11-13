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

        public MainForm()
        {
            InitializeComponent();

            for (int i = 0; i < 100; i++)
            {
                Environment.Core.AddUnit(new Environment.Body.FreeRunner());
            }

            new System.Threading.Tasks.Task(() =>
            {
                while (true)
                {
                    Environment.Core.Update();
                    System.Threading.Thread.Sleep(0);
                }
            }).Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            pictureBox1.Image = Environment.Core.ShowImage;
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
    }
}
