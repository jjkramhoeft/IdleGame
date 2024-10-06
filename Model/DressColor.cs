using System.Text.Json.Serialization;

namespace Model
{
    /// <summary>
    /// A named hair color
    /// </summary>
    public class DressColor(DressColorIds id)
    {
        /// <summary>
        /// 
        /// </summary>
        public DressColorIds Id { get; } = id;

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; } = "";

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
    public enum DressColorIds
    {
        /// 
        None = 0,
        ///             
        Yellow = 1,
        ///             
        Gold = 2,
        ///             
        White = 3,
        ///             
        Gray = 4,
        ///             
        Brown = 5,
        ///             
        Black = 6,
        ///             
        Red = 7,
        ///             
        Pink = 8,
        ///             
        Blue = 9,
        ///             
        Green = 10,
        ///             
        Purple = 11,
        ///             
        Silver = 12,
        ///             
        Beige = 13,
    }
}