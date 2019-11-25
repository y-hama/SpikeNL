using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Environment.Body
{
    public class FreeRunner : WheelCar
    {
        #region Property
        public List<int> ViewAngles { get { return Parameter.ViewAngles; } protected set { Parameter.ViewAngles = value; } }
        public List<Sensor.Sonar> Sonares { get { return Parameter.Sonares; } protected set { Parameter.Sonares = value; } }
        public List<double> ToF_Distance { get { return Parameter.ToF_Distance; } protected set { Parameter.ToF_Distance = value; } }
        public double WallCollisionRatio { get { return Parameter.WallCollisionRatio; } protected set { Parameter.WallCollisionRatio = value; } }
        public double[] SonarDistanceLimit { get { return Parameter.Gene.SonarDistanceLimit; } protected set { Parameter.Gene.SonarDistanceLimit = value; } }
        #endregion

        protected virtual void SetColor()
        {
            Parameter.Color = Color.Yellow;
        }

        #region Constructor
        public FreeRunner()
        {
            SetColor();

            for (int i = ViewAngles.Count - 1; i >= 0; i--)
            {
                Sonares.Add(new Sensor.Sonar(-ViewAngles[i]));
            }
            for (int i = 1; i < ViewAngles.Count; i++)
            {
                Sonares.Add(new Sensor.Sonar(ViewAngles[i]));
            }

            InitializeParameter();
        }
        protected virtual void InitializeParameter() { }
        #endregion

        public override void DrawIcon(ref Bitmap bitmap, ref Graphics g)
        {
            g.DrawLine(new Pen(Color.SteelBlue, 5), new Point((int)X, (int)Y), new Point((int)(X + Size * Math.Cos(Angle) + 1), (int)(Y + Size * Math.Sin(Angle) + 1)));
            g.DrawLine(Pens.Cyan, new Point((int)X, (int)Y), new Point((int)(X + Size * Math.Cos(Angle)), (int)(Y + Size * Math.Sin(Angle))));
            int makesize = 4;
            for (int i = 0; i < Sonares.Count; i++)
            {
                var item = Sonares[i];
                Color p = Color.Gray;
                byte a = 25;
                if (item.Px >= -10 && item.Px < Background.Vision.Image.Width + 10 &&
                    item.Py >= -10 && item.Py < Background.Vision.Image.Height + 10)
                {
                    if (Math.Abs(item.Distance) < WallCollisionRatio * SonarDistanceLimit[i])
                    {
                        p = Color.OrangeRed;
                        a = byte.MaxValue;
                    }
                    g.DrawLine(new Pen(Color.FromArgb(a, p), 1), new Point((int)X, (int)Y), new Point((int)item.Px, (int)item.Py));
                    g.FillEllipse(new SolidBrush(Color.FromArgb(a, p)), (int)item.Px - makesize, (int)item.Py - makesize, 2 * makesize, 2 * makesize);
                }
            }

            g.DrawLine(Pens.LightGreen, new Point((int)X, (int)Y), new Point((int)(X + 20 * Vx), (int)(Y + 20 * Vy)));
            g.DrawEllipse(new Pen(Color, 2), (int)(X - Size / 2), (int)(Y - Size / 2), (int)Size, (int)Size);
            g.DrawEllipse(new Pen(Color, 2), (int)(X - 1), (int)(Y - 1), (int)3, (int)3);
        }

        protected double UpdateSonar()
        {
            double dist = double.MaxValue;
            ToF_Distance = new List<double>();
            int idx = 0;
            foreach (var item in Sonares)
            {
                item.Update(X, Y, Direction);
                ToF_Distance.Add(item.Distance);

                idx++;
                if (item.Distance < dist)
                {
                    dist = item.Distance;
                }
            }
            return dist;
        }

        public override void WheelRotation(ref double left, ref double right)
        {
            #region CollisionCheck
            double nnx = 0, nny = 0, direction = 0, xcnt = 0;
            double dist = UpdateSonar();
            if (dist < Size / 2)
            {
                this.IsDead = true;
            }
            else
            {
                double ratio = 1;
                foreach (var item in Sonares)
                {
                    if (item.Distance < WallCollisionRatio * Size)
                    {
                        nnx += ratio * item.Nx; nny += ratio * item.Ny; direction += ratio * item.RelativeDirection;
                        xcnt++;
                    }
                }
                if (xcnt > 0)
                {
                    nnx /= xcnt; nny /= xcnt; direction /= xcnt;
                    double dir = Math.Atan2(nny, nnx) * 180 / Math.PI;
                    if (direction > 360) { direction -= 360; }
                    else if (direction < 0) { direction += 360; }

                    double gratio = 1.5;
                    if (direction < 10 || direction > 350)
                    {
                        var frontsens = (Sonares.Find(x => x.RelativeDirection == 0));
                        if (frontsens != null && frontsens.Distance < Size * 3)
                        {
                            left *= -random.NextDouble(); right *= -random.NextDouble();
                        }
                    }
                    else
                    {
                        if (direction < 90)
                        {
                            {
                                ratio = (90 - (direction - 45)) / 90;
                                left /= -gratio;
                                right *= gratio;
                            }
                        }
                        else if (direction > 270)
                        {
                            {
                                ratio = ((direction - 45) - 270) / 90;
                                left *= gratio;
                                right /= -gratio;
                            }
                        }
                    }
                }
            }
            #endregion

        }

        public override BaseBody InheritanceNew(Parameter parameter, bool isMutation = false)
        {
            return new FreeRunner();
        }
        public override void ViewImage(out Bitmap bitmap)
        {
            bitmap = new Bitmap(ToF_Distance.Count, 1);
            double max = ToF_Distance.Max();
            for (int i = 0; i < ToF_Distance.Count; i++)
            {
                double d = 0;
                if (ToF_Distance[i] < (WallCollisionRatio * SonarDistanceLimit[i]))
                {
                    d = Math.Pow(Math.Exp(-(ToF_Distance[i] / 2) / (WallCollisionRatio * SonarDistanceLimit[i])), 1);
                }
                else
                {
                    d = Math.Pow(Math.Exp(-ToF_Distance[i] / (WallCollisionRatio * SonarDistanceLimit[i])), 1);

                }
                double dr, dg, db;
                double shift = Math.PI + Math.PI / 4;
                dr = byte.MaxValue * (Math.Sin(1.5 * Math.PI * d + shift) + 1) / 2;
                dg = byte.MaxValue * (Math.Sin(1.5 * Math.PI * d + shift + Math.PI / 2) + 1) / 2;
                db = byte.MaxValue * (Math.Sin(1.5 * Math.PI * d + shift + Math.PI) + 1) / 2;

                bitmap.SetPixel(i, 0, Color.FromArgb((byte)dr, (byte)dg, (byte)db));
            }
        }

    }
}
