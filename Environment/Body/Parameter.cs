using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;

namespace Environment.Body
{
    public class Parameter
    {
        [System.Xml.Serialization.XmlIgnore]
        protected static Random random { get; set; } = new Random();

        [System.Xml.Serialization.XmlIgnore]
        public Color Color { get; set; } = Color.Yellow;

        #region BaseBody
        public bool IsDead { get; set; }
        public double X { get; set; } = (Background.Vision.Image.Width) * random.NextDouble();
        public double Y { get; set; } = (Background.Vision.Image.Height) * random.NextDouble();
        public double Direction { get; set; } = random.NextDouble() * 360;
        public double Angle
        {
            get { return Math.PI * Direction / 180; }
        }

        public double Size { get; set; } = 20;

        public double Vx { get; set; }
        public double Vy { get; set; }
        public double V { get { return Math.Sqrt(Vx * Vx + Vy * Vy); } }
        #endregion

        #region WheelCar
        public double WheelRadius { get; set; } = 2;

        public double Error { get; set; } = 0.1;
        public double Inertia { get; set; } = 0.85;

        public double wr { get; set; }
        public double wl { get; set; }
        #endregion

        #region Evaluation
        public double AliveTime { get; set; } = 0;
        public double QualityAliveTime { get; set; } = 0;
        public double AliveRatio { get { return AliveTime == 0 ? 0 : (QualityAliveTime / AliveTime); } }
        #endregion

        #region Generation
        public class GeneSet
        {
            private double MutationProbability { get; set; } = 0.1;

            public double WMax { get; set; } = 1;

            public double[] SonarAngleResponse { get; set; }
            public double[] SonarDistanceLimit { get; set; }

            public void Mutation(GeneSet source)
            {
                if (random.NextDouble() < MutationProbability)
                {
                    WMax += 0.25 * (random.NextDouble() * 2 - 1);
                    WMax = Math.Min(Math.Max(WMax, 0), 1);
                }
                for (int i = 0; i < SonarAngleResponse.Length; i++)
                {
                    if (random.NextDouble() < MutationProbability)
                    {
                        SonarAngleResponse[i] += 0.25 * (random.NextDouble() * 2 - 1);
                        SonarAngleResponse[i] = Math.Min(Math.Max(SonarAngleResponse[i], 0), 1);
                    }
                    if (random.NextDouble() < MutationProbability * 2)
                    {
                        SonarDistanceLimit[i] += 0.5 * (random.NextDouble() * 2 - 1);
                        SonarDistanceLimit[i] = Math.Max(SonarDistanceLimit[i], 0);
                    }
                }
            }

            public void Mix(GeneSet g1, GeneSet g2)
            {
                WMax = (random.NextDouble() > 0.5) ? g1.WMax : g2.WMax;
                SonarAngleResponse = (double[])g1.SonarAngleResponse.Clone();
                SonarDistanceLimit = (double[])g1.SonarDistanceLimit.Clone();
                for (int i = 0; i < SonarAngleResponse.Length; i++)
                {
                    if (random.NextDouble() > 0.75)
                    {
                        SonarAngleResponse[i] = g2.SonarAngleResponse[i];
                    }
                    if (random.NextDouble() > 0.75)
                    {
                        SonarDistanceLimit[i] = g2.SonarDistanceLimit[i];
                    }
                }
            }

            #region Save/Load
            public void Save(string name)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(GeneSet));
                using (var streamWriter = new StreamWriter(name, false, Encoding.UTF8))
                {
                    serializer.Serialize(streamWriter, this);
                    streamWriter.Flush();
                }
            }

            public static GeneSet Load(string name)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(GeneSet));
                GeneSet res;
                var xmlSettings = new System.Xml.XmlReaderSettings()
                {
                    CheckCharacters = false,
                };
                using (var streamReader = new StreamReader(name, Encoding.UTF8))
                using (var xmlReader = System.Xml.XmlReader.Create(streamReader, xmlSettings))
                {
                    res = (GeneSet)serializer.Deserialize(xmlReader);
                }
                return res;
            }
            #endregion
        }
        public GeneSet Gene { get; set; } = new GeneSet();
        #endregion

        #region Sensor
        #region SensorClass
        public List<Sensor.Sonar> Sonares { get; set; } = new List<Sensor.Sonar>();
        #endregion

        public double WallCollisionRatio { get; set; } = 2;

        public List<int> ViewAngles { get; set; } = new List<int>() { 0, 1, 2, 3, 4, 6, 10, 15, 20, 30, 45, 65, 90, 110 };

        public List<double> ToF_Distance { get; set; }
        #endregion

        #region Operator
        public static Parameter operator *(Parameter p1, Parameter p2)
        {
            var ret = new Body.Parameter();
            ret.AliveTime = p1.AliveTime;
            ret.QualityAliveTime = p1.QualityAliveTime;
            ret.Gene.Mix(p1.Gene, p2.Gene);
            return ret;
        }
        #endregion

    }
}
