using System.Text.Json.Serialization;

namespace Model
{
    /// <summary>
    /// A named biom
    /// </summary>
    public class Biom(BiomIds id)
    {
        ///
        public BiomIds Id { get; } = id;

        ///
        public string Name { get; set; } = "";

        /// <summary>
        /// Alternative name
        /// </summary>
        public string AltName { get; set; } = "";

        /// <summary>
        /// Used for placing buildings in this type of location/biom. E.g.  "A house at the beach"
        /// </summary>
        public string PlacementName { get; set; } = "";

        /// <summary>
        /// Alternative name
        /// </summary>
        public List<string> Descriptions { get; set; } = [];

        /// <summary>
        /// Criterias for this biom
        /// </summary>
        [JsonIgnore]
        public List<LocationFilter> Filters { get; set; } = [];

        ///
        public PopulationPotentials PopulationPotential { get; set; }

        /// 
        public BiomIds AncientId { get; set; } = BiomIds.None;

        ///
        public BiomIds ModernId { get; set; } = BiomIds.None;

        ///
        public List<TravelMode> ValidTravelModes { get; set; } = [];

        /// 
        public List<TreeIds> Trees { get; set; } = [];


        /// 
        public List<PlantIds> Plants { get; set; } = [];


        /// 
        public List<CritterIds> Critter { get; set; } = [];


        /// 
        public List<AnimalIds> Animals { get; set; } = [];


        /// 
        public List<BeastIds> Beast { get; set; } = [];


        /// 
        public List<MonsterIds> Monsters { get; set; } = [];




        /// <summary>
        /// Potential Numbers of People living in a location in this biom
        /// </summary>
        public enum PopulationPotentials
        {
            /// <summary>
            /// 0
            /// </summary>
            None = 0,

            /// <summary>
            /// 0-5
            /// </summary>
            VeryFew,

            /// <summary>
            /// 0-10
            /// </summary>
            Few,

            /// <summary>
            /// 1-20
            /// </summary>
            Some,

            /// <summary>
            /// 4-50
            /// </summary>
            Good,

            /// <summary>
            /// 10-500
            /// </summary>
            Great
        }

        /// <summary>
        /// for foreach   
        /// </summary>
        public static List<BiomIds> GetAllUsedBioms()
        {
            var l = new[] { BiomIds.AncientForest, BiomIds.Bank, BiomIds.BareRock, BiomIds.Bog, BiomIds.BorealForests, 
                BiomIds.Cliffs, BiomIds.CoralReef, BiomIds.CrystalForest, BiomIds.Desert, BiomIds.Fields, 
                BiomIds.Glaciers, BiomIds.GrassSteppe, BiomIds.LavaPlain, BiomIds.Mangrove, BiomIds.Marsh, 
                BiomIds.MountainTundra, BiomIds.MushroomForest, BiomIds.Ocean, BiomIds.Permafrost, BiomIds.Plains, 
                BiomIds.ReedsBeach, BiomIds.RiverDelta, BiomIds.SandBeach, BiomIds.Savannah, BiomIds.Sea, 
                BiomIds.SeaIce, BiomIds.SeaweedForest, BiomIds.Swamp, BiomIds.TemperateConiferousForests, BiomIds.TemperateForests, 
                BiomIds.TemperateRainForests, BiomIds.TropicalCloudForests, BiomIds.TropicalDryForests, BiomIds.TropicalMoistForests, BiomIds.TropicalRainForests, 
                BiomIds.Tundra, BiomIds.Vulcano};
            return [.. l];
        }

    }

    ///
    public enum BiomIds
    {
        /// 
        None = 0,
        ///      
        AncientForest,
        ///
        Bank,
        ///
        BareRock,
        ///
        Bog,
        ///
        BorealForests,
        ///
        Cliffs,
        ///
        CoralReef,
        ///
        CrystalForest,
        ///
        Desert,
        ///
        Fields,
        ///
        Glaciers,
        ///
        GrassSteppe,
        ///
        LavaPlain,
        ///
        Mangrove,
        ///
        Marsh,
        ///
        MountainTundra,
        ///
        MushroomForest,
        ///
        Ocean,
        ///
        Permafrost,
        ///
        Plains,
        ///
        ReedsBeach,
        ///
        RiverDelta,
        ///
        SandBeach,
        ///
        Savannah,
        ///
        Sea,
        ///
        SeaIce,
        ///
        SeaweedForest,
        ///
        Swamp,
        ///
        TemperateConiferousForests,
        ///
        TemperateForests,
        ///
        TemperateRainForests,
        ///
        TropicalCloudForests,
        ///
        TropicalDryForests,
        ///
        TropicalMoistForests,
        ///
        TropicalRainForests,
        ///
        Tundra,
        ///
        Vulcano,
    }
}