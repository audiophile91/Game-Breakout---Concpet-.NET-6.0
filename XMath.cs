using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using System.Windows.Navigation;

namespace Breakout
{
    public static class XMath
    {
        private static readonly Random Random = new();

        /// <summary>
        /// Returns if tested value exceeds given range.
        /// </summary>
        /// <param name="tested"></param>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        /// <returns></returns>
        public static bool Exceeds(double tested, double minimum, double maximum) => tested < minimum || tested > maximum;

        /// <summary>
        /// Returns if tested value
        /// </summary>
        /// <param name="tested"></param>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static bool EqualsOr(double tested, double value1, double value2) => tested == value1 || tested == value2;

        /// <summary>
        /// Return if tested valued is within given range
        /// </summary>
        /// <param name="tested"></param>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        /// <returns></returns>
        public static bool InBoundaries(double tested, double minimum, double maximum) => tested >= minimum && tested <= maximum;

        public static bool DoesSegmentIntersect(double start, double delta, double boundary)
        {
            return (start <= boundary && start + delta >= boundary) || (start >= boundary && start + delta <= boundary);
        }

        public static double GetDirectionalCoefficient(Point pt1, Point pt2)
        {
            if (pt2.X - pt1.X == 0)
            {
                // does not exist
                return double.NaN;
            }
            return (pt2.Y - pt1.Y) / (pt2.X - pt1.X);
        }
        public static double GetValueRangePercent(double tested, double min, double max)
        {
            double range = max - min;
            double positionInRange = tested - min;

            double percent = (positionInRange / range) * 100;

            return percent;
        }
        public static Point GetCrossPoint(Point pt1, Point pt2, double crossPoint, bool axisX)
        {
            double m = GetDirectionalCoefficient(pt1, pt2);

            if (double.IsNaN(m))
            {
                return axisX ? new Point(pt1.X, crossPoint) : new Point(crossPoint, pt1.Y);
            }

            double b = pt1.Y - m * pt1.X;

            if (axisX)
            {
                return new Point((crossPoint - b) / m, crossPoint);
            }
            else
            {
                return new Point(crossPoint, m * crossPoint + b);
            }
        }

        public static double GetAngleInDegrees(Vector2 vector)
        {
            var angleRadians = Math.Atan2(vector.Y, vector.X);

            var angleDegrees = angleRadians * 180 / Math.PI;

            angleDegrees = (angleDegrees + 360) % 360;

            return angleDegrees;
        }
        public static Vector2 GetOffsetVector2(Vector2 vector, double angleOffsetDegrees)
        {
            double radians = angleOffsetDegrees * Math.PI / 180;

            float x = (float)(vector.X * Math.Cos(radians) - vector.Y * Math.Sin(radians));
            float y = (float)(vector.X * Math.Sin(radians) + vector.Y * Math.Cos(radians));

            return new Vector2(x, y);
        }

        public static Vector2 SetVectorAngle(Vector2 vector, double targetAngleDegrees)
        {
            double currentAngleRadians = Math.Atan2(vector.Y, vector.X);
            double targetAngleRadians = targetAngleDegrees * Math.PI / 180;

            double magnitude = vector.Length();

            float x = (float)(magnitude * Math.Cos(targetAngleRadians));
            float y = (float)(magnitude * Math.Sin(targetAngleRadians));

            return new Vector2(x, y);
        }

        public static Vector2 GetRandomVector2(double vectorLength)
        {
            float angle = (float)(Random.NextDouble() * 2 * Math.PI);

            var x = (float)(vectorLength * Math.Cos(angle));
            var y = (float)(vectorLength * Math.Sin(angle));

            return new Vector2(x, y);
        }

        public static Vector2 GetRandomOffsetVector2(Vector2 vector, double maxAngleOffset)
        {
            var convertAngle = (int)(maxAngleOffset % 360 * 100);

            var offset = Random.Next(-convertAngle, convertAngle + 1) / 100;

            var radians = offset * Math.PI / 180;

            var x = (float)(vector.X * Math.Cos(radians) - vector.Y * Math.Sin(radians));
            var y = (float)(vector.X * Math.Sin(radians) + vector.Y * Math.Cos(radians));

            return new Vector2(x, y);
        }
    }
}
