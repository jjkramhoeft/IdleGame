using System.Text.Json.Serialization;

namespace Model
{
    /// <summary>
    /// A Named climate specific for a single location
    /// </summary>
    public class Climate()
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public Direction? PredominantWindDirection { get; set; }

        /// <summary>
        /// 0-1
        /// </summary>
        public double StormFrequency { get; set; }

        /// <summary>
        /// 0-4
        /// </summary>
        public double PrecipitationAmount { get; set; }

        /// <summary>
        /// Celcius
        /// </summary>
        public double AverageTemperature { get; set; }

        /// <summary>
        /// Dictionary of named weather events for this climate
        /// </summary>
        [JsonIgnore]
        public Dictionary<WeatherEvent, double> WeatherEventWeights { get; set; } = [];
    }
}