namespace Model
{
    /// <summary>
    /// Notable plants
    /// </summary>
    public class Plant(PlantIds id)
    {
        /// 
        public PlantIds Id { get; } = id;

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
        /// <summary>
        /// Rarity frequens 0 -> 1
        /// </summary>
        public double Rarity { get; set; } = 0.5;
    }

    /// 
    public enum PlantIds
    {
        ///
        Anemone,
        ///
        Cacti,
        ///
        Cottongrass,
        ///
        Fern,
        ///
        Grass,
        ///
        Ivy,
        ///
        Lily,
        ///
        Lichen,//Lav
        ///
        Moss,
        ///
        Orchid,
        /// 
        PassionFlower,
        /// 
        Rose,
        ///
        Scrub,
        ///
        WaterLilies,
    }
}