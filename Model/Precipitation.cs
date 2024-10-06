namespace Model
{
    /// <summary>
    /// meters pr. year
    /// </summary>
    public class Precipitation
    {
        ///                           
        public const float Max = 230f;

        ///                   
        public const float WetVeryWetSplit = 120f;

        ///                         
        public const float WetNormalSplit = 32f;

        ///                                
        public const float NormalDrySplit = 16f;

        ///                             
        public const float DryVeryDrySplit = 9f;

        ///                         
        public const float Min = -2.2f; //found -2.035684585571289

        /// <summary>
        /// for filters    
        /// </summary>
        public static float GetIntervalLow(NamedInterval interval) => interval switch
        {
            NamedInterval.All => Min,
            NamedInterval.VeryWet => WetVeryWetSplit,
            NamedInterval.Wet => WetNormalSplit,
            NamedInterval.Normal => NormalDrySplit,
            NamedInterval.Dry => DryVeryDrySplit,
            NamedInterval.VeryDry => Min,
            NamedInterval.NotVery => DryVeryDrySplit,
            _ => Min
        };

        /// <summary>
        /// for filters    
        /// </summary>
        public static float GetIntervalHigh(NamedInterval interval) => interval switch
        {
            NamedInterval.All => Max,
            NamedInterval.VeryWet => Max,
            NamedInterval.Wet => WetVeryWetSplit,
            NamedInterval.Normal => WetNormalSplit,
            NamedInterval.Dry => NormalDrySplit,
            NamedInterval.VeryDry => DryVeryDrySplit,
            NamedInterval.NotVery => WetVeryWetSplit,
            _ => Max
        };

        /// <summary>
        /// for foreach   
        /// </summary>
        public static List<NamedInterval> GetAllMainIntervals()
        {
            var l = new[] { NamedInterval.VeryDry, NamedInterval.Dry, NamedInterval.Normal, NamedInterval.Wet, NamedInterval.VeryWet };
            return [.. l];
        }

        ///
        public static NamedInterval GetIntervalName(float p)
        {
            if (p < DryVeryDrySplit) return NamedInterval.VeryDry;
            else if (p < NormalDrySplit) return NamedInterval.Dry;
            else if (p < WetNormalSplit) return NamedInterval.Normal;
            else if (p < WetVeryWetSplit) return NamedInterval.Wet;
            return NamedInterval.VeryWet;
        }
        ///
        public static NamedInterval GetIntervalName(double? p)
        {
            if (p is null) return NamedInterval.Normal;
            return GetIntervalName((float)p);
        }

        /// <summary>
        /// for filters    
        /// </summary>
        public enum NamedInterval
        {
            /// <summary>
            /// min -> max
            /// </summary>
            All,

            /// 
            VeryWet,

            /// 
            Wet,

            /// 
            Normal,

            ///
            Dry,

            /// 
            VeryDry,

            /// <summary>
            /// No extremes
            /// </summary>
            NotVery,
        }
    }
}