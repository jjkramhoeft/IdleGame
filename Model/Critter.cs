namespace Model
{
    /// <summary>
    /// Notable insects ect. Not interacteble
    /// </summary>
    public class Critter(CritterIds id)
    {
        /// 
        public CritterIds Id { get; } = id;

        /// 
        public string Name { get; set; } = "";

        /// 
        public string NamePlural { get; set; } = "";

        /// <summary>
        /// Rarity frequens 0 -> 1
        /// </summary>
        public double Rarity { get; set; } = 0.5;
    }

    /// 
    public enum CritterIds
    {
        ///
        Beatle,
        ///
        Bee,
        ///
        Butterfly,
        ///
        Centipede,
        ///
        Cicada,
        ///
        Fly,
        ///
        Hummingbird,
        ///
        Mouse,
    }
}