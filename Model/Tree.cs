namespace Model
{
    /// <summary>
    /// Notable tree
    /// </summary>
    public class Tree(TreeIds id)
    {
        /// 
        public TreeIds Id { get; } = id;

        /// 
        public string Name { get; set; } = "";

        /// <summary>
        /// Alternative name
        /// </summary>
        public string AltName { get; set; } = "";

        /// <summary>
        /// Descriptions
        /// </summary>
        public List<string> Descriptions { get; set; } = [];

        

        /// 
        public string Fruit { get; set; } = "";

        /// <summary>
        /// Rarity frequens 0 -> 1
        /// </summary>
        public double Rarity { get; set; } = 0.5;
    }

    /// 
    public enum TreeIds
    {
        ///
        Acai,
        ///
        Apple,
        ///
        Apricot,
        ///
        Ash,
        ///
        Aspen,
        ///
        Avocado,
        ///
        Baobab,
        ///
        Basswood,
        ///
        Beech,
        ///
        Birch,
        ///
        BlackMangrove,
        ///   
        ButternutTree,
        ///
        Cedar,
        ///
        Curatella,
        ///
        CacaoTree,
        ///
        Cypress,
        ///
        Elm,
        ///
        Fig,
        ///
        Fir,
        ///
        Ginkgo,
        /// 
        Kapok, 
        ///        
        Lemmon,
        ///
        Lemon,
        ///
        Magnolia,
        ///
        Mango,
        ///
        Maple,
        ///
        Oak,
        ///
        Olive,
        ///
        Palm,
        ///
        Pecan,
        ///
        Pine,
        ///
        Plum,
        ///
        Poplar,
        ///
        Rambutan,
        ///
        RedAlder,
        ///
        RedMangrove,
        ///
        RubberTree,
        ///
        Teak,
        ///
        TulipTree,
        ///
        Walnut,
        ///
        Willow,
    }
}