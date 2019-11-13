using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Environment.Body
{
    public class Tracker : BaseBody
    {
        #region InnerClass
        private class WallSensor
        {
            public double S { get; private set; }
            public double Sp { get; private set; }
            public double Sm { get; private set; }

            public double Ppx { get; private set; }
            public double Ppy { get; private set; }
            public double Pmx { get; private set; }
            public double Pmy { get; private set; }

            public double Nx { get; private set; }
            public double Ny { get; private set; }
            public double Angle { get; private set; }
            public double WallDirection { get; private set; }

            public WallSensor(Tracker t, double angle)
            {
                double w = Background.Vision.Image.Width, h = Background.Vision.Image.Height;
                Angle = angle;
                Nx = Math.Cos(t.Direction + Math.PI * angle / 180);
                Ny = Math.Sin(t.Direction + Math.PI * angle / 180);
                double cx = t.X, cy = t.Y;
                double s1, s2, s3, s4;
                s1 = (0 - cx) / Nx;
                s2 = (w - cx) / Nx;
                s3 = (0 - cy) / Ny;
                s4 = (h - cy) / Ny;
                List<double> spp = new List<double>(), smm = new List<double>();
                if (s1 >= 0) { spp.Add(s1); } else { smm.Add(s1); }
                if (s2 >= 0) { spp.Add(s2); } else { smm.Add(s2); }
                if (s3 >= 0) { spp.Add(s3); } else { smm.Add(s3); }
                if (s4 >= 0) { spp.Add(s4); } else { smm.Add(s4); }
                Sp = double.MaxValue; Sm = double.MinValue;
                foreach (var item in spp)
                {
                    if (item < Sp) { Sp = item; }
                }
                foreach (var item in smm)
                {
                    if (item > Sm) { Sm = item; }
                }
                if (Math.Abs(Sp) <= Math.Abs(Sm))
                {
                    S = Math.Abs(Sp);
                    WallDirection = Angle;
                }
                else
                {
                    S = Math.Abs(Sm);
                    WallDirection = Angle + 180;
                }
                if (angle > 360) { angle -= 360; }

                Ppx = cx + Sp * Nx;
                Ppy = cy + Sp * Ny;
                Pmx = cx + Sm * Nx;
                Pmy = cy + Sm * Ny;
            }
        }
        #endregion
        private List<WallSensor> WallDetection { get; set; } = new List<WallSensor>();

        public double WheelRadius { get; set; } = 0.5;

        public int ViewSensorSize { get; set; } = 2;

        public int WallDetectionResolution { get; set; } = 8;
        public double WallCollisionRatio { get; set; } = 4.5;

        private double wr { get; set; }
        private double wl { get; set; }

        private double option_wr { get; set; }
        private double option_wl { get; set; }

        public Tracker()
        {
        }

        public override void DrawIcon(ref Bitmap bitmap, ref Graphics g)
        {
            g.DrawLine(new Pen(Color.SteelBlue, 5), new Point((int)X, (int)Y), new Point((int)(X + Size * Math.Cos(Direction) + 1), (int)(Y + Size * Math.Sin(Direction) + 1)));
            g.DrawLine(Pens.Cyan, new Point((int)X, (int)Y), new Point((int)(X + Size * Math.Cos(Direction)), (int)(Y + Size * Math.Sin(Direction))));

            int makesize = 4;
            foreach (var item in WallDetection)
            {
                Color p = Color.Gray;
                byte a = 100;
                if (item.Ppx >= -10 && item.Ppx < Background.Vision.Image.Width + 10 &&
                    item.Ppy >= -10 && item.Ppy < Background.Vision.Image.Height + 10)
                {
                    if (Math.Abs(item.Sp) < WallCollisionRatio * Size)
                    {
                        p = Color.Red;
                        a = 200;
                    }
                    g.DrawLine(new Pen(Color.FromArgb(a, p), 1), new Point((int)X, (int)Y), new Point((int)item.Ppx, (int)item.Ppy));
                    g.FillEllipse(new SolidBrush(Color.FromArgb(a, p)), (int)item.Ppx - makesize, (int)item.Ppy - makesize, 2 * makesize, 2 * makesize);
                }

                p = Color.Gray;
                a = 100;
                if (item.Pmx >= -10 && item.Pmx < Background.Vision.Image.Width + 10 &&
                    item.Pmy >= -10 && item.Pmy < Background.Vision.Image.Height + 10)
                {
                    if (Math.Abs(item.Sm) < WallCollisionRatio * Size)
                    {
                        p = Color.Red;
                        a = 200;
                    }
                    g.DrawLine(new Pen(Color.FromArgb(a, p), 1), new Point((int)X, (int)Y), new Point((int)item.Pmx, (int)item.Pmy));
                    g.FillEllipse(new SolidBrush(Color.FromArgb(a, p)), (int)item.Pmx - makesize, (int)item.Pmy - makesize, 2 * makesize, 2 * makesize);
                }

            }

            g.DrawEllipse(new Pen(Color.Red, 2), (int)(X - Size / 2), (int)(Y - Size / 2), (int)Size, (int)Size);
            g.DrawEllipse(new Pen(Color.Red, 2), (int)(X - 1), (int)(Y - 1), (int)3, (int)3);
        }

        public void SetWheel(double l, double r)
        {
            double error = 0.025;
            double rho = 0.75;
            wl = rho * wl + (1 - rho) * l + error * (random.NextDouble() * 2 - 1);
            wr = rho * wr + (1 - rho) * r + error * (random.NextDouble() * 2 - 1);
        }

        public void SetOptionalWheel(double l, double r)
        {
            option_wl = l;
            option_wr = r;
        }

        public override void Growup()
        {
            WallDetection.Clear();
            int step = 360 / WallDetectionResolution;
            for (int i = 0; i < 180; i += step)
            {
                WallDetection.Add(new Body.Tracker.WallSensor(this, (double)i));
            }

            double init = 0.1;
            double l, r;
            l = init; r = init;

            if (option_wl != 0 || option_wr != 0)
            {
                l = option_wl; r = option_wr;
            }
            else
            {

                var brightness = Background.Vision.GetAroundBrightness((int)X, (int)Y, ViewSensorSize);
                if (brightness < 0.1)
                {

                }
                else if (brightness > 0.6)
                {
                    l *= -1; r *= -1;
                }
            }

            #region WallFinder
            double wnx = 0, wny = 0, angle = 0, cnt = 0;
            foreach (var item in WallDetection)
            {
                if (item.S < WallCollisionRatio * Size)
                {
                    cnt++;
                    wnx += item.Nx;
                    wny += item.Ny;
                    angle += item.WallDirection;
                }
            }
            if (cnt > 0)
            {
                wnx /= cnt; wny /= cnt; angle /= cnt;
                if (angle > 180)
                {
                    l *= 1.5;
                    r /= 1.5;
                }
                else
                {
                    l /= 1.5;
                    r *= 1.5;
                }
            }
            #endregion
            SetWheel(l, r);
        }

        public override void Update()
        {
            if (wr == wl)
            {
                double v = (wr + wl) / 2;
                X += WheelRadius * v * Math.Cos(Direction);
                Y += WheelRadius * v * Math.Sin(Direction);
            }
            else
            {
                double wa, wb, ol, cx, cy, th, rh = 1;
                double nx, ny;
                if (wl > wr)
                {
                    rh = 1;
                    wa = wl; wb = wr;
                }
                else
                {
                    rh = -1;
                    wa = wr; wb = wl;
                }

                nx = rh * (Math.Sin(Direction));
                ny = rh * (-Math.Cos(Direction));

                th = rh * WheelRadius * (wa - wb) / Size;
                ol = ((wa + wb) / (wa - wb)) * (Size / 2);
                cx = X + nx * ol;
                cy = Y + ny * ol;

                double c, s;
                c = Math.Cos(th); s = Math.Sin(th);
                X = c * X + s * Y + (+(1 - c) * cx - s * cy);
                Y = -s * X + c * Y + (+s * cx + (1 - c) * cy);

                Direction += th;

                if (Direction < 0)
                {
                    Direction += 2 * Math.PI;
                }
                else if (Direction > 2 * Math.PI)
                {
                    Direction -= 2 * Math.PI;
                }

            }
        }
    }
}
