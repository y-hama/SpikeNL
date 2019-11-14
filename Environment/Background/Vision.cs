using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Environment.Background
{
    public enum ModeType
    {
        NoSignal,
        Image,
        Camera,
    }

    static class Vision
    {
        private static Random random { get; set; } = new Random();

        public static ModeType Mode { get; set; }


        public static Bitmap Image { get; set; }

        public static double GetAroundBrightness(int x, int y, int size)
        {
            double b = 0, cnt = 0;
            for (int i = -size; i <= size; i++)
            {
                for (int j = -size; j <= size; j++)
                {
                    if (x + i >= 0 && x + i < Image.Width && y + j >= 0 && y + j < Image.Height)
                    {
                        Color c = Image.GetPixel(x + i, y + j);
                        b += (c.R + c.G + c.B) / 3;
                        cnt++;
                    }
                }
            }
            if (cnt != 0)
            {
                b /= cnt;
            }
            b /= byte.MaxValue;
            var r = (random.NextDouble() * 2 - 1);
            b += 0.001 * r;
            return Math.Max(0, Math.Min(b, 1));
        }

        public static bool IsWall(int x, int y)
        {
            byte b = Image.GetPixel(x, y).B;
            if (b > 100) { return true; }
            else { return false; }

        }
    }
}
