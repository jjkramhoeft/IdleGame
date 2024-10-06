using Model;

namespace Generator.Locations
{
    public class Glade : LocationType
    {
        public override LocationTypes GetTypeKey() => LocationTypes.Glade;
        public override string GetTypeName() => "Glade";
        public override string GetTypeDescription(Location location, Region region)
        {
            return JoinPromt(
                "a glade in the forest. A grassy open space in the forest. Lots of flowers",
                GetBiomDescription(location),
                GetTemperatureDescription(location),
                GetGeologyDescription(location));
        }

        public override bool IsValidOn(Location location, Region region)
        {
            if (0 < location.Population)
                return false;
            if (World.GetForestBiomIds().Contains(location.BiomId))
                return true;
            else
                return false;
        }

        public override Rarity RarityOn(Location location, Region region)
        {
            if (location.BiomId == BiomIds.AncientForest ||
                location.BiomId == BiomIds.MushroomForest)
                return Rarity.VeryRare;
            else
                return Rarity.Rare;
        }

        public override async Task<List<PersonRequest>> GetPersonRequests(Location l, Region associateRegion) => await PersonGenerator.CreateNoHoushold();
    }
}