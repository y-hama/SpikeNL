using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Environment.Body
{
    static class BodyList
    {
        public static List<Body.BaseBody> UnitList { get; set; } = new List<Body.BaseBody>();
    }

    public abstract class BaseBody
    {
        public double X { get; protected set; } = (Background.Vision.Image.Width / 2) * random.NextDouble() + Background.Vision.Image.Width / 4;
        public double Y { get; protected set; } = (Background.Vision.Image.Height / 2) * random.NextDouble() + Background.Vision.Image.Height / 4;
        public double Direction { get; set; } = random.NextDouble() * 360;
        public double Angle
        {
            get { return Math.PI * Direction / 180; }
        }

        public static double Size { get; set; } = 20;

        public double Vx { get; protected set; }
        public double Vy { get; protected set; }
        public double V { get { return Math.Sqrt(Vx * Vx + Vy * Vy); } }

        public double W { get; protected set; }

        public bool IsDead { get; set; }

        private Queue<System.Drawing.PointF> Trajectory { get; set; } = new Queue<System.Drawing.PointF>();

        protected static Random random { get; set; } = new Random();

        public void Draw(ref System.Drawing.Bitmap bitmap, ref System.Drawing.Graphics g)
        {
            var trj = Trajectory.ToArray();
            for (int i = 1; i < trj.Length; i++)
            {
                byte b = (byte)(byte.MaxValue * (((double)i / trj.Length)));
                g.DrawLine(new System.Drawing.Pen(System.Drawing.Color.FromArgb(b, b, b), 1),
                    new System.Drawing.PointF(trj[i - 1].X, trj[i - 1].Y),
                    new System.Drawing.PointF(trj[i].X, trj[i].Y));
            }
            Trajectory.Enqueue(new System.Drawing.PointF((float)X, (float)Y));
            if (Trajectory.Count > 100) { Trajectory.Dequeue(); }
            DrawIcon(ref bitmap, ref g);
        }
        public abstract void DrawIcon(ref System.Drawing.Bitmap bitmap, ref System.Drawing.Graphics g);
        public abstract void Update();
        public abstract void Growup();

        public virtual void ViewImage(out System.Drawing.Bitmap bitmap)
        {
            bitmap = new System.Drawing.Bitmap(1, 1);
        }
    }
}
