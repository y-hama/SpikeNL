using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Environment.Body.Sensor
{
    class Sonar
    {

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
            RelativeDirection = direction;
            AreaWidth = Background.Vision.Image.Width;
            AreaHeight = Background.Vision.Image.Height;
        }

        public void Update(double x, double y, double direction)
        {
            Nx = Math.Cos(Math.PI * (RelativeDirection + direction) / 180);
            Ny = Math.Sin(Math.PI * (RelativeDirection + direction) / 180);
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
                Distance = spp.Min();
            }

            Px = x + Distance * Nx;
            Py = y + Distance * Ny;
        }
    }
}
