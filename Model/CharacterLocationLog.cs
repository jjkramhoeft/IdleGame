namespace Model
{
    ///
    public class CharacterLocationLog
    {
        /// <summary>
        /// Internal Db Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// CharacterId (1=player)
        /// </summary>
        public int CharacterId { get; set; }

        ///
        public required Point LocationPoint { get; set; }

        ///
        public required Box LocationBox { get; set; }

        ///        
        public required Status CharacterLocationStatus { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime ChangedAt { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? FirstAt { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int VisitCount { get; set; }

        ///
        public enum Status
        {
            ///
            Historic = 0,
            ///
            OnRoute = 1,

            ///
            Current = 2,

            /// 
            Departed = 3,
        }
    }
}