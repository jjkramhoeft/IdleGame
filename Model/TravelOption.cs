namespace Model
{
    /// <summary>
    /// 
    /// </summary>
    public class TravelOption(Point a, Point b, List<TravelMode> travelModes, Direction direction, int distance, bool hasPreviouslyVisitedB)
    {
        /// <summary>
        /// Departure location point
        /// </summary>
        public Point PointA { get; set; } = a;

        /// <summary>
        /// Departure location box
        /// </summary>
        public Box BoxA { get; } = Box.GetLocationBox(a);

        /// <summary>
        /// Arrival location point
        /// </summary>
        public Point PointB { get; set; } = b;

        /// <summary>
        /// 
        /// </summary>
        public bool HasPreviouslyVisitedB { get; set; } = hasPreviouslyVisitedB;

        /// <summary>
        /// Arrival location box
        /// </summary>
        public Box BoxB { get; } = Box.GetLocationBox(b);

        /// <summary>
        /// Mode of travel
        /// </summary>
        public List<TravelMode> TravelModes { get; } = travelModes;

        /// <summary>
        /// Travel direction
        /// </summary>
        public Direction Direction { get; } = direction;

        /// <summary>
        /// Travel distance in meters
        /// </summary>
        public int Distance { get; } = distance;

        /// <summary>
        /// Travel distance in meters
        /// </summary>
        public Uri DepartAction { get; set; } = new("http://localhost:5278/");
    }
}