using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Environment.Body
{
    static class BodyList
    {
        public static List<Body.BaseBody> UnitList { get; set; } = new List<Body.BaseBody>();
        public static List<Body.BaseBody> DeadUnitList { get; set; } = new List<Body.BaseBody>();
    }

    public abstract class BaseBody
    {
        protected static Random random { get; set; } = new Random();

        public Parameter Parameter { get; protected set; } = new Parameter();

        #region Property 
        public bool IsDead { get { return Parameter.IsDead; } protected set { Parameter.IsDead = value; } }
        public double AliveTime { get { return Parameter.AliveTime; } protected set { Parameter.AliveTime = value; } }
        public double QualityAliveTime { get { return Parameter.QualityAliveTime; } protected set { Parameter.QualityAliveTime = value; } }
        public double AliveRatio { get { return Parameter.AliveRatio; } }

        public Color Color { get { return Parameter.Color; } protected set { Parameter.Color = value; } }

        public double X { get { return Parameter.X; } protected set { Parameter.X = value; } }
        public double Y { get { return Parameter.Y; } protected set { Parameter.Y = value; } }
        public double Angle { get { return Parameter.Angle; } }
        public double Direction { get { return Parameter.Direction; } protected set { Parameter.Direction = value; } }

        public double Vx { get { return Parameter.Vx; } protected set { Parameter.Vx = value; } }
        public double Vy { get { return Parameter.Vy; } protected set { Parameter.Vy = value; } }
        public double V { get { return Parameter.V; } }

        public double Size { get { return Parameter.Size; } protected set { Parameter.Size = value; } }
        #endregion

        public BaseBody()
        {
            UninterferenceLocation();
        }

        private Queue<System.Drawing.PointF> Trajectory { get; set; } = new Queue<System.Drawing.PointF>();

        public void Kill() { IsDead = true; }

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
        public abstract void Growup(bool isUpdate = true);

        public abstract BaseBody InheritanceNew(Parameter parameter, bool isMutation = false);

        protected void UninterferenceLocation()
        {
            Growup(false);
            while (Parameter.IsDead)
            {
                Parameter.IsDead = false;
                Parameter.X = (Background.Vision.Image.Width) * random.NextDouble();
                Parameter.Y = (Background.Vision.Image.Height) * random.NextDouble();
                if (Background.Vision.IsWall((int)Parameter.X, (int)Parameter.Y))
                {
                    continue;
                }
                Growup(false);
            }
        }

        public virtual void ViewImage(out System.Drawing.Bitmap bitmap)
        {
            bitmap = new System.Drawing.Bitmap(1, 1);
        }
    }
}
