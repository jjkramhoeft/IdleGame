using System.Text.Json.Serialization;

namespace Model
{
    /// <summary>
    /// A named hair color
    /// </summary>
    public class Dress(DressIds id)
    {
        /// <summary>
        /// 
        /// </summary>
        public DressIds Id { get; } = id;

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Alternative name
        /// </summary>
        public string AltName { get; set; } = "";

        ///
        public string NameWithColorKey { get; set; } = "";

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

    /// 
    public enum DressIds
    {
        /// 
        None = 0,
        ///             
        BastSkirtAndShortTop,
        ///           
        BlouseAndSkirt,
        ///             
        Chainmail,
        ///             
        CookApron,
        ///             
        HuntingClothes,
        ///           
        JacketAndPants,
        ///             
        LeatherClothing,
        ///             
        LongDress,
        ///          
        LongPlainRobe,
        ///             
        ShortDress,
        ///             
        ThickFurCoat,
        ///             
        WorkApron,
        ///
        Rags,
        ///
        PlateArmor,
        ///
        ScaleAmor
    }
}