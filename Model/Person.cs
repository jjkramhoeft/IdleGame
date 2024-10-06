namespace Model
{
    /// <summary>
    /// A NPC
    /// </summary>
    public class Person(int id)
    {
        /// <summary> picked by db via personrequests </summary>
        public int Id { get; } = id;

        /// 
        public GenerativeState State { get; set; }

        /// <summary> give/first + surname</summary>
        public string Name { get; set; } = "";

        /// <summary> Male=0 Female=1 </summary>
        public SexIds SexId { get; set; }

        ///
        public RaceIds RaceId { get; set; }

        ///
        public PersonAgeIds AgeId { get; set; }

        /// <summary> Age group </summary>
        public PersonAgeIds PersonAgeId { get; set; }

        /// 
        public ProfessionIds ProfessionId { get; set; }

        /// 
        public HairStyleIds HairStyleId { get; set; }

        /// 
        public HairColorIds HairColorId { get; set; }

        /// 
        public SkinColorIds SkinColorId { get; set; }

        /// 
        public JewelryIds JewelryId { get; set; }

        /// 
        public DressIds DressId { get; set; }

        /// 
        public DressColorIds DressColorId { get; set; }

        /// 
        public LocationWealthIds WealthId { get; set; }

        /// 
        public string Description { get; set; } = "";

        /// 
        public string DescriptionPromptByLLM { get; set; } = "";

        /// <summary> Full path and name for the locations main picture </summary>
        public string? PicturePath { get; set; }

        /// <summary> Api uri to the locations main picture </summary>
        public Uri? PictureUri { get; set; }
    }
}