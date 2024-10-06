namespace Model
{
    /// <summary>
    /// An activity
    /// </summary>
    public class Craft(CraftIds id)
    {
        ///
        public CraftIds Id { get; } = id;

        ///
        public string Name { get; set; } = "";
    }

    ///
    public enum CraftIds
    {
        /// 
        None = 0,
        /// 
        Acting,
        /// 
        Archery,
        /// 
        Architecture,
        /// 
        Blacksmithing,
        /// 
        Botany,
        /// 
        Brewing,
        /// 
        Carpentry,
        /// 
        Cartography,
        /// 
        Combat,
        /// 
        Construction,
        /// 
        Cooking,
        /// 
        Courtesanship,
        /// 
        Dancing,
        /// 
        Defense,
        /// 
        Entertaining,
        /// 
        Farming,
        /// 
        Fishing,
        /// 
        Gathering,
        /// 
        Gemcutting,
        /// 
        Herbalism,
        /// 
        History,
        /// 
        Hunting,
        /// 
        Jeweler,
        /// 
        Magic,
        /// 
        Management,
        /// 
        Medicine,
        /// 
        Mining,
        /// <summary> Musiic </summary>
        Playing,
        /// 
        Pottery,
        /// 
        Reading,
        /// 
        Sewing,
        /// 
        Shipbuilding,
        /// 
        Singing,
        /// 
        Storytelling,
        /// 
        Tanning,
        /// 
        Theology,
        /// 
        Tracking,
        /// 
        Trade,
        /// 
        Trapping,
        /// 
        Warfare,
        /// 
        Weave,
        /// 
        WoodCutting,
        /// 
        Writing,
        /// 
        Zoology,
    }
}