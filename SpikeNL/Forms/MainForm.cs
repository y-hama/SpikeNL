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
        private double ProcessTime { get; set; }
        private double MeanProcessTime { get; set; }
        private double Alive { get; set; }
        private double AliveRatio { get; set; }

        public MainForm()
        {
            InitializeComponent();

            for (int i = 0; i < 5; i++)
            {
                Environment.Core.AddUnit(new Environment.Body.GeneticRunner());
            }

            new System.Threading.Tasks.Task(() =>
            {
                double max = 0, n = 0, rho = 0.1;
                MeanProcessTime = 10;
                while (!isTerminate)
                {
                    DateTime time = DateTime.Now;
                    Environment.Core.Update();
                    ProcessTime = (DateTime.Now - time).TotalMilliseconds;
                    double wait = MeanProcessTime - ProcessTime;
                    System.Threading.Thread.Sleep((int)Math.Max(wait, 0));
                    //max = Math.Max(max, ProcessTime);
                    //MeanProcessTime = (((n / (n + 1)) * MeanProcessTime + (1 / (n + 1)) * ProcessTime) + max) / 2;
                    //max = (rho) * MeanProcessTime + (1 - rho) * max;
                    n++;

                    if (Environment.Core.LatestParameter != null)
                    {
                        Alive = Environment.Core.LatestParameter.AliveTime;
                        AliveRatio = Environment.Core.LatestParameter.QualityAliveTime;
                    }
                }
            }).Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label1.Text = Environment.Core.Generation.ToString() + " - " +
                ProcessTime.ToString() + " - " +
                MeanProcessTime.ToString() + " - " +
                Alive.ToString() + " - " +
                AliveRatio.ToString();
            if (this.WindowState != FormWindowState.Minimized)
            {
                if (Environment.Core.ShowImage != null)
                {
                    pictureBox1.Image = Environment.Core.ShowImage;
                }

                if (Environment.Core.HasRepresentView)
                {
                    pictureBox2.Image = Environment.Core.RepresentView(pictureBox1.Size);
                }
            }
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            isTerminate = true;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (Environment.Core.LatestParameter != null)
            {
                Environment.Core.LatestParameter.Gene.Save("sample.obj");
            }
        }
    }
}
