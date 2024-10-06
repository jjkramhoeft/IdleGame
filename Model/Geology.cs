namespace Model
{
    /// <summary>
    /// A geology type effecting locations
    /// 4 common: Forrest, Plain, Poor, Fertile
    /// 4 rare: Magic, Old, Minirals, Dark
    /// </summary>
    public class Geology(GeologyId id)
    {
        /// <summary>
        /// 
        /// </summary>                             
        public const int Minerals = 0;

        /// <summary>
        /// 
        /// </summary>                             
        public const int Dark = 1;

        /// <summary>
        /// 
        /// </summary>                             
        public const int Fertile = 2;

        /// <summary>
        /// 
        /// </summary>                             
        public const int Forest = 3;

        /// <summary>
        /// 
        /// </summary>                             
        public const int Old = 4;

        /// <summary>
        /// 
        /// </summary>                             
        public const int Magic = 5;

        /// <summary>
        /// 
        /// </summary>                             
        public const int Poor = 6;

        /// <summary>
        /// 
        /// </summary>                             
        public const int Plain = 7;

        /// <summary>
        /// 
        /// </summary>
        public GeologyId GeologyId { get; set; } = id;

        /// <summary>
        /// Forrest, Plain, Poor, Fertile, Magic, Old, Minirals or Dark
        /// </summary>
        public string Name { get; set; } = "";

        /// 
        public static List<GeologyId> AllIds => [GeologyId.Minerals, GeologyId.Dark, GeologyId.Fertile, GeologyId.Forest, GeologyId.Old, GeologyId.Magic, GeologyId.Poor, GeologyId.Plain];
    }

    /// 
    public enum GeologyId
    {
        /// 
        Minerals = 0,
        /// 
        Dark = 1,
        /// 
        Fertile = 2,
        /// 
        Forest = 3,
        /// 
        Old = 4,
        /// 
        Magic = 5,
        /// 
        Poor = 6,
        /// 
        Plain = 7
    }
}