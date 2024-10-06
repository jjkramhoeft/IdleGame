using System.Text.Json.Serialization;

namespace Model
{
    /// <summary>
    /// A named hair color
    /// </summary>
    public class HairColor(HairColorIds id)
    {
        /// <summary>
        /// 
        /// </summary>
        public HairColorIds Id { get; } = id;

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
    public enum HairColorIds
    {
        /// 
        None = 0,
        ///             
        Blond = 1,
        ///             
        Golden = 2,
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
    }
}