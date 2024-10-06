using Model;

namespace Generator.Locations
{
    public class DeadAnimal : LocationType
    {
        public override LocationTypes GetTypeKey() => LocationTypes.DeadAnimal;
        public override string GetTypeName() => "Dead Animal";
        public override string GetTypeDescription(Location location, Region region)
        {
            return JoinPromt(
                "the remains of a large dead animal lie on the forest floor",
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