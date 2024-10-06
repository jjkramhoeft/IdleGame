using Model;

namespace Generator.Locations
{
    public class GuardTower : LocationType
    {
        public override LocationTypes GetTypeKey() => LocationTypes.GuardTower;
        public override string GetTypeName() => "Guard Tower";
        public override string GetTypeDescription(Location location, Region region)
        {
            return JoinPromt(
                "a small fortified tower {GetBiomPlacement(location)}",
                GetTemperatureDescription(location));
        }

        public override bool IsValidOn(Location location, Region region)
        {
            List<BiomIds> fittingBioms = [BiomIds.Bog, BiomIds.AncientForest, BiomIds.Cliffs, BiomIds.CrystalForest, 
            BiomIds.LavaPlain, BiomIds.Mangrove, BiomIds.Marsh, BiomIds.MountainTundra, BiomIds.MushroomForest, 
            BiomIds.BareRock, BiomIds.Tundra, BiomIds.BorealForests, BiomIds.Desert, BiomIds.Fields, BiomIds.Glaciers, 
            BiomIds.GrassSteppe, BiomIds.Permafrost, BiomIds.Plains, BiomIds.ReedsBeach, BiomIds.RiverDelta, 
            BiomIds.SandBeach, BiomIds.Savannah, BiomIds.Swamp, BiomIds.TemperateConiferousForests, BiomIds.TemperateForests,
            BiomIds.TemperateRainForests, BiomIds.TropicalCloudForests, BiomIds.TropicalDryForests, BiomIds.TropicalMoistForests,
            BiomIds.TropicalRainForests, BiomIds.Vulcano];
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