using Model;

namespace Generator.Locations
{
    public class DeadTree : LocationType
    {
        public override LocationTypes GetTypeKey() => LocationTypes.DeadTree;
        public override string GetTypeName() => "Dead Tree";
        public override string GetTypeDescription(Location location, Region region)
        {
            if(World.GetForestBiomIds().Contains(location.BiomId))
                return JoinPromt(
                    "a single huge dead tree in the middle of the forest",
                    GetBiomDescription(location),
                    GetTemperatureDescription(location),
                    GetGeologyDescription(location));
            else
                return JoinPromt(
                    "a single huge lone dead tree out in the open",
                    GetBiomDescription(location),
                    GetTemperatureDescription(location),
                    GetGeologyDescription(location));
        }

        public override bool IsValidOn(Location location, Region region)
        {
            if (0 < location.Population)
                return false;
            if (Height.MainlandCoastSplit<location.Height &&
                Temperature.NormalColdSplit<location.Climate?.AverageTemperature)
                return true;
            else
                return false;
        }

        public override Rarity RarityOn(Location location, Region region)
        {
            if (location.BiomId == BiomIds.Desert ||
                location.BiomId == BiomIds.BareRock ||
                location.BiomId == BiomIds.Tundra ||
                location.BiomId == BiomIds.Glaciers ||
                location.BiomId == BiomIds.Permafrost ||
                location.BiomId == BiomIds.MountainTundra ||
                location.BiomId == BiomIds.Vulcano )
                return Rarity.VeryRare;
            else if (World.GetForestBiomIds().Contains(location.BiomId))
                return Rarity.Commen;
            else
                return Rarity.Commen;
        }

        public override async Task<List<PersonRequest>> GetPersonRequests(Location l, Region associateRegion) => await PersonGenerator.CreateNoHoushold();
    }
}