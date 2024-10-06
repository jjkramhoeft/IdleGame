namespace Model
{
    /// <summary>
    /// A dangerous wild animal
    /// </summary>
    public class Beast(BeastIds id)
    {
        /// <summary>
        /// 
        /// </summary>
        public BeastIds Id { get; } = id;

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public double Rarity { get; set; } = 1;
    }

    /// 
    public enum BeastIds
    {
        ///
        Bear,
        ///
        Boar,
        ///
        Lion,
        ///
        Puma,
        ///
        Phyton,
        ///
        PolarBear,
        ///
        Wolf,
        ///
        Tiger
    }
}