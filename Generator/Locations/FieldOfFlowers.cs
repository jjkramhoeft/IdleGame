using Model;

namespace Generator.Locations
{
    public class FieldOfFlowers : LocationType
    {
        public override LocationTypes GetTypeKey() => LocationTypes.FieldOfFlowers;
        public override string GetTypeName() => "Field of Flowers";
        public override string GetTypeDescription(Location location, Region region)
        {
            return JoinPromt(
                "an open field with colorful flowers. Lots of flowers.",
                GetBiomDescription(location),
                GetTemperatureDescription(location),
                GetGeologyDescription(location));
        }

        public override bool IsValidOn(Location location, Region region)
        {
            if (0 < location.Population)
                return false;
            if (location.BiomId == BiomIds.Plains ||
                location.BiomId == BiomIds.Savannah ||
                location.BiomId == BiomIds.Tundra ||
                location.BiomId == BiomIds.MountainTundra ||
                location.BiomId == BiomIds.Fields ||
                location.BiomId == BiomIds.GrassSteppe )
                return false;
            else
                return false;
        }

        public override Rarity RarityOn(Location location, Region region)
        {
            if (location.BiomId == BiomIds.Savannah )
                return Rarity.VeryRare;
            else
                return Rarity.Commen;
        }

        public override async Task<List<PersonRequest>> GetPersonRequests(Location l, Region associateRegion) => await PersonGenerator.CreateNoHoushold();
    }
}