using System.Text.Json.Serialization;

namespace Model
{
    /// <summary>
    /// A named Jewelry
    /// </summary>
    public class Jewelry(JewelryIds id)
    {
        /// 
        public JewelryIds Id { get; } = id;

        ///
        public string Name { get; set; } = "";

        /// <summary>
        /// Alternative name
        /// </summary>
        public string AltName { get; set; } = "";

        /// 
        public List<string> Descriptions { get; set; } = [];

        /// <summary>
        /// Criterias for this Jewelry
        /// </summary>
        [JsonIgnore]
        public PersonFilter? Filter { get; set; }


    }

    /// 
    public enum JewelryIds
    {
        /// 
        None = 0,
        ///             
        BearToothNecklace,
        ///             
        Crown,
        ///             
        Diadem,
        ///             
        GoldBracelets,
        ///             
        GuldRings,
        ///             
        Necklace,
        ///
        GoldPendant,
        ///
        SilverPendant,
    }
}