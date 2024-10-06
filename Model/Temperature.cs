namespace Model
{
    /// <summary>
    /// Celsius
    /// </summary>
    public class Temperature
    {

        /// <summary>
        /// Higst posibel in this world (10 out of 10)
        /// </summary>
        public const float Max = 42.3f;

        /// <summary>
        /// (9 out of 10)
        /// </summary>
        public const float HotVeryHotSplit = 30f;

        /// <summary>
        /// (8 out of 10)
        /// </summary>                             
        public const float WarmHotSplit = 24f;

        /// <summary>
        /// (7 out of 10)
        /// </summary>                                     
        public const float WarmNormalSplit = 12;

        /// <summary>
        /// (6 out of 10)
        /// </summary>                                     
        public const float NormalColdSplit = 1f;

        /// <summary>
        /// (5 out of 10)
        /// </summary>                                     
        public const float ColdVeryColdSplit = -6f;

        /// <summary>
        /// Lowest posibel in this world (1 out of 10)
        /// </summary>
        public const float Min = -31.0f;

        /// <summary>
        /// for filters    
        /// </summary>
        public static float GetIntervalLow(NamedInterval interval) => interval switch
        {
            NamedInterval.All => Min,
            NamedInterval.VeryHot => HotVeryHotSplit,
            NamedInterval.Hot => WarmHotSplit,
            NamedInterval.Warm => WarmNormalSplit,
            NamedInterval.Normal => NormalColdSplit,
            NamedInterval.Cold => ColdVeryColdSplit,
            NamedInterval.VeryCold => Min,
            NamedInterval.NotVeryCold => ColdVeryColdSplit,
            NamedInterval.ColdToWarm => ColdVeryColdSplit,
            NamedInterval.WarmAndUp => WarmNormalSplit,
            NamedInterval.NormalToHot => NormalColdSplit,
            NamedInterval.NormalAndWarm => NormalColdSplit,
            NamedInterval.ColdAndVeryCold => Min,
            NamedInterval.HotAndVeryHot => WarmHotSplit,
            _ => Min
        };

        /// <summary>
        /// for filters    
        /// </summary>
        public static float GetIntervalHigh(NamedInterval interval) => interval switch
        {
            NamedInterval.All => Max,
            NamedInterval.VeryHot => Max,
            NamedInterval.Hot => HotVeryHotSplit,
            NamedInterval.Warm => WarmHotSplit,
            NamedInterval.Normal => WarmNormalSplit,
            NamedInterval.Cold => NormalColdSplit,
            NamedInterval.VeryCold => ColdVeryColdSplit,
            NamedInterval.NotVeryCold => Max,
            NamedInterval.ColdToWarm => WarmHotSplit,
            NamedInterval.WarmAndUp => Max,
            NamedInterval.NormalToHot => HotVeryHotSplit,
            NamedInterval.NormalAndWarm => WarmHotSplit,
            NamedInterval.ColdAndVeryCold => NormalColdSplit,
            NamedInterval.HotAndVeryHot => Max,
            _ => Max
        };

        /// <summary>
        /// for foreach   
        /// </summary>
        public static List<NamedInterval> GetAllMainIntervals()
        {
            var l = new[] { NamedInterval.VeryCold, NamedInterval.Cold, NamedInterval.Normal, NamedInterval.Warm, NamedInterval.Hot, NamedInterval.VeryHot };
            return [.. l];
        }

        ///
        public static NamedInterval GetIntervalName(float h)
        {
            if (h < ColdVeryColdSplit) return NamedInterval.VeryCold;
            else if (h < NormalColdSplit) return NamedInterval.Cold;
            else if (h < WarmNormalSplit) return NamedInterval.Normal;
            else if (h < WarmHotSplit) return NamedInterval.Warm;
            else if (h < HotVeryHotSplit) return NamedInterval.Hot;
            return NamedInterval.VeryHot;
        }
        ///
        public static NamedInterval GetIntervalName(double? h)
        {
            if (h is null) return NamedInterval.Normal;
            return GetIntervalName((float)h);
        }

        /// <summary>
        /// For filters.    
        /// </summary>
        public enum NamedInterval
        {
            /// <summary>
            /// min -> max
            /// </summary>
            All,

            /// <summary>
            /// 
            /// </summary>
            VeryHot,

            /// <summary>
            /// 
            /// </summary>
            Hot,

            /// <summary>
            /// 
            /// </summary>
            Warm,

            /// <summary>
            /// 
            /// </summary>
            Normal,

            /// <summary>
            /// 
            /// </summary>
            Cold,

            /// <summary>
            /// 
            /// </summary>
            VeryCold,

            /// <summary>
            /// Cold or warmer
            /// </summary>
            NotVeryCold,

            /// <summary>
            /// Cold and down
            /// </summary>
            ColdAndVeryCold,

            /// <summary>
            /// Cold and down
            /// </summary>
            HotAndVeryHot,
            
            /// <summary>
            /// Cold and down
            /// </summary>
            NormalAndWarm,

            /// <summary>
            /// No extremes
            /// </summary>
            ColdToWarm,

            /// <summary>
            /// No extremes
            /// </summary>
            WarmAndUp,

            /// <summary>
            /// Plessant
            /// </summary>
            NormalToHot,
        }
    }
}