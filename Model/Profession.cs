namespace Model
{
    /// <summary>
    /// A vocation
    /// </summary>
    public class Profession(ProfessionIds id)
    {
        ///
        public ProfessionIds Id { get; } = id;

        /// 
        public string Name { get; set; } = "";

        /// 
        public List<CraftIds> MainCrafts { get; set; } = [];

        ///                         
        public List<CraftIds> SecondaryCrafts { get; set; } = [];

        ///                         
        public PersonFilter? Filter { get; set; }
    }

    ///
    public enum ProfessionIds
    {
        ///
        None = 0,
        ///
        Bard,
        ///
        Beggar,
        ///
        Blacksmith,
        ///
        Carpenter,
        ///
        Cook,
        ///
        Courtesan,
        ///
        Farmer,
        ///
        Fisherman,
        ///
        Guard,
        ///
        Herbalist,
        ///
        Hunter,
        ///
        InnKeeper,
        ///
        Miner,
        ///
        Monk,
        ///
        Officer,
        ///
        Ruler,
        ///
        Shepherd,
        ///
        Tailor,
        ///
        Trader,
        ///
        Servant,
        ///
        Wizard,
    }
}