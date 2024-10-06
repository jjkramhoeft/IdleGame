namespace Model
{
    /// <summary>
    /// A named race for NPC's
    /// </summary>
    public class Race(RaceIds id)
    {
        /// 
        public RaceIds Id { get; } = id;

        ///
        public string Name { get; set; } = "";

        /// <summary>
        /// Alternative name
        /// </summary>
        public string AltName { get; set; } = "";

        ///
        public Language Language { get; set; }

        ///
        public List<BiomIds> PrimaryBioms { get; set; } = [];

        ///
        public List<BiomIds> SecondaryBioms { get; set; } = [];

        /// <summary>
        /// An additional filter beyond bioms
        /// </summary>
        public LocationFilter? Filter { get; set; }

        /// <summary>
        /// Multiplied to population potential, (1=Human)
        /// </summary>        
        public float PopulationModifier { get; set; } = 1f;

        /// 
        public static List<RaceIds> AllIds => [RaceIds.Human,RaceIds.Elve,RaceIds.Dwarf,RaceIds.Orc,
            RaceIds.Goblin,RaceIds.Haflings,RaceIds.Lizard,RaceIds.Hare,RaceIds.Centaur,RaceIds.Ent,
            RaceIds.Mer,RaceIds.Minotaur,RaceIds.Satyr,RaceIds.Fae,RaceIds.Nymph,RaceIds.Thiefling];
    }

    ///
    public enum RaceIds
    {
        ///
        None = 0,
        ///
        Centaur,
        ///
        Dwarf,
        ///
        Elve,
        ///
        Ent,
        ///
        Fae,
        ///
        Goblin,
        ///
        Haflings,
        ///
        Hare,
        ///
        Human,
        ///
        Lizard,
        ///
        Mer,
        ///
        Minotaur,
        ///
        Nymph,
        ///
        Orc,
        ///
        Satyr,
        ///
        Thiefling,
        ///
        All = 99
    }
}

