using Model;

namespace Generator.Locations
{
    public class FarmHouse : LocationType
    {
        public override LocationTypes GetTypeKey() => LocationTypes.FarmHouse;
        public override string GetTypeName() => "Farm House";
        public override string GetTypeDescription(Location location, Region region)
        {
            return JoinPromt(
                "a farm house {GetBiomPlacement(location)}",
                GetTemperatureDescription(location),
                GetGeologyDescription(location));
        }

        public override bool IsValidOn(Location location, Region region)
        {
            if (0 < location.Population &&
                location.Population < 13 &&
                location.BiomId == BiomIds.Fields)
                return true;
            return false;
        }

        public override Rarity RarityOn(Location location, Region region)
        {
            return Rarity.Commen;
        }

        public override async Task<List<PersonRequest>> GetPersonRequests(Location location, Region associateRegion)
        {
            return await PersonGenerator.CreateHoushold(location,associateRegion);
        }
    }
}