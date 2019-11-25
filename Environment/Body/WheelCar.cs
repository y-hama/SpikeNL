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

        #region Property 
        public double WMax { get { return Parameter.Gene.WMax; } protected set { Parameter.Gene.WMax = value; } }
        public double wl { get { return Parameter.wl; } protected set { Parameter.wl = value; } }
        public double wr { get { return Parameter.wr; } protected set { Parameter.wr = value; } }
        public double Inertia { get { return Parameter.Inertia; } protected set { Parameter.Inertia = value; } }
        public double Error { get { return Parameter.Error; } protected set { Parameter.Error = value; } }
        public double WheelRadius { get { return Parameter.WheelRadius; } protected set { Parameter.WheelRadius = value; } }

        #endregion

        public void SetWheel(double l, double r)
        {
            wl = Inertia * wl + (1 - Inertia) * l;
            wr = Inertia * wr + (1 - Inertia) * r;
        }

        public override void Growup(bool isUpdate = true)
        {
            double l, r;
            l = r = 1;
            WheelRotation(ref l, ref r);
            if (isUpdate)
            {
                l += Error * (random.NextDouble() * 2 - 1);
                r += Error * (random.NextDouble() * 2 - 1);
                double n = Math.Sqrt(l * l + r * r);
                if (n > WMax)
                {
                    l /= n; r /= n;
                }
                SetWheel(l, r);
            }
        }

        public abstract void WheelRotation(ref double left, ref double right);

        public override void Update()
        {
            double cX = X, cY = Y;
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
            Vx = (X - cX); Vy = (Y - cY);
            AliveTime++;
            QualityAliveTime += V;
        }
    }
}
