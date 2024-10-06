using Model;

namespace Generator.Locations
{
    public abstract class LocationType
    {
        public abstract LocationTypes GetTypeKey();
        public abstract string GetTypeName();
        public abstract string GetTypeDescription(Location location, Region region);

        /// <summary>
        /// The first part of person init. Names are not included - only placeholder surnames
        /// </summary>
        public abstract Task<List<PersonRequest>> GetPersonRequests(Location l, Region associateRegion);
        public abstract bool IsValidOn(Location location, Region region);
        public abstract Rarity RarityOn(Location location, Region region);

        public static string GetBiomPlacement(Location location)
        {
            if(location.BiomId==BiomIds.None)
                throw new Exception($"No biom to location at box ({location.Box.LowerLeftPoint.Longitude},{location.Box.LowerLeftPoint.Latitude}) height:{location.Height}");
            var b = World.Bioms[location.BiomId];
            if(string.IsNullOrEmpty(b.PlacementName))
                throw new Exception($"No placement name for biom {b.Id} {b.Name}");
            
            return b.PlacementName;
        }

        public static string GetBiomDescription(Location location)
        {
            if(location.BiomId==BiomIds.None)
            {
                Console.WriteLine($"Possible error! No biom to location {location.Id}");
                return "No biom!! Error!";
            }
            
            Random rand = new(location.Box.GetSeed());
            var b = World.Bioms[location.BiomId];
            if (0 < b.Descriptions.Count)
            {
                int index = rand.Next(b.Descriptions.Count);
                return b.Descriptions[index];
            }
            else if (!string.IsNullOrEmpty(b.AltName))
            {
                if (rand.NextDouble() < 0.25)
                    return b.AltName;
                else
                    return b.Name;
            }
            else
                return b.Name;
        }

        public static string GetTemperatureDescription(Location location)
        {
            if (Temperature.HotVeryHotSplit < (location.Climate?.AverageTemperature ?? 10))
                return "it is very hot";
            else if ((location.Climate?.AverageTemperature ?? 10) < Temperature.ColdVeryColdSplit)
                return "it is very cold";
            else if (Temperature.WarmHotSplit < (location.Climate?.AverageTemperature ?? 10))
                return "it is hot";
            else if ((location.Climate?.AverageTemperature ?? 10) < Temperature.NormalColdSplit)
                return "it is cold";
            else if (Temperature.WarmNormalSplit < (location.Climate?.AverageTemperature ?? 10))
                return "";
            return "";
        }

        public static string GetGeologyDescription(Location location)
        {
            if (location.GeologyId == GeologyId.Fertile)
                return "it is a fertile and rich place";
            else if (location.GeologyId == GeologyId.Poor)
                return "it is a barren and poor place";
            else if (location.GeologyId == GeologyId.Magic)
                return "it is a magical place";
            else if (location.GeologyId == GeologyId.Dark)
                return "it is a dark and evil place";
            else if (location.GeologyId == GeologyId.Old)
                return "it is a very old place";
            return "";
        }

        public static string JoinPromt(params string?[] values)
        {
            HashSet<string> parts = [];
            foreach (var v in values)
                if (!string.IsNullOrWhiteSpace(v))
                    parts.Add(Clean(SplitByColonAndPickLongest(v)).Trim());
            return string.Join(", ", parts).ToLower();
        }

        public static string Clean(string s) =>
            s.Replace("\"", "").Replace(":", "").Replace("\t", "").Replace("\r", "").Replace("\n", "");

        public static string SplitByColonAndPickLongest(string s)
        {
            var parts = s.Split(':');
            if (parts.Length == 1)
                return s;
            else if (parts.Length == 2)
            {
                if (parts[0].Length < parts[1].Length)
                    return parts[1];
                else
                    return parts[0];
            }
            else
                return string.Join(" ", parts[1..]);
        }        

        public enum Rarity
        {
            Commen,
            Rare,
            VeryRare

        }
    }


}