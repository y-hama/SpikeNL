using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Environment.Body
{
    public abstract class WheelCar : BaseBody
    {

        #region Property/Field
        public double WheelRadius { get; set; } = 2.5;

        public double Error { get; set; } = 0.025;
        public double Inertia { get; set; } = 0.85;

        private double wr { get; set; }
        private double wl { get; set; }
        #endregion

        public void SetWheel(double l, double r)
        {
            wl = Inertia * wl + (1 - Inertia) * l + Error * (random.NextDouble() * 2 - 1);
            wr = Inertia * wr + (1 - Inertia) * r + Error * (random.NextDouble() * 2 - 1);
        }

        public override void Growup()
        {
            double l, r;
            l = r = 1;
            WheelRotation(ref l, ref r);
            double n = Math.Sqrt(l * l + r * r);
            if (n > 1)
            {
                l /= n; r /= n;
            }
            SetWheel(l, r);
        }

        public abstract void WheelRotation(ref double left, ref double right);

        public override void Update()
        {
            if (wr == wl)
            {
                double v = (wr + wl) / 2;
                X += WheelRadius * v * Math.Cos(Angle);
                Y += WheelRadius * v * Math.Sin(Angle);
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

                nx = rh * (Math.Sin(Angle));
                ny = rh * (-Math.Cos(Angle));

                th = rh * WheelRadius * (wa - wb) / Size;
                ol = ((wa + wb) / (wa - wb)) * (Size / 2);
                cx = X + nx * ol;
                cy = Y + ny * ol;

                double c, s;
                c = Math.Cos(th); s = Math.Sin(th);
                X = c * X + s * Y + (+(1 - c) * cx - s * cy);
                Y = -s * X + c * Y + (+s * cx + (1 - c) * cy);

                Direction += th * 180 / Math.PI;

                if (Direction < 0)
                {
                    Direction += 360;
                }
                else if (Direction > 360)
                {
                    Direction -= 360;
                }

            }
        }
    }
}
