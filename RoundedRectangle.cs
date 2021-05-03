using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Asocijacije {
    public static class RoundedRectangle {
        public static void FillRoundedRectangle(this Graphics graphics, Brush brush, RectangleF bounds, float cornerRadius) {
            if (graphics == null)
                throw new ArgumentNullException("graphics");
            if (brush == null)
                throw new ArgumentNullException("brush");

            using (GraphicsPath path = RoundedRect(bounds, cornerRadius)) {
                graphics.FillPath(brush, path);
            }
        }

        private static GraphicsPath RoundedRect(RectangleF rect, float radius) {
            PointF point1, point2;
            GraphicsPath path = new GraphicsPath();
            path.AddArc(new RectangleF(rect.X, rect.Y, 2 * radius, 2 * radius), 180, 90);
            point1 = new PointF(rect.X + radius, rect.Y);
            point2 = new PointF(rect.Right - radius, rect.Y);
            path.AddLine(point1, point2);
            path.AddArc(new RectangleF(rect.Right - 2 * radius, rect.Y, 2 * radius, 2 * radius), 270, 90);
            point1 = new PointF(rect.Right, rect.Y + radius);
            point2 = new PointF(rect.Right, rect.Bottom - radius);
            path.AddLine(point1, point2);
            path.AddArc(new RectangleF(rect.Right - 2 * radius, rect.Bottom - 2 * radius, 2 * radius, 2 * radius), 0, 90);
            point1 = new PointF(rect.Right - radius, rect.Bottom);
            point2 = new PointF(rect.X + radius, rect.Bottom);
            path.AddLine(point1, point2);
            path.AddArc(new RectangleF(rect.X, rect.Bottom - 2 * radius, 2 * radius, 2 * radius), 90, 90);
            point1 = new PointF(rect.X, rect.Bottom - radius);
            point2 = new PointF(rect.X, rect.Y + radius);
            path.AddLine(point1, point2);
            path.CloseFigure();
            return path;
        }

        //public static GraphicsPath RoundedRect(Rectangle bounds, int radius) {
        //    int diameter = radius * 2;
        //    Size size = new Size(diameter, diameter);
        //    Rectangle arc = new Rectangle(bounds.Location, size);
        //    GraphicsPath path = new GraphicsPath();
        //    if (radius == 0) {
        //        path.AddRectangle(bounds);
        //        return path;
        //    }
        //    path.AddArc(arc, 180, 90);
        //    arc.X = bounds.Right - diameter;
        //    path.AddArc(arc, 270, 90);
        //    arc.Y = bounds.Bottom - diameter;
        //    path.AddArc(arc, 0, 90);
        //    arc.X = bounds.Left;
        //    path.AddArc(arc, 90, 90);
        //    path.CloseFigure();
        //    return path;
        //}
    }
}
