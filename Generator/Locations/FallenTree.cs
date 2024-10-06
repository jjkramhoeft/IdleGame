using Model;

namespace Generator.Locations
{
    public class FallenTree : LocationType
    {
        public override LocationTypes GetTypeKey() => LocationTypes.FallenTree;
        public override string GetTypeName() => "Fallen Tree";
        public override string GetTypeDescription(Location location, Region region)
        {
            return JoinPromt(
                "a fallen tree in the middle of the forest. broken branches. ",
                GetBiomDescription(location),
                GetTemperatureDescription(location),
                GetGeologyDescription(location));
        }

        public override bool IsValidOn(Location location, Region region)
        {
            if (0 < location.Population)
                return false;
            if (World.GetForestBiomIds().Contains(location.BiomId))
                return true;
            else
                return false;
        }

        public override Rarity RarityOn(Location location, Region region)
        {
            if ( location.BiomId == BiomIds.MushroomForest)
                return Rarity.VeryRare;
            else if(location.BiomId == BiomIds.AncientForest)
                return Rarity.Rare;
            else
            return Rarity.Commen;
        }

        public override async Task<List<PersonRequest>> GetPersonRequests(Location l, Region associateRegion) => await PersonGenerator.CreateNoHoushold();
    }
}