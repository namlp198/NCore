using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Npc.Foundation.Math
{
    public static class VectorEx
    {
        private const double DegToRad = System.Math.PI / 180;

        /// <summary>
        /// Rotate a vector around a certain point
        /// [NCS-2235]
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="angle"></param>
        /// <param name="center"></param>
        /// <returns></returns>
        public static Point Rotate(this Point pt, double angle, Point center)
        {
            var v = new Vector(pt.X - center.X, pt.Y - center.Y).Rotate(angle);
            return new Point(v.X + center.X, v.Y + center.Y);
        }

        public static Vector Rotate(this Vector v, double degrees)
        {
            return v.RotateRadians(degrees * DegToRad);
        }

        public static Vector RotateRadians(this Vector v, double radians)
        {
            var ca = System.Math.Cos(radians);
            var sa = System.Math.Sin(radians);
            return new Vector(ca * v.X - sa * v.Y, sa * v.X + ca * v.Y);
        }
    }
}
