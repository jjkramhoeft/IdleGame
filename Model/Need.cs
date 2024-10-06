namespace Model
{
    /// <summary>
    /// Player and NPC need. Defining NPC behavior
    /// </summary>
    public class Need(NeedIds id)
    {
        ///
        public NeedIds Id { get; } = id;

        /// 
        public string Name { get; set; } = "";
    }

    ///
    public enum NeedIds
    {
        /// 
        None = 0,
        /// 
        Air,
        /// 
        Clothes,
        /// 
        Community,
        /// 
        Drink,
        /// 
        Entertainment,
        /// 
        Expression,
        /// 
        Fellowship,
        /// 
        Food,
        ///
        Health,
        ///
        Reproduction,
        /// 
        Rest,
        ///
        SecurityProtection,
        /// 
        SecurityWealth,
        ///
        Shelter,
        ///
        Temperature,
    }

}