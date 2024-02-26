using NpcCore.Wpf.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace NpcCore.Wpf.Core
{
    public static class MeshExtensions
    {
        #region Transformation

        // Apply a transformation Matrix3D or transformation class.
        public static void ApplyTransformation(this MeshGeometry3D mesh, Matrix3D transformation)
        {
            Point3D[] points = mesh.Positions.ToArray();
            transformation.Transform(points);
            mesh.Positions = new Point3DCollection(points);

            Vector3D[] normals = mesh.Normals.ToArray();
            transformation.Transform(normals);
            mesh.Normals = new Vector3DCollection(normals);
        }

        public static void ApplyTransformation(this MeshGeometry3D mesh, Transform3D transformation)
        {
            Point3D[] points = mesh.Positions.ToArray();
            transformation.Transform(points);
            mesh.Positions = new Point3DCollection(points);

            Vector3D[] normals = mesh.Normals.ToArray();
            transformation.Transform(normals);
            mesh.Normals = new Vector3DCollection(normals);
        }

        #endregion

        /// <summary>
        /// Add a cylinder with smooth sides.
        /// http://csharphelper.com/blog/2015/04/draw-smooth-cylinders-using-wpf-and-c/
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="end_point"></param>
        /// <param name="axis"></param>
        /// <param name="radius"></param>
        /// <param name="num_sides"></param>
        public static void AddSmoothCylinder(this MeshGeometry3D mesh, Point3D end_point, Vector3D axis, double radius, int num_sides = 20)
        {
            // Get two vectors perpendicular to the axis.
            Vector3D v1;
            if ((axis.Z < -0.01) || (axis.Z > 0.01))
                v1 = new Vector3D(axis.Z, axis.Z, -axis.X - axis.Y);
            else
                v1 = new Vector3D(-axis.Y - axis.Z, axis.X, axis.X);
            Vector3D v2 = Vector3D.CrossProduct(v1, axis);

            // Make the vectors have length radius.
            v1 *= (radius / v1.Length);
            v2 *= (radius / v2.Length);

            // Make the top end cap.
            // Make the end point.
            int pt0 = mesh.Positions.Count; // Index of end_point.
            mesh.Positions.Add(end_point);

            // Make the top points.
            double theta = 0;
            double dtheta = 2 * Math.PI / num_sides;
            for (int i = 0; i < num_sides; i++)
            {
                mesh.Positions.Add(end_point +
                    Math.Cos(theta) * v1 +
                    Math.Sin(theta) * v2);
                theta += dtheta;
            }

            // Make the top triangles.
            int pt1 = mesh.Positions.Count - 1; // Index of last point.
            int pt2 = pt0 + 1;                  // Index of first point in this cap.
            for (int i = 0; i < num_sides; i++)
            {
                mesh.TriangleIndices.Add(pt0);
                mesh.TriangleIndices.Add(pt1);
                mesh.TriangleIndices.Add(pt2);
                pt1 = pt2++;
            }

            // Make the bottom end cap.
            // Make the end point.
            pt0 = mesh.Positions.Count; // Index of end_point2.
            Point3D end_point2 = end_point + axis;
            mesh.Positions.Add(end_point2);

            // Make the bottom points.
            theta = 0;
            for (int i = 0; i < num_sides; i++)
            {
                mesh.Positions.Add(end_point2 +
                    Math.Cos(theta) * v1 +
                    Math.Sin(theta) * v2);
                theta += dtheta;
            }

            // Make the bottom triangles.
            pt1 = mesh.Positions.Count - 1; // Index of last point.
            pt2 = pt0 + 1;                  // Index of first point in this cap.
            for (int i = 0; i < num_sides; i++)
            {
                mesh.TriangleIndices.Add(num_sides + 1);    // end_point2
                mesh.TriangleIndices.Add(pt2);
                mesh.TriangleIndices.Add(pt1);
                pt1 = pt2++;
            }

            // Make the sides.
            // Add the points to the mesh.
            int first_side_point = mesh.Positions.Count;
            theta = 0;
            for (int i = 0; i < num_sides; i++)
            {
                Point3D p1 = end_point +
                    Math.Cos(theta) * v1 +
                    Math.Sin(theta) * v2;
                mesh.Positions.Add(p1);
                Point3D p2 = p1 + axis;
                mesh.Positions.Add(p2);
                theta += dtheta;
            }

            // Make the side triangles.
            pt1 = mesh.Positions.Count - 2;
            pt2 = pt1 + 1;
            int pt3 = first_side_point;
            int pt4 = pt3 + 1;
            for (int i = 0; i < num_sides; i++)
            {
                mesh.TriangleIndices.Add(pt1);
                mesh.TriangleIndices.Add(pt2);
                mesh.TriangleIndices.Add(pt4);

                mesh.TriangleIndices.Add(pt1);
                mesh.TriangleIndices.Add(pt4);
                mesh.TriangleIndices.Add(pt3);

                pt1 = pt3;
                pt3 += 2;
                pt2 = pt4;
                pt4 += 2;
            }
        }

        public static void AddSmoothPyramid(this MeshGeometry3D mesh, Point3D point, Vector3D axis, double radius, int side, int position)
        {
            //Create a collection of vertext positions
            Point3DCollection myPositionCollection = new Point3DCollection();
            myPositionCollection.Add(new Point3D(0, -1.0 * axis.Length, 0));
            if (side == MeshCreator.RightSide)
            {
                if (position == 1)
                {
                    myPositionCollection.Add(new Point3D((-1) * radius, 0, 0));
                    myPositionCollection.Add(new Point3D(radius * Math.Sin(Math.PI / 6), 0, radius * Math.Cos(Math.PI / 6)));
                    myPositionCollection.Add(new Point3D(radius * Math.Sin(Math.PI / 6), 0, (-1) * radius * Math.Cos(Math.PI / 6)));
                }
                else
                {
                    myPositionCollection.Add(new Point3D(0, 0, (-1) * radius));
                    myPositionCollection.Add(new Point3D((-1) * radius * Math.Cos(Math.PI / 6), 0, radius * Math.Sin(Math.PI / 6)));
                    myPositionCollection.Add(new Point3D(radius * Math.Cos(Math.PI / 6), 0, radius * Math.Sin(Math.PI / 6)));
                }
            }
            else
            {
                if (position == 1)
                {
                    myPositionCollection.Add(new Point3D(radius, 0, 0));
                    myPositionCollection.Add(new Point3D((-1) * radius * Math.Sin(Math.PI / 6), 0, (-1) * radius * Math.Cos(Math.PI / 6)));
                    myPositionCollection.Add(new Point3D((-1) * radius * Math.Sin(Math.PI / 6), 0, radius * Math.Cos(Math.PI / 6)));
                }
                else
                {
                    myPositionCollection.Add(new Point3D(0, 0, radius));
                    myPositionCollection.Add(new Point3D(radius * Math.Cos(Math.PI / 6), 0, (-1) * radius * Math.Sin(Math.PI / 6)));
                    myPositionCollection.Add(new Point3D((-1) * radius * Math.Cos(Math.PI / 6), 0, (-1) * radius * Math.Sin(Math.PI / 6)));
                }
            }
            // Create a collection of triangle indices
            Int32Collection myTriangleIndicesCollection = new Int32Collection();
            // Triangle
            myTriangleIndicesCollection.Add(0);
            myTriangleIndicesCollection.Add(2);
            myTriangleIndicesCollection.Add(1);
            // Triangle
            myTriangleIndicesCollection.Add(0);
            myTriangleIndicesCollection.Add(1);
            myTriangleIndicesCollection.Add(3);
            // Triangle
            myTriangleIndicesCollection.Add(0);
            myTriangleIndicesCollection.Add(3);
            myTriangleIndicesCollection.Add(2);
            // Triangle
            myTriangleIndicesCollection.Add(1);
            myTriangleIndicesCollection.Add(2);
            myTriangleIndicesCollection.Add(3);
            mesh.Positions = myPositionCollection;
            mesh.TriangleIndices = myTriangleIndicesCollection;

            var transformTranslate = new TranslateTransform3D(point - new Point3D(0, 0, 0));
            mesh.ApplyTransformation(transformTranslate);
        }

        public static MeshGeometry3D ConvertToWireframe(this MeshGeometry3D mesh, double thickness)
        {
            MeshGeometry3D wireframeMesh = null;
            Dictionary<int, int> dictDrawnSegment = new Dictionary<int, int>();
            if (dictDrawnSegment != null)
            {
                wireframeMesh = new MeshGeometry3D();
                for (int triangle = 0; triangle < mesh.TriangleIndices.Count; triangle += 3)
                {
                    int index1 = mesh.TriangleIndices[triangle];
                    int index2 = mesh.TriangleIndices[triangle + 1];
                    int index3 = mesh.TriangleIndices[triangle + 2];
                    AddTriangleSegment(mesh, wireframeMesh, dictDrawnSegment, index1, index2, thickness);
                    AddTriangleSegment(mesh, wireframeMesh, dictDrawnSegment, index2, index3, thickness);
                    AddTriangleSegment(mesh, wireframeMesh, dictDrawnSegment, index3, index1, thickness);
                }
            }
            return wireframeMesh;
        }

        private static void AddTriangleSegment(MeshGeometry3D mesh, MeshGeometry3D wireframe, Dictionary<int, int> dictDrawnSegment, int index1, int index2, double thickness)
        {
            if (index1 > index2)
            {
                int temp = index1;
                index1 = index2;
                index2 = temp;
            }
            int segmentId = index1 * mesh.Positions.Count + index2;
            if (dictDrawnSegment.ContainsKey(segmentId))
                return;
            dictDrawnSegment.Add(segmentId, segmentId);
            AddSegment(wireframe, mesh.Positions[index1], mesh.Positions[index2], thickness);
        }

        public static void AddSegment(MeshGeometry3D mesh,
            Point3D point1, Point3D point2, double thickness, bool extend)
        {
            // Find an up vector that is not colinear with the segment.
            // Start with a vector parallel to the Y axis.
            Vector3D up = new Vector3D(0, 1, 0);

            // If the segment and up vector point in more or less the
            // same direction, use an up vector parallel to the X axis.
            Vector3D segment = point2 - point1;
            segment.Normalize();
            if (Math.Abs(Vector3D.DotProduct(up, segment)) > 0.9)
                up = new Vector3D(1, 0, 0);

            // Add the segment.
            AddSegment(mesh, point1, point2, up, thickness, extend);
        }
        public static void AddSegment(MeshGeometry3D mesh,
            Point3D point1, Point3D point2, double thickness)
        {
            AddSegment(mesh, point1, point2, thickness, false);
        }
        public static void AddSegment(MeshGeometry3D mesh,
            Point3D point1, Point3D point2, Vector3D up, double thickness)
        {
            AddSegment(mesh, point1, point2, up, thickness, false);
        }
        public static void AddSegment(MeshGeometry3D mesh,
            Point3D point1, Point3D point2, Vector3D up, double thickness,
            bool extend)
        {
            // Get the segment's vector.
            Vector3D v = point2 - point1;

            if (extend)
            {
                // Increase the segment's length on both ends by thickness / 2.
                Vector3D n = ScaleVector(v, thickness / 2.0);
                point1 -= n;
                point2 += n;
            }

            // Get the scaled up vector.
            Vector3D n1 = ScaleVector(up, thickness / 2.0);

            // Get another scaled perpendicular vector.
            Vector3D n2 = Vector3D.CrossProduct(v, n1);
            n2 = ScaleVector(n2, thickness / 2.0);

            // Make a skinny box.
            // p1pm means point1 PLUS n1 MINUS n2.
            Point3D p1pp = point1 + n1 + n2;
            Point3D p1mp = point1 - n1 + n2;
            Point3D p1pm = point1 + n1 - n2;
            Point3D p1mm = point1 - n1 - n2;
            Point3D p2pp = point2 + n1 + n2;
            Point3D p2mp = point2 - n1 + n2;
            Point3D p2pm = point2 + n1 - n2;
            Point3D p2mm = point2 - n1 - n2;

            // Sides.
            AddTriangle(mesh, p1pp, p1mp, p2mp);
            AddTriangle(mesh, p1pp, p2mp, p2pp);

            AddTriangle(mesh, p1pp, p2pp, p2pm);
            AddTriangle(mesh, p1pp, p2pm, p1pm);

            AddTriangle(mesh, p1pm, p2pm, p2mm);
            AddTriangle(mesh, p1pm, p2mm, p1mm);

            AddTriangle(mesh, p1mm, p2mm, p2mp);
            AddTriangle(mesh, p1mm, p2mp, p1mp);

            // Ends.
            AddTriangle(mesh, p1pp, p1pm, p1mm);
            AddTriangle(mesh, p1pp, p1mm, p1mp);

            AddTriangle(mesh, p2pp, p2mp, p2mm);
            AddTriangle(mesh, p2pp, p2mm, p2pm);
        }

        public static void AddSmoothCone(this MeshGeometry3D mesh, Point3D point, Vector3D axis, double radius1, double radius2, int num_sides)
        {
            Vector3D top_v1;
            if ((axis.Z < -0.01) || (axis.Z > 0.01))
                top_v1 = new Vector3D(axis.Z, axis.Z, -axis.X - axis.Y);
            else
                top_v1 = new Vector3D(-axis.Y - axis.Z, axis.X, axis.X);
            Vector3D top_v2 = Vector3D.CrossProduct(top_v1, axis);
            Vector3D bot_v1 = top_v1;
            Vector3D bot_v2 = top_v2;

            // Make the vectors have length radius.
            top_v1 *= (radius1 / top_v1.Length);
            top_v2 *= (radius1 / top_v2.Length);

            bot_v1 *= (radius2 / bot_v1.Length);
            bot_v2 *= (radius2 / bot_v2.Length);

            // Make the top end cap.
            double theta = 0;
            double dtheta = 2 * Math.PI / num_sides;
            for (int i = 0; i < num_sides; i++)
            {
                Point3D p1 = point +
                    Math.Cos(theta) * top_v1 +
                    Math.Sin(theta) * top_v2;
                theta += dtheta;
                Point3D p2 = point +
                    Math.Cos(theta) * top_v1 +
                    Math.Sin(theta) * top_v2;
                AddTriangle(mesh, point, p1, p2);
            }

            // Make the bottom end cap.
            Point3D end_point2 = point + axis;
            theta = 0;
            for (int i = 0; i < num_sides; i++)
            {
                Point3D p1 = end_point2 +
                    Math.Cos(theta) * bot_v1 +
                    Math.Sin(theta) * bot_v2;
                theta += dtheta;
                Point3D p2 = end_point2 +
                    Math.Cos(theta) * bot_v1 +
                    Math.Sin(theta) * bot_v2;
                AddTriangle(mesh, end_point2, p2, p1);
            }

            // Make the sides.
            theta = 0;
            for (int i = 0; i < num_sides; i++)
            {
                Point3D p1 = point +
                    Math.Cos(theta) * top_v1 +
                    Math.Sin(theta) * top_v2;
                Point3D p3 = point + axis +
                    Math.Cos(theta) * bot_v1 +
                    Math.Sin(theta) * bot_v2;
                theta += dtheta;
                Point3D p2 = point +
                    Math.Cos(theta) * top_v1 +
                    Math.Sin(theta) * top_v2;
                Point3D p4 = point + axis +
                    Math.Cos(theta) * bot_v1 +
                    Math.Sin(theta) * bot_v2;

                AddTriangle(mesh, p1, p3, p2);
                AddTriangle(mesh, p2, p3, p4);
            }
        }

        // Add a triangle to the indicated mesh.
        // Reuse points so triangles share normals.
        private static void AddSmoothTriangle(this MeshGeometry3D mesh, Dictionary<Point3D, int> dict, Point3D point1, Point3D point2, Point3D point3)
        {
            int index1, index2, index3;

            // Find or create the points.
            if (dict.ContainsKey(point1)) index1 = dict[point1];
            else
            {
                index1 = mesh.Positions.Count;
                mesh.Positions.Add(point1);
                dict.Add(point1, index1);
            }

            if (dict.ContainsKey(point2)) index2 = dict[point2];
            else
            {
                index2 = mesh.Positions.Count;
                mesh.Positions.Add(point2);
                dict.Add(point2, index2);
            }

            if (dict.ContainsKey(point3)) index3 = dict[point3];
            else
            {
                index3 = mesh.Positions.Count;
                mesh.Positions.Add(point3);
                dict.Add(point3, index3);
            }

            // If two or more of the points are
            // the same, it's not a triangle.
            if ((index1 == index2) ||
                (index2 == index3) ||
                (index3 == index1)) return;

            // Create the triangle.
            mesh.TriangleIndices.Add(index1);
            mesh.TriangleIndices.Add(index2);
            mesh.TriangleIndices.Add(index3);
        }

        // Add a sphere.
        public static void AddSmoothSphere(this MeshGeometry3D mesh, Point3D center, double radius, int num_phi, int num_theta)
        {
            // Make a dictionary to track the sphere's points.
            Dictionary<Point3D, int> dict = new Dictionary<Point3D, int>();

            double phi0, theta0;
            double dphi = Math.PI / num_phi;
            double dtheta = 2 * Math.PI / num_theta;

            phi0 = 0;
            double y0 = radius * Math.Cos(phi0);
            double r0 = radius * Math.Sin(phi0);
            for (int i = 0; i < num_phi; i++)
            {
                double phi1 = phi0 + dphi;
                double y1 = radius * Math.Cos(phi1);
                double r1 = radius * Math.Sin(phi1);

                // Point ptAB has phi value A and theta value B.
                // For example, pt01 has phi = phi0 and theta = theta1.
                // Find the points with theta = theta0.
                theta0 = 0;
                Point3D pt00 = new Point3D(
                    center.X + r0 * Math.Cos(theta0),
                    center.Y + y0,
                    center.Z + r0 * Math.Sin(theta0));
                Point3D pt10 = new Point3D(
                    center.X + r1 * Math.Cos(theta0),
                    center.Y + y1,
                    center.Z + r1 * Math.Sin(theta0));
                for (int j = 0; j < num_theta; j++)
                {
                    // Find the points with theta = theta1.
                    double theta1 = theta0 + dtheta;
                    Point3D pt01 = new Point3D(
                        center.X + r0 * Math.Cos(theta1),
                        center.Y + y0,
                        center.Z + r0 * Math.Sin(theta1));
                    Point3D pt11 = new Point3D(
                        center.X + r1 * Math.Cos(theta1),
                        center.Y + y1,
                        center.Z + r1 * Math.Sin(theta1));

                    // Create the triangles.
                    AddSmoothTriangle(mesh, dict, pt00, pt11, pt10);
                    AddSmoothTriangle(mesh, dict, pt00, pt01, pt11);

                    // Move to the next value of theta.
                    theta0 = theta1;
                    pt00 = pt01;
                    pt10 = pt11;
                }

                // Move to the next value of phi.
                phi0 = phi1;
                y0 = y1;
                r0 = r1;
            }
        }

        [SuppressMessage("Major Code Smell", "S125:Sections of code should not be commented out", Justification = "<Pending>")]
        public static void AddSmoothPlane(this MeshGeometry3D mesh, Point3D startPoint, Point3D endPoint, double adjustHeight, double thickness = 0.01)
        {
            Point3D p1 = new Point3D(startPoint.X, startPoint.Y, startPoint.Z);
            Point3D p2 = new Point3D(startPoint.X, (-1)*startPoint.Y + adjustHeight, startPoint.Z);
            Point3D p3 = new Point3D(endPoint.X, (-1)*startPoint.Y + adjustHeight, endPoint.Z);
            Point3D p4 = new Point3D(endPoint.X, endPoint.Y, endPoint.Z);

            Vector3D v = endPoint - startPoint;

            // Get normal vector
            var n = new Vector3D(-v.Z, 0, v.X);
            n = ScaleVector(n, thickness / 2.0);
            //var angle = Vector3D.AngleBetween(v, n);

            // Make a skinny plane.
            Point3D p11 = p1 + n;
            Point3D p12 = p1 - n;
            //Point3D p21 = p2 + n;
            //Point3D p22 = p2 - n;
            //Point3D p31 = p3 + n;
            //Point3D p32 = p3 - n;
            Point3D p41 = p4 + n;
            Point3D p42 = p4 - n;

            mesh.DrawRectangle(p1, p2, p3, p4);
            mesh.DrawRectangle(p12, p11, p41, p42);
            //mesh.DrawRectangle(p11, p21, p31, p41);
            //mesh.DrawRectangle(p41, p31, p32, p42);
            //mesh.DrawRectangle(p42, p32, p22, p12);
            //mesh.DrawRectangle(p12, p22, p21, p11);
            //mesh.DrawRectangle(p12, p22, p31, p41);
        }

        public static void AddBasePlane(this MeshGeometry3D mesh, Point3D point1, Point3D point2, Point3D point3, Point3D point4, double thickness = 0.01)
        {
            Vector3D vNormal = new Vector3D(0, 1, 0);
            var vNormalScale = ScaleVector(vNormal, (-1)*thickness);
            Point3D point12 = point1 + vNormalScale;
            Point3D point22 = point2 + vNormalScale;
            Point3D point32 = point3 + vNormalScale;
            Point3D point42 = point4 + vNormalScale;
            mesh.DrawRectangle(point1, point2, point3, point4);
            mesh.DrawRectangle(point12, point22, point32, point42);
        }

        /// <summary>
        /// Make a thin rectangular prism between the two points.
        /// If extend is true, extend the segment by half the
        /// thickness so segments with the same end points meet nicely.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="up"></param>
        /// <param name="extend"></param>
        public static void AddSmoothLine(this MeshGeometry3D mesh,
            Point3D point1, Point3D point2, Vector3D up, double thickness = 1,
            bool extend = false)
        {
            // Get the segment's vector.
            Vector3D v = point2 - point1;

            if (extend)
            {
                // Increase the segment's length on both ends by thickness / 2.
                Vector3D n = ScaleVector(v, thickness / 2.0);
                point1 -= n;
                point2 += n;
            }

            // Get the scaled up vector.
            Vector3D n1 = ScaleVector(up, thickness / 2.0);

            // Get another scaled perpendicular vector.
            Vector3D n2 = Vector3D.CrossProduct(v, n1);
            n2 = ScaleVector(n2, thickness / 2.0);

            // Make a skinny box.
            // p1pm means point1 PLUS n1 MINUS n2.
            Point3D p1pp = point1 + n1 + n2;
            Point3D p1mp = point1 - n1 + n2;
            Point3D p1pm = point1 + n1 - n2;
            Point3D p1mm = point1 - n1 - n2;
            Point3D p2pp = point2 + n1 + n2;
            Point3D p2mp = point2 - n1 + n2;
            Point3D p2pm = point2 + n1 - n2;
            Point3D p2mm = point2 - n1 - n2;

            // Sides.
            AddTriangle(mesh, p1pp, p1mp, p2mp);
            AddTriangle(mesh, p1pp, p2mp, p2pp);

            AddTriangle(mesh, p1pp, p2pp, p2pm);
            AddTriangle(mesh, p1pp, p2pm, p1pm);

            AddTriangle(mesh, p1pm, p2pm, p2mm);
            AddTriangle(mesh, p1pm, p2mm, p1mm);

            AddTriangle(mesh, p1mm, p2mm, p2mp);
            AddTriangle(mesh, p1mm, p2mp, p1mp);

            // Ends.
            AddTriangle(mesh, p1pp, p1pm, p1mm);
            AddTriangle(mesh, p1pp, p1mm, p1mp);

            AddTriangle(mesh, p2pp, p2mp, p2mm);
            AddTriangle(mesh, p2pp, p2mm, p2pm);
        }

        /// <summary>
        /// Draw a rectangle on the indicated mesh. The points must be outwardly oriented (0,1,2 right hand rule; 0, 2, 3)
        /// First triangle			   \\\ Second triangle
        ///   0 \						     0....3
        ///   |  \						      \   |
        ///   1....2				           \  2
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="rect"></param>
        public static void DrawRectangle(this MeshGeometry3D mesh, Rectangle3D rect)
        {
            // [NCS-2695] CID 171191 Dereference null return (stat)
            //// Create the rectangle's triangles
            //AddTriangle(mesh, rect.Points[0], rect.Points[1], rect.Points[2]);
            //mesh.TextureCoordinates.Add(new Point(0, 0));
            //mesh.TextureCoordinates.Add(new Point(0, 1));
            //mesh.TextureCoordinates.Add(new Point(1, 1));
            //AddTriangle(mesh, rect.Points[0], rect.Points[2], rect.Points[3]);
            //mesh.TextureCoordinates.Add(new Point(0, 0));
            //mesh.TextureCoordinates.Add(new Point(1, 1));
            //mesh.TextureCoordinates.Add(new Point(1, 0));
            // Create the rectangle's triangles
            AddTriangle(mesh, rect.Points[0], rect.Points[1], rect.Points[2]);
            if (mesh.TextureCoordinates != null)
            {
                mesh.TextureCoordinates.Add(new Point(0, 0));
                mesh.TextureCoordinates.Add(new Point(0, 1));
                mesh.TextureCoordinates.Add(new Point(1, 1));
            }

            AddTriangle(mesh, rect.Points[0], rect.Points[2], rect.Points[3]);
            if (mesh.TextureCoordinates != null)
            {
                mesh.TextureCoordinates.Add(new Point(0, 0));
                mesh.TextureCoordinates.Add(new Point(1, 1));
                mesh.TextureCoordinates.Add(new Point(1, 0));
            }
        }

        public static void DrawRectangle(this MeshGeometry3D mesh, Point3D point1, Point3D point2, Point3D point3, Point3D point4)
        {
            DrawRectangle(mesh, new Rectangle3D(point1, point2, point3, point4));
        }

        /// <summary>
        /// Add a triangle to the indicated mesh; The points must be outwardly oriented
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="point3"></param>
        public static void AddTriangle(this MeshGeometry3D mesh, Point3D point1, Point3D point2, Point3D point3)
        {
            // Create the points.
            int index = mesh.Positions.Count;
            mesh.Positions.Add(point1);
            mesh.Positions.Add(point2);
            mesh.Positions.Add(point3);

            // Create the triangle.
            mesh.TriangleIndices.Add(index++);
            mesh.TriangleIndices.Add(index++);
            mesh.TriangleIndices.Add(index);
        }

        public static void AddTextureCoords(this MeshGeometry3D mesh, double value, double min_value, double max_value)
        {
            // Convert into a value between 0 and 1.
            double y = 1 - ((value - min_value) / (max_value - min_value));

            // [NCS-2695] CID 171157 Dereference null return (stat)
            //mesh.TextureCoordinates.Add(new Point(0, y));
            mesh.TextureCoordinates?.Add(new Point(0, y));
        }

        // Set the vector's length.
        private static Vector3D ScaleVector(Vector3D vector, double length)
        {
            double scale = length / vector.Length;
            return new Vector3D(
                vector.X * scale,
                vector.Y * scale,
                vector.Z * scale);
        }
    }

    public class Rectangle3D
    {
        // The rectangle's approximate points.
        public Point3D[] Points { get; set; }

        // Initializing constructor.
        public Rectangle3D(Point3D point1, Point3D point2, Point3D point3, Point3D point4)
        {
            // Save the points.
            Points = new Point3D[]
            {
                point1,
                point2,
                point3,
                point4,
            };
        }
    }

}