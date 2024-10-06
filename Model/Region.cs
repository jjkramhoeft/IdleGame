using System.Text.Json.Serialization;

namespace Model
{
    /// <summary> A region of locations with the same socialstructure (ruler(s)).</summary>
    [Serializable]
    public class Region
    {
        /// <summary> Construct from box.</summary>
        public Region(Box box)
        {
            Box = box;
            Center = new Point(0, 0);
            BiomCount = [];
            SupportingRaceIds = [];
            OpposingRaceIds = [];
            RacialPopulationPotential = [];
            RacialTopLocations = [];
            RacialStrongholdPoints = [];
            GeologiesLocationCount = [];
            RacialLocationCount = [];
            RacialStrongholdRequests=[];
            Name = "";
        }

        /// <summary> The box associated with this region.</summary>
        public Box Box { get; set; }

        /// <summary> The geometric randomly chosen center.</summary>
        public Point Center { get; set; }

        /// <summary> Set by storage when saved first time.</summary>
        public int Id { get; set; }

        /// 
        public GenerativeState State { get; set; }

        /// <summary> Name of the region. </summary>
        public string Name { get; set; }

        /// <summary> Id of the leading race in the region </summary>
        public RaceIds MainRaceId { get; set; }

        /// <summary> Race in region </summary>
        public List<RaceIds> SupportingRaceIds { get; set; }

        /// <summary> Race in region </summary>
        public List<RaceIds> OpposingRaceIds { get; set; }

        ///
        public RegionWealth RegionWealth { get; set; }

        /// <summary> The Surname of the ruling/leading family.</summary>
        public string? RulingFamilyName { get; set; }

        /// <summary> The regions capital city </summary>
        public Point? CapitalPoint { get; set; }

        /// <summary> The regions number two city </summary>
        public Point? SecondaryCityPoint { get; set; }

        /// 
        public Dictionary<RaceIds, Point> RacialStrongholdPoints { get; set; }

        /// <summary> The location of this regions life tree.</summary>
        public Point? LifeTreePoint { get; set; }

        // ---- Stats ---------------------------------------------------------------------------------------


        /// <summary> Candidate locations for capital and race strongholds </summary>
        [JsonIgnore]
        public Dictionary<RaceIds, List<Location>> RacialTopLocations { get; set; }
        
        /// <summary> temp loc for stronghold request </summary>
        [JsonIgnore]
        public Dictionary<RaceIds, List<Location>> RacialStrongholdRequests { get; set; }

        /// <summary> Size. </summary>
        public int LocationCount { get; set; }

        /// <summary> Amount of Water.</summary>
        public int WaterLocationCount { get; set; }

        /// 
        public int ModernLocationCount { get; set; }

        /// 
        public int AncientLocationCount { get; set; }

        /// <summary> Total number of location with popuplation potential in this region.</summary>
        public int InhabitedLocationCount { get; set; }

        /// <summary> Number of location where this race has the most popuplation potential.</summary>
        public Dictionary<RaceIds, int> RacialLocationCount { get; set; }


        /// <summary> 6 </summary>
        public Dictionary<GeologyId, int> GeologiesLocationCount { get; set; }

        /// <summary> Sum of general population potential.</summary>
        public int CombinedPopulationPotential { get; set; }

        /// <summary> Sum of population potential for a race </summary>
        public Dictionary<RaceIds, int> RacialPopulationPotential { get; set; }

        /// <summary> BiomId, Count </summary>
        public Dictionary<BiomIds, int> BiomCount { get; set; }
    }
}