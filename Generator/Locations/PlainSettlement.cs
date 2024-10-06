using Model;

namespace Generator.Locations
{
    public class PlainSettlement : LocationType
    {
        public override LocationTypes GetTypeKey() => LocationTypes.PlainSettlement;
        public override string GetTypeName() => "Plain Settlement";
        public override string GetTypeDescription(Location location, Region region)
        {
            return JoinPromt(
                "a settlement in " + GetBiomDescription(location),
                GetTemperatureDescription(location),
                GetGeologyDescription(location));
        }

        public override bool IsValidOn(Location location, Region region)
        {
            if (7 < location.Population)
                return true;
            return false;
        }

        public override Rarity RarityOn(Location location, Region region)
        {
            return Rarity.Commen;
        }

        public override async Task<List<PersonRequest>> GetPersonRequests(Location l, Region associateRegion) => await PersonGenerator.CreateNoHoushold();// ToDo implement Settlement
    }
}