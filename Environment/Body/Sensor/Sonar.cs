using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Environment.Body.Sensor
{
    class Sonar
    {
        private static int _seed { get; set; } = 0;
        public int ID { get; private set; }

        public double RelativeDirection { get; private set; }

        private double AreaWidth { get; set; }
        private double AreaHeight { get; set; }

        public double Distance { get; private set; }
        public double Nx { get; private set; }
        public double Ny { get; private set; }

        public double Px { get; private set; }
        public double Py { get; private set; }

        public Sonar(double direction)
        {
            ID = _seed++;
            RelativeDirection = direction;
            AreaWidth = Background.Vision.Image.Width;
            AreaHeight = Background.Vision.Image.Height;
        }

        public void Update(double x, double y, double direction)
        {
            Nx = Math.Cos(Math.PI * (RelativeDirection + direction) / 180);
            Ny = Math.Sin(Math.PI * (RelativeDirection + direction) / 180);

            double rectBorder = RectangleBorder(x, y, Nx, Ny);
            double wallBorder = WallBorder(x, y, Nx, Ny, rectBorder);
            Distance = wallBorder;

            if (BodyList.UnitList.Count > 1)
            {
                Distance = Math.Min(Distance, UnitBorder(x, y, Nx, Ny, Distance));
            }
            Px = x + Distance * Nx;
            Py = y + Distance * Ny;
        }

        private double RectangleBorder(double x, double y, double nx, double ny)
        {
            double ret = 0;
            double s1, s2, s3, s4;
            s1 = (0 - x) / Nx;
            s2 = (AreaWidth - x) / Nx;
            s3 = (0 - y) / Ny;
            s4 = (AreaHeight - y) / Ny;
            List<double> spp = new List<double>();
            if (s1 >= 0) { spp.Add(s1); }
            if (s2 >= 0) { spp.Add(s2); }
            if (s3 >= 0) { spp.Add(s3); }
            if (s4 >= 0) { spp.Add(s4); }
            if (spp.Count > 0)
            {
                ret = spp.Min();
            }
            return ret;
        }

        private double WallBorder(double px, double py, double nx, double ny, double maxdist)
        {
            double ret = 0;
            while (ret < maxdist)
            {
                if (Background.Vision.IsWall((int)(px + ret * nx), (int)(py + ret * ny)))
                {
                    break;
                }
                ret++;
            }
            return ret;
        }

        private double UnitBorder(double x, double y, double nx, double ny, double maxdist)
        {
            double ret = double.MaxValue;
            double ppx = 0, ppy = 0;
            double mb = 0;
            bool col = false;
            while (mb < maxdist)
            {
                ppx = x + mb * nx;
                ppy = y + mb * ny;
                double distmin = double.MaxValue;
                foreach (var item in BodyList.UnitList)
                {
                    if (item.X == x && item.Y == y) { continue; }
                    double dist = Math.Sqrt((ppx - item.X) * (ppx - item.X) + (ppy - item.Y) * (ppy - item.Y));
                    if (dist < BaseBody.Size / 2 && dist < distmin) { ret = mb; distmin = dist; col = true; }
                }
                if (col)
                { break; }
                else
                {
                    mb += 1.0;
                }
            }
            return ret;
        }
    }
}
