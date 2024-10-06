using Model;

namespace Generator.Locations
{
    public class PlainLocation : LocationType
    {
        public override LocationTypes GetTypeKey() => LocationTypes.PlainLocation;
        public override string GetTypeName() => "Plain Location";
        public override string GetTypeDescription(Location location, Region region)
        {
            return JoinPromt(
                GetBiomDescription(location),
                GetTemperatureDescription(location),
                GetGeologyDescription(location));
        }

        public override bool IsValidOn(Location location, Region region)
        {
            if (0 < location.Population)
                return false;
            return true;
        }

        public override Rarity RarityOn(Location location, Region region)
        {
            return Rarity.Commen;
        }

        public override async Task<List<PersonRequest>> GetPersonRequests(Location l, Region associateRegion) => await PersonGenerator.CreateNoHoushold();
    }
}