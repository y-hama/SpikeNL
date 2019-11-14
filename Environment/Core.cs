using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Environment
{
    public static class Core
    {
        public static Bitmap View
        {
            get { return Background.Vision.Image; }
        }

        public static Bitmap ShowImage { get; private set; }

        public static void SetVision(Background.ModeType mode, Bitmap bitmap = null)
        {
            if (Background.Vision.Mode == Background.ModeType.NoSignal)
            {
                switch (mode)
                {
                    case Background.ModeType.NoSignal:
                        break;
                    case Background.ModeType.Image:
                        Background.Vision.Image = bitmap;
                        break;
                    case Background.ModeType.Camera:
                        break;
                    default:
                        break;
                }
                Background.Vision.Mode = mode;
            }
        }

        public static void AddUnit(Body.BaseBody body)
        {
            Body.BodyList.UnitList.Add(body);
        }


        public static void Update()
        {
            List<Body.BaseBody> remlist = new List<Body.BaseBody>();
            foreach (var item in Body.BodyList.UnitList)
            {
                item.Growup();
                if (item.IsDead)
                {
                    remlist.Add(item);
                }
                else
                {
                    item.Update();
                }
            }
            foreach (var item in remlist)
            {
                Body.BodyList.UnitList.Remove(item);
            }
            UpdateShowImage();
            GC.Collect();
        }

        private static void UpdateShowImage()
        {
            var bitmap = (Bitmap)View.Clone();
            var g = Graphics.FromImage(bitmap);
            foreach (var item in Body.BodyList.UnitList)
            {
                item.Draw(ref bitmap, ref g);
            }
            ShowImage = bitmap;
        }

    }
}
