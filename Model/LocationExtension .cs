namespace Model
{
    /// 
    public static class LocationExtension
    {
        /// <summary>
        /// Test all not null elements of the filter, Returs 'True' if the match is OK.
        /// </summary>
        public static bool Match(this Location l, LocationFilter? f)
        {
            if (f is null || l.Climate is null)
                return false;

            if (f.MaxDistanceFromOrigin is not null &&
                f.MaxDistanceFromOrigin < l.Point.Dist(new Point(0, 0)))
                return false;

            if (f.MaxSlopeValue is not null &&
                f.MaxSlopeValue < l.SlopeValue)
                return false;

            if (f.MaxHeight is not null &&
                f.MaxHeight < l.Height)
                return false;

            if (f.MaxPrecipitation is not null &&
                f.MaxPrecipitation < l.Climate.PrecipitationAmount)
                return false;

            if (f.MaxStormFrequency is not null &&
                f.MaxStormFrequency < l.Climate.StormFrequency)
                return false;

            if (f.MaxTemperature is not null &&
                f.MaxTemperature < l.Climate.AverageTemperature)
                return false;

            if (f.MinDistanceFromOrigin is not null &&
                f.MinDistanceFromOrigin > l.Point.Dist(new Point(0, 0)))
                return false;

            if (f.MinSlopeValue is not null &&
                f.MinSlopeValue > l.SlopeValue)
                return false;

            if (f.MinHeight is not null &&
                f.MinHeight > l.Height)
                return false;

            if (f.MinPrecipitation is not null &&
                f.MinPrecipitation > l.Climate.PrecipitationAmount)
                return false;

            if (f.MinStormFrequency is not null &&
                f.MinStormFrequency > l.Climate.StormFrequency)
                return false;

            if (f.MinTemperature is not null &&
                f.MinTemperature > l.Climate.AverageTemperature)
                return false;

            if (f.NeededGeologyId is not null &&
                f.NeededGeologyId != l.GeologyId)
                return false;

            return true;
        }
    }
}