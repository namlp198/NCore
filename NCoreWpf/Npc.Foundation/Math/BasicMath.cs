using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Windows.Media;

namespace System
{
    public static class BasicMath
    {
        private static Random _rand = new Random((int)DateTime.Now.Ticks);

        // 도 -> 라디안
        public static double DegreeToRadian(double degree) { return Math.PI * degree / 180.0d; }

        public static double DegreeNormalize(double degree)
        {
            var m = degree % 360.0d;

            if (m < 0.0d)
            {
                m += 360.0d;
            }

            return m;
        }

        /// <summary>
        /// KJH : 두점 사이의 거리.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static double CalcDistanceOfTwoPoints(Point start, Point end)
        {
            return Math.Sqrt(Math.Pow((end.X - start.X), 2) + Math.Pow((end.Y - start.Y), 2));
        }

        /// <summary>
        /// KJH : 두점 사이의 거리
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static double CalcDistanceOfTwoPoints(double start, double end)
        {
            return Math.Sqrt(Math.Pow(start, 2) + Math.Pow(end, 2));
        }

        // 두 점의 각도를 계산
        public static double CalcDegreeOfTwoPoints(Point start, Point end)
        {
            var deltaY = Math.Pow((end.Y - end.Y), 2d);

            var radian = Math.Atan2((end.Y - start.Y), (end.X - start.X));

            return (radian * (180d / Math.PI) + 360d) % 360d;
        }

        // 점을 회전
        public static Point CalcRotatePoint(Point pointToRotate, Point centerPoint, double degrees)
        {
            double angleInRadians = degrees * (Math.PI / 180.0d);
            double cosTheta = Math.Cos(angleInRadians);
            double sinTheta = Math.Sin(angleInRadians);
            return new Point
            {
                X =
                    (double)
                    (cosTheta * (pointToRotate.X - centerPoint.X) -
                    sinTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.X),
                Y =
                    (double)
                    (sinTheta * (pointToRotate.X - centerPoint.X) +
                    cosTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.Y)
            };
        }

        public static Rect CalcRotateRect(Rect rectToRotate, double degrees)
        {
            return CalcRotateRect(rectToRotate, GetCenter(rectToRotate), degrees);
        }

        public static Rect CalcRotateRect(Rect rectToRotate, Point centerPoint, double degrees)
        {
            var a = CalcRotatePoint(rectToRotate.TopLeft, centerPoint, degrees);
            var b = CalcRotatePoint(rectToRotate.BottomRight, centerPoint, degrees);
            return new Rect(a, b);
        }

        /// <summary>
        /// 사각형을 회전한 후 전체를 포함하는 Rect를 구함
        /// </summary>
        /// <param name="rectToRotate"></param>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static Rect CalcRotateRectOuter(Rect rectToRotate, double degrees)
        {
            Point centerPoint = GetCenter(rectToRotate);
            return CalcRotateRectOuter(rectToRotate, centerPoint, degrees);
        }

        /// <summary>
        /// 사각형을 회전한 후 전체를 포함하는 Rect를 구함
        /// </summary>
        /// <param name="rectToRotate"></param>
        /// <param name="centerPoint"></param>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static Rect CalcRotateRectOuter(Rect rectToRotate, Point centerPoint, double degrees)
        {
            if (rectToRotate == Rect.Empty)
                return Rect.Empty;

            //모서리 네개의 점을 회전함
            Point topLeft = rectToRotate.TopLeft;
            Point topRight = rectToRotate.TopRight;
            Point bottomLeft = rectToRotate.BottomLeft;
            Point bottomRight = rectToRotate.BottomRight;

            Point topLeftRotate = BasicMath.CalcRotatePoint(topLeft, centerPoint, degrees);
            Point topRightRotate = BasicMath.CalcRotatePoint(topRight, centerPoint, degrees);
            Point bottomLeftRotate = BasicMath.CalcRotatePoint(bottomLeft, centerPoint, degrees);
            Point bottomRightRotate = BasicMath.CalcRotatePoint(bottomRight, centerPoint, degrees);

            //회전한 네개의 점들의 min, max값을 계산해서 Rect 값을 구함
            double minX = Math.Min(bottomRightRotate.X, Math.Min(bottomLeftRotate.X, Math.Min(topLeftRotate.X, topRightRotate.X)));
            double maxX = Math.Max(bottomRightRotate.X, Math.Max(bottomLeftRotate.X, Math.Max(topLeftRotate.X, topRightRotate.X)));
            double minY = Math.Min(bottomRightRotate.Y, Math.Min(bottomLeftRotate.Y, Math.Min(topLeftRotate.Y, topRightRotate.Y)));
            double maxY = Math.Max(bottomRightRotate.Y, Math.Max(bottomLeftRotate.Y, Math.Max(topLeftRotate.Y, topRightRotate.Y)));

            Rect result = new Rect(minX, minY, maxX - minX, maxY - minY);

            return result;
        }

        /// <summary>
        /// 사각형을 center 기준으로 Resize함
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="ratio">확대 비율 (ex. 10% 확대인 경우 값이 1.1임. 이때 left, top, right, bottom 각각 절반인 5%씩 확대됨)</param>
        /// <returns></returns>
        public static Rect ResizeRect(Rect rect, double ratio)
        {
            if (ratio == 1.0)
                return rect;

            double resultWidth = rect.Width * ratio;
            double gapWidth = Math.Abs(rect.Width - resultWidth) / 2;

            double resultHeight = rect.Height * ratio;
            double gapHeight = Math.Abs(rect.Height - resultHeight) / 2;

            Rect resultRect = new Rect();
            if (ratio < 1.0)
            {
                resultRect.X = rect.X + gapWidth;
                resultRect.Y = rect.Y + gapHeight;
                resultRect.Width = rect.Width - (gapWidth * 2);
                resultRect.Height = rect.Height - (gapHeight * 2);
            }
            else
            {
                resultRect.X = rect.X - gapWidth;
                resultRect.Y = rect.Y - gapHeight;
                resultRect.Width = rect.Width + (gapWidth * 2);
                resultRect.Height = rect.Height + (gapHeight * 2);
            }

            return resultRect;
        }

        public static Point GetCenter(Rect rect)
        {
            return new Point(rect.X + rect.Width * 0.5d, rect.Y + rect.Height * 0.5d);
        }

        public static Point GetLeftTopPoint(Rect rect)
        {
            return new Point(rect.X - rect.Width * 0.5d, rect.Y - rect.Height * 0.5d);
        }

        public static Point PointTryParse(string data)
        {
            Point result = new Point();
            if (string.IsNullOrWhiteSpace(data) == true)
            {
                return result;
            }

            data = data.Replace(" ", "");
            string[] arr = data.Split(',');

            double x;
            double y;

            if (arr.Length == 2 && double.TryParse(arr[0], out x) && double.TryParse(arr[1], out y))
            {
                result.X = x;
                result.Y = y;
            }

            return result;
        }

        /// <summary>
        /// [ NCS-520 : Progress Notify Merge ]
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Size SizeTryParse(string data)
        {
            Size result = new Size();
            if (string.IsNullOrWhiteSpace(data) == true)
            {
                return result;
            }

            data = data.Replace(" ", "");
            string[] arr = data.Split(',');

            double x;
            double y;

            if (arr.Length == 2 && double.TryParse(arr[0], out x) && double.TryParse(arr[1], out y))
            {
                result.Width = x;
                result.Height = y;
            }

            return result;
        }

        /// <summary>
        /// min, max를 포함한 Random값을 구함
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int GetRandomIntValue(int min, int max)
        {
            return _rand.Next(min, max + 1);
        }

        /// <summary>
        /// 0 ~ 1 사이 Double값을 가져옴
        /// </summary>
        /// <param name="roundCount">자리수</param>
        /// <returns></returns>
        public static double GetRandomDoubleValue(int roundCount)
        {
            double value = _rand.NextDouble();
            if (roundCount <= 0)
                return value;

            return System.Math.Round(value, roundCount);
        }

        /// <summary>
        /// object 에서 double 값을 추출합니다.
        /// </summary>
        /// <param name="objectValue"></param>
        /// <returns></returns>
        public static double GetDoubleValueFromObject(object objectValue)
        {
            if (objectValue == DependencyProperty.UnsetValue)
            {
                return 0.0d;
            }
            else
            {
                return System.Convert.ToDouble(objectValue ?? 0.0d);
            }
        }

        #region new copy
        public static double GetAspectRatio(Size size)
        {
            return size.Width / size.Height;
        }

        public static double DegreesToRadians(double degrees)
        {
            return degrees * (Math.PI / 180.0);
        }

        private static Matrix3D GetViewMatrix(ProjectionCamera camera)
        {
            Debug.Assert(camera != null,
                "Caller needs to ensure camera is non-null.");

            // This math is identical to what you find documented for
            // D3DXMatrixLookAtRH with the exception that WPF uses a
            // LookDirection vector rather than a LookAt point.

            Vector3D zAxis = -camera.LookDirection;
            zAxis.Normalize();

            Vector3D xAxis = Vector3D.CrossProduct(camera.UpDirection, zAxis);
            xAxis.Normalize();

            Vector3D yAxis = Vector3D.CrossProduct(zAxis, xAxis);

            Vector3D position = (Vector3D)camera.Position;
            double offsetX = -Vector3D.DotProduct(xAxis, position);
            double offsetY = -Vector3D.DotProduct(yAxis, position);
            double offsetZ = -Vector3D.DotProduct(zAxis, position);

            return new Matrix3D(
                xAxis.X, yAxis.X, zAxis.X, 0,
                xAxis.Y, yAxis.Y, zAxis.Y, 0,
                xAxis.Z, yAxis.Z, zAxis.Z, 0,
                offsetX, offsetY, offsetZ, 1);
        }

        /// <summary>
        ///     Computes the effective view matrix for the given
        ///     camera.
        /// </summary>
        public static Matrix3D GetViewMatrix(Camera camera)
        {
            if (camera == null)
            {
                throw new ArgumentNullException("camera");
            }

            ProjectionCamera projectionCamera = camera as ProjectionCamera;

            if (projectionCamera != null)
            {
                return GetViewMatrix(projectionCamera);
            }

            MatrixCamera matrixCamera = camera as MatrixCamera;

            if (matrixCamera != null)
            {
                return matrixCamera.ViewMatrix;
            }

            throw new ArgumentException(String.Format("Unsupported camera type '{0}'.", camera.GetType().FullName), "camera");
        }

        private static Matrix3D GetProjectionMatrix(OrthographicCamera camera, double aspectRatio)
        {
            Debug.Assert(camera != null,
                "Caller needs to ensure camera is non-null.");

            // This math is identical to what you find documented for
            // D3DXMatrixOrthoRH with the exception that in WPF only
            // the camera's width is specified.  Height is calculated
            // from width and the aspect ratio.

            double w = camera.Width;
            double h = w / aspectRatio;
            double zn = camera.NearPlaneDistance;
            double zf = camera.FarPlaneDistance;

            double m33 = 1 / (zn - zf);
            double m43 = zn * m33;

            return new Matrix3D(
                2 / w, 0, 0, 0,
                  0, 2 / h, 0, 0,
                  0, 0, m33, 0,
                  0, 0, m43, 1);
        }

        private static Matrix3D GetProjectionMatrix(PerspectiveCamera camera, double aspectRatio)
        {
            Debug.Assert(camera != null,
                "Caller needs to ensure camera is non-null.");

            // This math is identical to what you find documented for
            // D3DXMatrixPerspectiveFovRH with the exception that in
            // WPF the camera's horizontal rather the vertical
            // field-of-view is specified.

            double hFoV = DegreesToRadians(camera.FieldOfView);
            double zn = camera.NearPlaneDistance;
            double zf = camera.FarPlaneDistance;

            double xScale = 1 / Math.Tan(hFoV / 2);
            double yScale = aspectRatio * xScale;
            double m33 = (zf == double.PositiveInfinity) ? -1 : (zf / (zn - zf));
            double m43 = zn * m33;

            return new Matrix3D(
                xScale, 0, 0, 0,
                     0, yScale, 0, 0,
                     0, 0, m33, -1,
                     0, 0, m43, 0);
        }

        /// <summary>
        ///     Computes the effective projection matrix for the given
        ///     camera.
        /// </summary>
        public static Matrix3D GetProjectionMatrix(Camera camera, double aspectRatio)
        {
            if (camera == null)
            {
                throw new ArgumentNullException("camera");
            }

            PerspectiveCamera perspectiveCamera = camera as PerspectiveCamera;

            if (perspectiveCamera != null)
            {
                return GetProjectionMatrix(perspectiveCamera, aspectRatio);
            }

            OrthographicCamera orthographicCamera = camera as OrthographicCamera;

            if (orthographicCamera != null)
            {
                return GetProjectionMatrix(orthographicCamera, aspectRatio);
            }

            MatrixCamera matrixCamera = camera as MatrixCamera;

            if (matrixCamera != null)
            {
                return matrixCamera.ProjectionMatrix;
            }

            throw new ArgumentException(String.Format("Unsupported camera type '{0}'.", camera.GetType().FullName), "camera");
        }

        private static Matrix3D GetHomogeneousToViewportTransform(Rect viewport)
        {
            double scaleX = viewport.Width / 2;
            double scaleY = viewport.Height / 2;
            double offsetX = viewport.X + scaleX;
            double offsetY = viewport.Y + scaleY;

            return new Matrix3D(
                 scaleX, 0, 0, 0,
                      0, -scaleY, 0, 0,
                      0, 0, 1, 0,
                offsetX, offsetY, 0, 1);
        }

        /// <summary>
        ///     Computes the transform from world space to the Viewport3DVisual's
        ///     inner 2D space.
        /// 
        ///     This method can fail if Camera.Transform is non-invertable
        ///     in which case the camera clip planes will be coincident and
        ///     nothing will render.  In this case success will be false.
        /// </summary>
        public static Matrix3D TryWorldToViewportTransform(Viewport3DVisual visual, out bool success)
        {
            success = false;
            Matrix3D result = TryWorldToCameraTransform(visual, out success);

            if (success)
            {
                result.Append(GetProjectionMatrix(visual.Camera, GetAspectRatio(visual.Viewport.Size)));
                result.Append(GetHomogeneousToViewportTransform(visual.Viewport));
                success = true;
            }

            return result;
        }


        /// <summary>
        ///     Computes the transform from world space to camera space
        /// 
        ///     This method can fail if Camera.Transform is non-invertable
        ///     in which case the camera clip planes will be coincident and
        ///     nothing will render.  In this case success will be false.
        /// </summary>
        public static Matrix3D TryWorldToCameraTransform(Viewport3DVisual visual, out bool success)
        {
            success = false;
            Matrix3D result = Matrix3D.Identity;

            Camera camera = visual.Camera;

            if (camera == null)
            {
                return ZeroMatrix;
            }

            Rect viewport = visual.Viewport;

            if (viewport == Rect.Empty)
            {
                return ZeroMatrix;
            }

            Transform3D cameraTransform = camera.Transform;

            if (cameraTransform != null)
            {
                Matrix3D m = cameraTransform.Value;

                if (!m.HasInverse)
                {
                    return ZeroMatrix;
                }

                m.Invert();
                result.Append(m);
            }

            result.Append(GetViewMatrix(camera));

            success = true;
            return result;
        }

        /// <summary>
        /// Gets the object space to world space transformation for the given DependencyObject
        /// </summary>
        /// <param name="visual">The visual whose world space transform should be found</param>
        /// <param name="viewport">The Viewport3DVisual the Visual is contained within</param>
        /// <returns>The world space transformation</returns>
        private static Matrix3D GetWorldTransformationMatrix(DependencyObject visual, out Viewport3DVisual viewport)
        {
            Matrix3D worldTransform = Matrix3D.Identity;
            viewport = null;

            if (!(visual is Visual3D))
            {
                throw new ArgumentException("Must be of type Visual3D.", "visual");
            }

            while (visual != null)
            {
                if (!(visual is ModelVisual3D))
                {
                    break;
                }

                Transform3D transform = (Transform3D)visual.GetValue(ModelVisual3D.TransformProperty);

                if (transform != null)
                {
                    worldTransform.Append(transform.Value);
                }

                visual = VisualTreeHelper.GetParent(visual);
            }

            viewport = visual as Viewport3DVisual;

            if (viewport == null)
            {
                if (visual != null)
                {
                    // In WPF 3D v1 the only possible configuration is a chain of
                    // ModelVisual3Ds leading up to a Viewport3DVisual.

                    throw new ApplicationException(
                        String.Format("Unsupported type: '{0}'.  Expected tree of ModelVisual3Ds leading up to a Viewport3DVisual.",
                        visual.GetType().FullName));
                }

                return ZeroMatrix;
            }

            return worldTransform;
        }

        /// <summary>
        ///     Computes the transform from the inner space of the given
        ///     Visual3D to the 2D space of the Viewport3DVisual which
        ///     contains it.
        /// 
        ///     The result will contain the transform of the given visual.
        /// 
        ///     This method can fail if Camera.Transform is non-invertable
        ///     in which case the camera clip planes will be coincident and
        ///     nothing will render.  In this case success will be false.
        /// </summary>
        /// <param name="visual"></param>
        /// <param name="success"></param>
        /// <returns></returns>
        public static Matrix3D TryTransformTo2DAncestor(DependencyObject visual, out Viewport3DVisual viewport, out bool success)
        {
            Matrix3D to2D = GetWorldTransformationMatrix(visual, out viewport);
            to2D.Append(TryWorldToViewportTransform(viewport, out success));

            if (!success)
            {
                return ZeroMatrix;
            }

            return to2D;
        }


        /// <summary>
        ///     Computes the transform from the inner space of the given
        ///     Visual3D to the camera coordinate space
        /// 
        ///     The result will contain the transform of the given visual.
        /// 
        ///     This method can fail if Camera.Transform is non-invertable
        ///     in which case the camera clip planes will be coincident and
        ///     nothing will render.  In this case success will be false.
        /// </summary>
        /// <param name="visual"></param>
        /// <param name="success"></param>
        /// <returns></returns>
        public static Matrix3D TryTransformToCameraSpace(DependencyObject visual, out Viewport3DVisual viewport, out bool success)
        {
            Matrix3D toViewSpace = GetWorldTransformationMatrix(visual, out viewport);
            toViewSpace.Append(TryWorldToCameraTransform(viewport, out success));

            if (!success)
            {
                return ZeroMatrix;
            }

            return toViewSpace;
        }

        /// <summary>
        ///     Transforms the axis-aligned bounding box 'bounds' by
        ///     'transform'
        /// </summary>
        /// <param name="bounds">The AABB to transform</param>
        /// <returns>Transformed AABB</returns>
        public static Rect3D TransformBounds(Rect3D bounds, Matrix3D transform)
        {
            double x1 = bounds.X;
            double y1 = bounds.Y;
            double z1 = bounds.Z;
            double x2 = bounds.X + bounds.SizeX;
            double y2 = bounds.Y + bounds.SizeY;
            double z2 = bounds.Z + bounds.SizeZ;

            Point3D[] points = new Point3D[] {
                new Point3D(x1, y1, z1),
                new Point3D(x1, y1, z2),
                new Point3D(x1, y2, z1),
                new Point3D(x1, y2, z2),
                new Point3D(x2, y1, z1),
                new Point3D(x2, y1, z2),
                new Point3D(x2, y2, z1),
                new Point3D(x2, y2, z2),
            };

            transform.Transform(points);

            // reuse the 1 and 2 variables to stand for smallest and largest
            Point3D p = points[0];
            x1 = x2 = p.X;
            y1 = y2 = p.Y;
            z1 = z2 = p.Z;

            for (int i = 1; i < points.Length; i++)
            {
                p = points[i];

                x1 = Math.Min(x1, p.X); y1 = Math.Min(y1, p.Y); z1 = Math.Min(z1, p.Z);
                x2 = Math.Max(x2, p.X); y2 = Math.Max(y2, p.Y); z2 = Math.Max(z2, p.Z);
            }

            return new Rect3D(x1, y1, z1, x2 - x1, y2 - y1, z2 - z1);
        }

        /// <summary>
        ///     Normalizes v if |v| > 0.
        /// 
        ///     This normalization is slightly different from Vector3D.Normalize. Here
        ///     we just divide by the length but Vector3D.Normalize tries to avoid
        ///     overflow when finding the length.
        /// </summary>
        /// <param name="v">The vector to normalize</param>
        /// <returns>'true' if v was normalized</returns>
        public static bool TryNormalize(ref Vector3D v)
        {
            double length = v.Length;

            if (length != 0)
            {
                v /= length;
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Computes the center of 'box'
        /// </summary>
        /// <param name="box">The Rect3D we want the center of</param>
        /// <returns>The center point</returns>
        public static Point3D GetCenter(Rect3D box)
        {
            return new Point3D(box.X + box.SizeX / 2, box.Y + box.SizeY / 2, box.Z + box.SizeZ / 2);
        }

        public static readonly Matrix3D ZeroMatrix = new Matrix3D(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

        public static readonly Vector3D XAxis = new Vector3D(1, 0, 0);
        public static readonly Vector3D YAxis = new Vector3D(0, 1, 0);
        public static readonly Vector3D ZAxis = new Vector3D(0, 0, 1);
    }
    #endregion
}
