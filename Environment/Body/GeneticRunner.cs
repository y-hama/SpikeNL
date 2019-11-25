using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Environment.Body
{
    public class GeneticRunner : FreeRunner
    {
        #region Property
        public double[] SonarAngleResponse { get { return Parameter.Gene.SonarAngleResponse; } protected set { Parameter.Gene.SonarAngleResponse = value; } }
        #endregion

        protected override void InitializeParameter()
        {
            WMax = 0.5 + random.NextDouble() / 2;
            SonarAngleResponse = new double[Sonares.Count];
            SonarDistanceLimit = new double[Sonares.Count];
            for (int i = 0; i < SonarAngleResponse.Length; i++)
            {
                SonarAngleResponse[i] = Math.Exp(-Math.Pow(8 * Sonares[i].RelativeDirection / 180, 2)) + 0.1 * (random.NextDouble() * 2 - 1);
                SonarDistanceLimit[i] = Size * (1 + Math.Exp(-Math.Pow(8 * Sonares[i].RelativeDirection / 180, 2))) + 0.1 * (random.NextDouble() * 2 - 1);
            }
        }

        protected override void SetColor()
        {
            Color = Color.LawnGreen;
        }

        public override void WheelRotation(ref double left, ref double right)
        {
            double dist = UpdateSonar();

            if (dist < Size / 2)
            {
                this.IsDead = true;
            }

            left = right = 1;
            for (int i = 0; i < Sonares.Count; i++)
            {
                var item = Sonares[i];
                if (Math.Abs(item.Distance) < WallCollisionRatio * SonarDistanceLimit[i])
                {
                    double distr = ((item.Distance == 0) ? 0 : (1.0 / item.Distance)) * Size;
                    if (Math.Abs(item.RelativeDirection) == 0)
                    {
                        left -= distr * SonarAngleResponse[i];
                        right -= distr * SonarAngleResponse[i];
                    }
                    else
                    if (item.RelativeDirection < 0)
                    {
                        left += distr * SonarAngleResponse[i];
                    }
                    else if (item.RelativeDirection > 0)
                    {
                        right += distr * SonarAngleResponse[i];
                    }
                }
            }
        }

        public override BaseBody InheritanceNew(Parameter parameter, bool isMutation = false)
        {
            var item = new GeneticRunner();
            if (isMutation) { item.Parameter.Gene.Mutation(parameter.Gene); }
            return item;
        }
    }
}
