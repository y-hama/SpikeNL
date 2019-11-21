﻿using System;
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

        public double WallCollisionRatio { get; set; } = 3;

        public int ViewAngle { get; set; } = 80;
        public int ViewResolition { get; set; } = 10;
        private List<int> ViewAngles { get; set; } = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 8, 10, 13, 16, 20, 25, 30, 40, 50, 65, 85, 110 };

        public double[] ToF_Distance { get; private set; }

        #region Constructor
        public FreeRunner()
        {
            for (int i = ViewAngles.Count - 1; i >= 0; i--)
            {
                Sonares.Add(new Sensor.Sonar(-ViewAngles[i]));
            }
            for (int i = 1; i < ViewAngles.Count; i++)
            {
                Sonares.Add(new Sensor.Sonar(ViewAngles[i]));
            }


            //for (int i = -ViewAngle; i < 0; i += ViewResolition)
            //{
            //    Sonares.Add(new Sensor.Sonar(i));
            //}
            //Sonares.Add(new Sensor.Sonar(0));
            //for (int i = ViewResolition; i <= ViewAngle; i += ViewResolition)
            //{
            //    Sonares.Add(new Sensor.Sonar(i));
            //}
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
                        p = Color.OrangeRed;
                        a = byte.MaxValue;
                    }
                    g.DrawLine(new Pen(Color.FromArgb(a, p), 1), new Point((int)X, (int)Y), new Point((int)item.Px, (int)item.Py));
                    g.FillEllipse(new SolidBrush(Color.FromArgb(a, p)), (int)item.Px - makesize, (int)item.Py - makesize, 2 * makesize, 2 * makesize);
                }
            }

            g.DrawLine(Pens.LightGreen, new Point((int)X, (int)Y), new Point((int)(X + 20 * Vx), (int)(Y + 20 * Vy)));
            g.DrawEllipse(new Pen(Color.Yellow, 2), (int)(X - Size / 2), (int)(Y - Size / 2), (int)Size, (int)Size);
            g.DrawEllipse(new Pen(Color.Yellow, 2), (int)(X - 1), (int)(Y - 1), (int)3, (int)3);
        }

        public override void WheelRotation(ref double left, ref double right)
        {
            #region CollisionCheck
            double nnx = 0, nny = 0, direction = 0, xcnt = 0;
            int nearestid = -1;
            double dist = double.MaxValue;
            ToF_Distance = new double[Sonares.Count];
            int idx = 0;
            foreach (var item in Sonares)
            {
                item.Update(X, Y, Direction);
                ToF_Distance[idx] = (item.Distance);

                idx++;
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

        public override void ViewImage(out Bitmap bitmap)
        {
            bitmap = new Bitmap(ToF_Distance.Length, 1);
            double max = ToF_Distance.Max();
            for (int i = 0; i < ToF_Distance.Length; i++)
            {
                double d = 0;
                if (ToF_Distance[i] < (WallCollisionRatio * Size))
                {
                    d = Math.Pow(Math.Exp(-(ToF_Distance[i] / 2) / (WallCollisionRatio * Size)), 1);
                }
                else
                {
                    d = Math.Pow(Math.Exp(-ToF_Distance[i] / (WallCollisionRatio * Size)), 1);

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
