using Model;

namespace Generator.Locations
{
    public class FishingHut : LocationType
    {
        public override LocationTypes GetTypeKey() => LocationTypes.FishingHut;
        public override string GetTypeName() => "Fishing hut";
        public override string GetTypeDescription(Location location, Region region)
        {
            return JoinPromt(
                $"a small fishing hut {GetBiomPlacement(location)}, fishing nets and a small boat ",
                GetTemperatureDescription(location));
        }

        public override bool IsValidOn(Location location, Region region)
        {
            if (0 < location.Population &&
                location.Population < 13 &&
                (location.BiomId == BiomIds.ReedsBeach || location.BiomId == BiomIds.SandBeach || location.BiomId == BiomIds.Mangrove || location.BiomId == BiomIds.RiverDelta || location.BiomId == BiomIds.Swamp))
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