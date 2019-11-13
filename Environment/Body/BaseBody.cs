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
        public double X { get; protected set; } = Background.Vision.Image.Width / 2;
        public double Y { get; protected set; } = Background.Vision.Image.Height / 2;
        public double Direction { get; set; } = 360 * random.NextDouble();
        public double Angle
        {
            get { return Math.PI * Direction / 180; }
        }

        public double Size { get; set; } = 15;

        protected static Random random { get; set; } = new Random();
        public abstract void DrawIcon(ref System.Drawing.Bitmap bitmap, ref System.Drawing.Graphics g);
        public abstract void Update();
        public abstract void Growup();

    }
}
