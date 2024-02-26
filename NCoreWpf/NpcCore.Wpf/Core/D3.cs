using System.Windows.Media.Media3D;

namespace NpcCore.Wpf.Core
{
    public static class D3
    {
        // Make a transformation for rotation around an arbitrary axis.
        public static RotateTransform3D Rotate(Vector3D axis, double angle)
        {
            Rotation3D rotation = new AxisAngleRotation3D(axis, angle);
            return new RotateTransform3D(rotation);
        }

        public static RotateTransform3D Rotate(Vector3D axis, Point3D center, double angle)
        {
            Rotation3D rotation = new AxisAngleRotation3D(axis, angle);
            return new RotateTransform3D(rotation, center);
        }

        public static ScaleTransform3D InverseTransform(ScaleTransform3D transform)
        {
            return new ScaleTransform3D(1 / transform.ScaleX, 1 / transform.ScaleY, 1 / transform.ScaleZ);
        }

        public static RotateTransform3D InverseTransform(RotateTransform3D transform)
        {
            // [NCS-2695] CID 171172 Unchecked dynamic_cast
            //AxisAngleRotation3D rotation = transform.Rotation as AxisAngleRotation3D;
            //return Rotate(rotation.Axis, -rotation.Angle);
            if (transform.Rotation is AxisAngleRotation3D rotation)
            {
                return Rotate(rotation.Axis, -rotation.Angle);
            }
            else
            {
                return new RotateTransform3D();
            }
        }

        // Return the origin.
        public static Point3D Origin
        {
            get { return new Point3D(); }
        }

        // Return vectors along the coordinate axes.
        public static Vector3D XVector(double length = 1)
        {
            return new Vector3D(length, 0, 0);
        }
        public static Vector3D YVector(double length = 1)
        {
            return new Vector3D(0, length, 0);
        }
        public static Vector3D ZVector(double length = 1)
        {
            return new Vector3D(0, 0, length);
        }
    }
}
