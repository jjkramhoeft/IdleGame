namespace Model
{
    /// <summary>
    /// An animal. Can be hunted or domesticated
    /// </summary>
    public class Animal(AnimalIds id)
    {
        /// <summary>
        /// 
        /// </summary>
        public AnimalIds Id { get; } = id;

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public string NamePlural { get; set; } = "";

        /// <summary>
        /// Rarity frequens 0 -> 1
        /// </summary>
        public double Rarity { get; set; } = 0.5;


    }

    /// 
    public enum AnimalIds
    {
        ///
        Cow,
        ///
        Fox,
        ///
        Deer,
        ///
        Pig,
        ///
        Rabbit,
        ///
        Horse,
        ///
        Beaver,
        ///
        Gazelle
    }
}