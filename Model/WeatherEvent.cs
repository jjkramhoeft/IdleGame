namespace Model
{
    /// <summary>
    /// 
    /// </summary>
    public class WeatherEvent(int id)
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; } = id;

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; } = "";
    }
}