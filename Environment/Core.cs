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
        public static Bitmap RepresentView { get; private set; }

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

        public static Body.BaseBody GetRepresentUnit()
        {
            return Body.BodyList.UnitList[0];
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
            try
            {
                var bitmap = (Bitmap)View.Clone();
                var g = Graphics.FromImage(bitmap);
                foreach (var item in Body.BodyList.UnitList)
                {
                    item.Draw(ref bitmap, ref g);
                }

                if (Body.BodyList.UnitList.Count > 0)
                {
                    var rep = Body.BodyList.UnitList[0];
                    g.DrawRectangle(new Pen(Color.Violet, 5), new Rectangle((int)(rep.X - Body.BaseBody.Size / 4), (int)(rep.Y - Body.BaseBody.Size / 4), (int)Body.BaseBody.Size / 2, (int)Body.BaseBody.Size / 2));

                    Bitmap repView;
                    rep.ViewImage(out repView);
                    RepresentView = repView;
                }

                ShowImage = bitmap;

            }
            catch (Exception)
            {

            }
        }

    }
}
