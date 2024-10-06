using System.Text.Json.Serialization;

namespace Model
{
    /// <summary>
    /// A named hair color
    /// </summary>
    public class HairStyle(HairStyleIds id)
    {
        /// <summary>
        /// 
        /// </summary>
        public HairStyleIds Id { get; } = id;

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public string NameWithColorKey { get; set; } = "";

        /// <summary>
        /// Alternative name
        /// </summary>
        public string AltName { get; set; } = "";

        /// <summary>
        /// Alternative name
        /// </summary>
        public List<string> Descriptions { get; set; } = [];

        /// <summary>
        /// Criterias for this hair color
        /// </summary>
        [JsonIgnore]
        public PersonFilter? Filter { get; set; }


    }

    /// <summary>
    /// 
    /// </summary>
    public enum HairStyleIds
    {
        /// 
        None = 0,
        ///             
        Afro,
        ///             
        Bald,
        ///             
        BraidedLongHair,
        ///             
        Dreadlocks,
        ///             
        FrenchBraid,
        ///             
        LongHair,
        ///             
        LongUpdo,
        /// 
        Messy,
        ///             
        ShortCurly,
        ///             
        ShortHair,
        

        
    }
}