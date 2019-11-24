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

        protected override void SetColor()
        {
            Color = Color.LawnGreen;
        }

        public override void WheelRotation(ref double left, ref double right)
        {
            double dist = UpdateSonar();

            if (dist < BaseBody.Size / 2)
            {
                this.IsDead = true;
            }

            left = random.NextDouble();
            right = random.NextDouble();
        }

        public override BaseBody InheritanceNew()
        {
            var item = new GeneticRunner();
            return item;
        }
    }
}
