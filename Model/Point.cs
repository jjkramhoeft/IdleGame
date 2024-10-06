namespace Model
{
    /// <summary>
    /// Map location.
    /// The unit is meters.
    /// Only integer precision.
    /// </summary>
    public class Point(long longitude, long latitude)
    {
        /// <summary>
        /// Longitude ~ x. West -> East  is positive.
        /// </summary>
        public long Longitude { get; set; } = longitude;

        /// <summary>
        /// Latitude ~ y. South -> North is positive.
        /// </summary>
        public long Latitude { get; set; } = latitude;

        /// <summary>
        /// Test for same position
        /// </summary>
        public bool Equals(Point testPoint)
        {
            if (Longitude == testPoint.Longitude && Latitude == testPoint.Latitude)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Real distiance
        /// </summary>
        public double Dist(Point point)
        {
            return Math.Sqrt(
                (point.Longitude - Longitude) * (double)(point.Longitude - Longitude) +
                (point.Latitude - Latitude) * (double)(point.Latitude - Latitude));
        }

        /// <summary>
        /// Distance ^ 2
        /// </summary>
        public double Dist2(Point point)
        {
            return
                (point.Longitude - Longitude) * (double)(point.Longitude - Longitude) +
                (point.Latitude - Latitude) * (double)(point.Latitude - Latitude);
        }
    }
}