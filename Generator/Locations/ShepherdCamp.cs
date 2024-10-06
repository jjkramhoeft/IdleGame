using Model;

namespace Generator.Locations
{
    public class ShepherdCamp : LocationType
    {
        public override LocationTypes GetTypeKey() => LocationTypes.ShepherdCamp;
        public override string GetTypeName() => "Shepherd Camp";
        public override string GetTypeDescription(Location location, Region region)
        {
            return JoinPromt(
                $"a shepherd camp, some tents and a well {GetBiomPlacement(location)}",
                GetTemperatureDescription(location));
        }

        public override bool IsValidOn(Location location, Region region)
        {
            if (0 < location.Population &&
                location.Population < 13 &&
                (location.BiomId== BiomIds.Savannah||location.BiomId== BiomIds.GrassSteppe||location.BiomId== BiomIds.Plains))
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