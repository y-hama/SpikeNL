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
        public static Body.Parameter LatestParameter { get; private set; }

        public static Bitmap View
        {
            get { return Background.Vision.Image; }
        }

        public static Bitmap ShowImage { get; private set; }

        public static int Generation { get; private set; } = 0;

        public static bool HasRepresentView
        {
            get { return innerRepresentView == null ? false : true; }
        }
        private static Bitmap innerRepresentView { get; set; }
        public static Bitmap RepresentView(Size size)
        {
            Bitmap res = (Bitmap)innerRepresentView.Clone();
            Bitmap view = new Bitmap(size.Width, size.Height);
            Graphics g = Graphics.FromImage(view);
            g.FillRectangle(Brushes.Black, new Rectangle(new Point(), size));

            int stepsize = (int)(size.Width / res.Width) + 1;
            for (int i = 0; i < res.Width; i++)
            {
                Color c = res.GetPixel(i, 0);
                g.FillRectangle(new SolidBrush(c), new Rectangle(new Point(i * stepsize, 0), new Size(stepsize, size.Height)));
            }
            return view;
        }

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
                    Body.BodyList.DeadUnitList.Add(item);
                }
                else
                {
                    item.Update();
                    if (LatestParameter != null)
                    {
                        if (item.AliveTime > LatestParameter.AliveTime * 10)
                        {
                            item.Kill();
                            remlist.Add(item);
                            Body.BodyList.DeadUnitList.Add(item);
                        }
                    }
                }
            }
            bool ckg = false;
            foreach (var item in remlist)
            {
                Body.BodyList.UnitList.Remove(item);
                if (LatestParameter != null)
                {
                    if (LatestParameter.AliveTime < item.QualityAliveTime)
                    {
                        LatestParameter = item.Parameter * LatestParameter;
                    }
                    var reborn = item.InheritanceNew(LatestParameter, true);
                    reborn.Growup();
                    if (reborn != null)
                    {
                        AddUnit(reborn);
                        if (!ckg)
                        {
                            ckg = true;
                            Generation++;
                        }
                    }
                }
            }
            if (Body.BodyList.UnitList.Count == 0)
            {
                Generation++;
                var fitem = Body.BodyList.DeadUnitList.First(x => Equals(x.QualityAliveTime, Body.BodyList.DeadUnitList.Max(y => y.QualityAliveTime)));
                var fparam = fitem.Parameter;
                var slist = new List<Body.BaseBody>(Body.BodyList.DeadUnitList);
                if (LatestParameter == null)
                {
                    slist.Remove(fitem);
                }
                else
                {
                    if (LatestParameter.AliveTime < fitem.QualityAliveTime)
                    {
                        fparam = LatestParameter;
                    }
                    else
                    {
                        slist.Remove(fitem);
                    }
                }
                var sitem = slist.First(x => Equals(x.QualityAliveTime, slist.Max(y => y.QualityAliveTime)));
                LatestParameter = fparam * sitem.Parameter;
                var source = Body.BodyList.DeadUnitList[0];
                int count = Body.BodyList.DeadUnitList.Count;
                var reborn = source.InheritanceNew(fparam, true);
                if (reborn != null)
                {
                    AddUnit(reborn);
                }
                for (int i = 1; i < count; i++)
                {
                    reborn = source.InheritanceNew(LatestParameter, true);
                    if (reborn != null)
                    {
                        AddUnit(reborn);
                    }
                }
                Body.BodyList.DeadUnitList.Clear();
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
                    var rep = Body.BodyList.UnitList.First(x => Equals(x.QualityAliveTime, Body.BodyList.UnitList.Max(y => y.QualityAliveTime)));
                    g.DrawEllipse(new Pen(Color.LightCyan, 5), new Rectangle((int)(rep.X - rep.Size / 4), (int)(rep.Y - rep.Size / 4), (int)rep.Size / 2, (int)rep.Size / 2));

                    Bitmap repView;
                    rep.ViewImage(out repView);
                    innerRepresentView = repView;
                }

                ShowImage = bitmap;

            }
            catch (Exception)
            {

            }
        }

    }
}
