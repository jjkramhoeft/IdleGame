using Model;

namespace Generator.Locations
{
    public class HuntingCabin : LocationType
    {
        public override LocationTypes GetTypeKey() => LocationTypes.HuntingCabin;
        public override string GetTypeName() => "Hunting Cabin";
        public override string GetTypeDescription(Location location, Region region)
        {
            return JoinPromt(
                $"a hunting cabin {GetBiomPlacement(location)}",
                GetTemperatureDescription(location));
        }

        public override bool IsValidOn(Location location, Region region)
        {
            if (0 < location.Population &&
                location.Population < 13 &&
                World.GetForestBiomIds().Contains(location.BiomId))
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