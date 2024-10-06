namespace Model
{
    /// <summary>
    /// An epic monster. Feared by all.
    /// </summary>
    public class Monster(MonsterIds id)
    {
        /// 
        public MonsterIds Id { get; } = id;

        ///
        public string Name { get; set; } = "";

        /// 
        public double Rarity { get; set; } = 1;
    }

    /// 
    public enum MonsterIds
    {
        ///
        Dragon,
        ///
        Minotauer,
        ///
        Harpyr,
        ///
        Balrock,
        ///
        Ogre,
        ///
        Griffin,
        ///
        Zombie,
        ///
        Troll
    }
}