namespace Model
{
    /// <summary>
    /// Map line given by two points
    /// </summary>
    public class Line
    {
        /// <summary>
        /// Construct a map line by two points
        /// </summary>
        public Line(Point point1, Point point2)
        {
            if (point1.Equals(point2))
                throw new ArgumentException("A line can not be declared from to identical points!");
            Point1 = point1;
            Point2 = point2;
        }

        /// <summary>
        /// First point
        /// </summary>
        public Point Point1 { get; set; }

        /// <summary>
        /// Second point
        /// </summary>
        public Point Point2 { get; set; }

        /// <summary>
        /// Calculate intersection with a other line
        /// </summary>
        /// <returns>Intersection point or null if lines are parallel </returns>
        public Point? GetIntersection(Line otherLine)
        {
            if (Determinanten(otherLine) == 0)
                return null;
            double? alphaThis = Alpha();
            double? betaThis = Beta();
            double? alphaOther = otherLine.Alpha();
            double? betaOther = otherLine.Beta();
            if (alphaThis is null || betaThis is null || alphaOther is null || betaOther is null)
            {
                if (alphaThis is null && alphaOther is null) // both lines are vertical
                    return null;
                else
                {
                    if (alphaThis is null) // this line is vertical
                    {
                        double x = Point1.Longitude;
                        double y = (double)(alphaOther! * x + betaOther!);
                        return new Point(FastFloor(x), FastFloor(y));
                    }
                    else // other line is vertical
                    {
                        double x = otherLine.Point1.Longitude;
                        double y = (double)(alphaThis! * x + betaThis!);
                        return new Point(FastFloor(x), FastFloor(y));
                    }
                }
            }
            else
            {
                double x = (double)((betaOther - betaThis) / (alphaThis - alphaOther));
                double y = (double)(alphaThis * x + betaThis);
                double y2 = (double)(alphaOther * x + betaOther);
                Point res = new Point(FastFloor(x), FastFloor(y));
                return res;
            }
        }

        /// <summary>
        /// Calculate slope of line
        /// </summary>
        public double? Alpha()
        {
            long div = Point2.Longitude - Point1.Longitude;
            if (div == 0)
                return null;
            double tael = Point2.Latitude - Point1.Latitude;
            return tael / (double)div;
        }

        /// <summary>
        /// Calculate intersection with y-axis
        /// </summary>
        public double? Beta()
        {
            double? alpha = Alpha();
            if (alpha is null)
                return null;
            return Point1.Latitude - (double)alpha * Point1.Longitude;
        }

        /// <summary>
        /// Calculate a line bisecting(halfing) this line in an 90 degre angle. The claculated line start in the intersection and be 10.000 meters long
        /// </summary>
        public Line GetPerpendicularBisector10000Line()
        {
            Point bisectPoint = new((Point1.Longitude + Point2.Longitude) / 2L, (Point1.Latitude + Point2.Latitude) / 2L);
            long deltaX = bisectPoint.Longitude - Point1.Longitude;
            long deltaY = bisectPoint.Latitude - Point1.Latitude;
            double deltaLength = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            double factor = 10000.0 / deltaLength;
            long delta1000X = FastFloor(deltaX * factor);
            long delta1000Y = FastFloor(deltaY * factor);
            Point perpendicular = new(bisectPoint.Longitude + delta1000Y, bisectPoint.Latitude - delta1000X);
            Line perpendicularbisector = new(bisectPoint, perpendicular);
            return perpendicularbisector;
        }

        /// <summary>
        /// Calculate the determinant of two lines. Zero if parallel
        /// </summary>
        public long Determinanten(Line otherLine)
        {
            long ax = Point2.Longitude - Point1.Longitude;
            long ay = Point2.Latitude - Point1.Latitude;
            long bx = otherLine.Point2.Longitude - otherLine.Point1.Longitude;
            long by = otherLine.Point2.Latitude - otherLine.Point1.Latitude;
            return ax * by - ay * bx;
        }

        private static int FastFloorInt(double x)
        {
            var xi = (int)x;
            return x < xi ? xi - 1 : xi;
        }

        private static long FastFloor(double x)
        {
            var xi = (long)x;
            return x < xi ? xi - 1 : xi;
        }
    }
}