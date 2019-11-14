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
        #region SensorClass
        private List<Sensor.Sonar> Sonares { get; set; } = new List<Sensor.Sonar>();
        #endregion

        public double WallCollisionRatio { get; set; } = 2;

        #region Constructor
        public FreeRunner()
        {
            int area = 60;
            for (int i = -area; i <= area; i += 10)
            {
                Sonares.Add(new Sensor.Sonar(i));
            }
        }
        #endregion

        public override void DrawIcon(ref Bitmap bitmap, ref Graphics g)
        {
            g.DrawLine(new Pen(Color.SteelBlue, 5), new Point((int)X, (int)Y), new Point((int)(X + Size * Math.Cos(Angle) + 1), (int)(Y + Size * Math.Sin(Angle) + 1)));
            g.DrawLine(Pens.Cyan, new Point((int)X, (int)Y), new Point((int)(X + Size * Math.Cos(Angle)), (int)(Y + Size * Math.Sin(Angle))));
            int makesize = 4;
            foreach (var item in Sonares)
            {
                Color p = Color.Gray;
                byte a = 25;
                if (item.Px >= -10 && item.Px < Background.Vision.Image.Width + 10 &&
                    item.Py >= -10 && item.Py < Background.Vision.Image.Height + 10)
                {
                    if (Math.Abs(item.Distance) < WallCollisionRatio * Size)
                    {
                        p = Color.Red;
                        a = byte.MaxValue;
                    }
                    g.DrawLine(new Pen(Color.FromArgb(a, p), 1), new Point((int)X, (int)Y), new Point((int)item.Px, (int)item.Py));
                    g.FillEllipse(new SolidBrush(Color.FromArgb(a, p)), (int)item.Px - makesize, (int)item.Py - makesize, 2 * makesize, 2 * makesize);
                }
            }

            g.DrawEllipse(new Pen(Color.Yellow, 2), (int)(X - Size / 2), (int)(Y - Size / 2), (int)Size, (int)Size);
            g.DrawEllipse(new Pen(Color.Yellow, 2), (int)(X - 1), (int)(Y - 1), (int)3, (int)3);
        }

        public override void WheelRotation(ref double left, ref double right)
        {
            #region CollisionCheck
            double nnx = 0, nny = 0, direction = 0, xcnt = 0;
            int nearestid = -1;
            double dist = double.MaxValue;
            foreach (var item in Sonares)
            {
                item.Update(X, Y, Direction);
                if (item.Distance < dist)
                {
                    nearestid = item.ID;
                    dist = item.Distance;
                }
            }
            if (dist < BaseBody.Size / 2)
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
                        if (frontsens != null && frontsens.Distance < BaseBody.Size * 3)
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
    }
}
