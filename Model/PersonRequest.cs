namespace Model
{
    /// 
    public class PersonRequest(Box locationBox)
    {
        /// 
        public int Id {get;set;}
        /// <summary>
        /// Person seed
        /// </summary>
        public int Seed {get;set;}
        /// 
        public RaceIds RaceId {get;set;}
        /// 
        public SexIds SexId {get;set;}
        /// 
        public PersonAgeIds PersonAgeId {get;set;}
        /// 
        public ProfessionIds ProfessionId {get;set;}
        ///
        public FamilyRelationshipIds FamilyRelationshipId {get;set;}
        ///
        public LocationWealthIds LocationWealthId {get;set;}
        /// <summary>
        /// Location box where the person is created
        /// </summary>
        public Box LocationBox {get;set;}=locationBox;
        ///        
        public GenerativeState CurrentGenerativeState { get; set; }
        /// 
        public string GivenName {get;set;}="";
        /// 
        public string SurName {get;set;}="";
        ///
        public DateTime? RequestTime { get; set; }
    }
}