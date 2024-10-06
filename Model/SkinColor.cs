using System.Text.Json.Serialization;

namespace Model
{
    /// <summary>
    /// A named skin color
    /// </summary>
    public class SkinColor(SkinColorIds id)
    {
        /// 
        public SkinColorIds Id { get; } = id;

        ///
        public string Name { get; set; } = "";

        /// <summary>
        /// Alternative name
        /// </summary>
        public string AltName { get; set; } = "";

        /// 
        public List<string> Descriptions { get; set; } = [];

        /// <summary>
        /// Criterias for this skin color
        /// </summary>
        [JsonIgnore]
        public PersonFilter? Filter { get; set; }


    }

    ///
    public enum SkinColorIds
    {
        /// 
        None = 0,
        ///   
        Black,
        ///     
        Brown,
        ///             
        Green,
        ///              
        White,
    }
}