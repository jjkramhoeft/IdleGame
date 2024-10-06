namespace Model
{
    /// 
    public static class PersonExtension
    {
        /// <summary>
        /// Test all not null elements of the filter, Returs 'True' if the match is OK.
        /// </summary>
        public static bool Match(this Person p, PersonFilter? f, Location l)
        {
            if (f is null)
                return false;

            if (f.MaxDistanceFromOrigin is not null &&
                f.MaxDistanceFromOrigin < l.Point.Dist(new Point(0, 0)))
                return false;

            if (f.MinDistanceFromOrigin is not null &&
                f.MinDistanceFromOrigin > l.Point.Dist(new Point(0, 0)))
                return false;

            if (f.MaxTemperature is not null &&
                f.MaxTemperature < l.Climate!.AverageTemperature)
                return false;

            if (f.MinTemperature is not null &&
                f.MinTemperature > l.Climate!.AverageTemperature)
                return false;

            if (0 < f.Races.Count &&
                !f.Races.Contains(p.RaceId))
                return false;

            if (0 < f.Professions.Count &&
                !f.Professions.Contains(p.ProfessionId))
                return false;

            if (0 < f.Sex.Count &&
                !f.Sex.Contains(p.SexId))
                return false;

            if (0 < f.Age.Count &&
                !f.Age.Contains(p.AgeId))
                return false;

            if (0 < f.Wealth.Count &&
                !f.Wealth.Contains(p.WealthId))
                return false;

            return true;
        }
    }
}