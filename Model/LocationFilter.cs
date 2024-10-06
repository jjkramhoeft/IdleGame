namespace Model
{
    /// <summary>
    /// A set of world values that limits to fitting locations
    /// </summary>
    public class LocationFilter
    {
        /// <summary>
        /// Lowlands
        /// </summary>
        public float? MaxHeight { get; set; }

        /// <summary>
        /// Mountains
        /// </summary>
        public float? MinHeight { get; set; }

        /// <summary>
        /// Cold things
        /// </summary>
        public float? MaxTemperature { get; set; }

        /// <summary>
        /// Hot things
        /// </summary>
        public float? MinTemperature { get; set; }

        /// <summary>
        /// Dry
        /// </summary>
        public float? MaxPrecipitation { get; set; }

        /// <summary>
        /// Wet
        /// </summary>
        public float? MinPrecipitation { get; set; }

        /// <summary>
        /// Only on clifs
        /// </summary>
        public float? MaxSlopeValue { get; set; }

        /// <summary>
        /// Room to build 
        /// </summary>
        public float? MinSlopeValue { get; set; }

        /// <summary>
        /// A queit place
        /// </summary>
        public float? MaxStormFrequency { get; set; }

        /// <summary>
        /// Sorring winds
        /// </summary>
        public float? MinStormFrequency { get; set; }

        /// <summary>
        /// Starter content
        /// </summary>
        public double? MaxDistanceFromOrigin { get; set; }

        /// <summary>
        /// High Level content
        /// </summary>
        public double? MinDistanceFromOrigin { get; set; }

        /// <summary>
        /// Geological nessesity 
        /// </summary>
        public GeologyId? NeededGeologyId { get; set; }

        /// <summary>
        /// If present weight is doubled 
        /// </summary>
        public GeologyId? BoostingGeologyId { get; set; }

        /// <summary>
        /// Frequency/likelihood. 0 -> 1
        /// </summary>
        public float Weight { get; set; } = 1f;

        ///      
        public static LocationFilter GetFilter(
            Height.NamedInterval h,
            Temperature.NamedInterval t)
        {
            float hMax = Height.GetIntervalHigh(h);
            float hMin = Height.GetIntervalLow(h);
            float tMax = Temperature.GetIntervalHigh(t);
            float tMin = Temperature.GetIntervalLow(t);

            return new()
            {
                MaxHeight = hMax,
                MinHeight = hMin,
                MaxTemperature = tMax,
                MinTemperature = tMin
            };
        }

        ///   
        public static LocationFilter GetFilter(
            Height.NamedInterval h,
            Temperature.NamedInterval t,
            Precipitation.NamedInterval p,
            float weight)
        {
            float hMax = Height.GetIntervalHigh(h);
            float hMin = Height.GetIntervalLow(h);
            float tMax = Temperature.GetIntervalHigh(t);
            float tMin = Temperature.GetIntervalLow(t);
            float pMax = Precipitation.GetIntervalHigh(p);
            float pMin = Precipitation.GetIntervalLow(p);

            return new()
            {
                MaxHeight = hMax,
                MinHeight = hMin,
                MaxTemperature = tMax,
                MinTemperature = tMin,
                MaxPrecipitation = pMax,
                MinPrecipitation = pMin,
                Weight = weight
            };
        }

        ///     
        public static LocationFilter GetFilter(
            Height.NamedInterval h,
            Temperature.NamedInterval t,
            Precipitation.NamedInterval p,
            Slope.NamedInterval s,
            float weight)
        {
            float hMax = Height.GetIntervalHigh(h);
            float hMin = Height.GetIntervalLow(h);
            float tMax = Temperature.GetIntervalHigh(t);
            float tMin = Temperature.GetIntervalLow(t);
            float pMax = Precipitation.GetIntervalHigh(p);
            float pMin = Precipitation.GetIntervalLow(p);
            float sMax = Slope.GetIntervalHigh(s);
            float sMin = Slope.GetIntervalLow(s);

            return new()
            {
                MaxHeight = hMax,
                MinHeight = hMin,
                MaxTemperature = tMax,
                MinTemperature = tMin,
                MaxPrecipitation = pMax,
                MinPrecipitation = pMin,
                MaxSlopeValue = sMax,
                MinSlopeValue = sMin,
                Weight = weight
            };
        }
    }
}