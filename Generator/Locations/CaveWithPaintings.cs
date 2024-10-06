using Model;

namespace Generator.Locations
{
    public class CaveWithPaintings : LocationType
    {
        public override LocationTypes GetTypeKey() => LocationTypes.CaveWithPaintings;
        public override string GetTypeName() => "Cave with Paintings";
        public override string GetTypeDescription(Location location, Region region)
        {
            return JoinPromt(
                "a natural cave with cave paintings on the wall ",
                GetBiomDescription(location),
                GetTemperatureDescription(location),
                GetGeologyDescription(location));
        }

        public override bool IsValidOn(Location location, Region region)
        {
            if (0 < location.Population)
                return false;
            if (Height.MainlandCoastSplit < location.Height &&
                (Slope.FlatNormalSplit + Slope.NormalSteepSplit) / 2.0 < location.SlopeValue)
                return true;
            else
                return false;
        }

        public override Rarity RarityOn(Location location, Region region)
        {
            return Rarity.Rare;
        }

        public override async Task<List<PersonRequest>> GetPersonRequests(Location l, Region associateRegion) => await PersonGenerator.CreateNoHoushold();
    }
}