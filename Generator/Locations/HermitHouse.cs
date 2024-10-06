using Model;

namespace Generator.Locations
{
    public class HermitHouse : LocationType
    {
        public override LocationTypes GetTypeKey() => LocationTypes.HermitHouse;
        public override string GetTypeName() => "Hermit House";
        public override string GetTypeDescription(Location location, Region region)
        {
            return JoinPromt(
                "a lonely house {GetBiomPlacement(location)}",
                GetTemperatureDescription(location),
                GetGeologyDescription(location));
        }

        public override bool IsValidOn(Location location, Region region)
        {
            List<BiomIds> fittingBioms = [BiomIds.Bog, BiomIds.AncientForest, BiomIds.Cliffs, BiomIds.CrystalForest, BiomIds.LavaPlain, BiomIds.Mangrove, BiomIds.Marsh, BiomIds.MountainTundra, BiomIds.MushroomForest, BiomIds.BareRock, BiomIds.Tundra];
            if (0 < location.Population &&
                location.Population < 4 &&
                fittingBioms.Contains(location.BiomId))
                return true;
            return false;
        }

        public override Rarity RarityOn(Location location, Region region)
        {
            return Rarity.Rare;
        }

        public override async Task<List<PersonRequest>> GetPersonRequests(Location location, Region associateRegion)
        {
            return await PersonGenerator.CreateHoushold(location,associateRegion);
        }

    }
}