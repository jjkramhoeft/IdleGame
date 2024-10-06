namespace Model
{
    /// <summary>
    /// A named wind direction
    /// </summary>
    public class Direction(int id)
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; } = id;

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public double LatitudePart { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public double LongitudePart { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public double AngleInRadians { get; set; } = 0;
    }
}