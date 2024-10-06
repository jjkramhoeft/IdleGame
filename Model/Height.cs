namespace Model
{
    /// <summary>
    /// 1000 meters
    /// </summary>
    public class Height
    {
        /// <summary>
        /// 
        /// </summary>
        public const float Max = 1.31f;

        /// <summary>
        /// 
        /// </summary>
        public const float MountainHillSplit = 0.700f;

        /// <summary>
        /// 
        /// </summary>                            
        public const float HillLowHillSplit = 0.400f;

        /// <summary>
        /// 
        /// </summary>                              
        public const float LowHillsMainlandSplit = 0.200f;

        /// <summary>
        /// 
        /// </summary>                            
        public const float MainlandCoastSplit = 0.065f;

        /// <summary>
        ///     
        /// </summary>
        public const float WaterLevel = 0f;

        /// <summary>
        ///     
        /// </summary>
        public const float ShallowDeepWaterSplit = -0.400f;

        /// <summary>
        ///     
        /// </summary>
        public const float Min = -0.81f;

        /// <summary>
        /// for filters    
        /// </summary>
        public static float GetIntervalLow(NamedInterval interval) => interval switch
        {
            NamedInterval.All => Min,
            NamedInterval.Mountains => MountainHillSplit,
            NamedInterval.Hills => HillLowHillSplit,
            NamedInterval.LowHills => LowHillsMainlandSplit,
            NamedInterval.Mainland => MainlandCoastSplit,
            NamedInterval.Coast => WaterLevel,
            NamedInterval.ShallowWater => ShallowDeepWaterSplit,
            NamedInterval.DeepWater => Min,
            NamedInterval.MainlandPlusAllHills => MainlandCoastSplit,
            NamedInterval.HillsAndUp => HillLowHillSplit,
            NamedInterval.CoastToLowHills => WaterLevel,
            _ => Min
        };

        /// <summary>
        /// for filters    
        /// </summary>
        public static float GetIntervalHigh(NamedInterval interval) => interval switch
        {
            NamedInterval.All => Max,
            NamedInterval.Mountains => Max,
            NamedInterval.Hills => MountainHillSplit,
            NamedInterval.LowHills => HillLowHillSplit,
            NamedInterval.Mainland => LowHillsMainlandSplit,
            NamedInterval.Coast => MainlandCoastSplit,
            NamedInterval.ShallowWater => WaterLevel,
            NamedInterval.DeepWater => ShallowDeepWaterSplit,
            NamedInterval.MainlandPlusAllHills => MountainHillSplit,
            NamedInterval.HillsAndUp => Max,
            NamedInterval.CoastToLowHills => HillLowHillSplit,
            _ => Max
        };

        /// <summary>
        /// for foreach   
        /// </summary>
        public static List<NamedInterval> GetAllMainIntervals()
        {
            var l = new[] { NamedInterval.DeepWater, NamedInterval.ShallowWater, NamedInterval.Coast, NamedInterval.Mainland, NamedInterval.LowHills, NamedInterval.Hills, NamedInterval.Mountains };
            return [.. l];
        }

        ///
        public static NamedInterval GetIntervalName(float h)
        {
            if (h < ShallowDeepWaterSplit) return NamedInterval.DeepWater;
            else if (h < WaterLevel) return NamedInterval.ShallowWater;
            else if (h < MainlandCoastSplit) return NamedInterval.Coast;
            else if (h < LowHillsMainlandSplit) return NamedInterval.Mainland;
            else if (h < HillLowHillSplit) return NamedInterval.LowHills;
            else if (h < MountainHillSplit) return NamedInterval.Hills;
            return NamedInterval.Mountains;
        }
        ///
        public static NamedInterval GetIntervalName(double h)
        {
            return GetIntervalName((float)h);
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

            /// <summary>
            /// Mountains -> max
            /// </summary>
            Mountains,

            /// <summary>
            /// Hills -> Mountains
            /// </summary>
            Hills,

            /// <summary>
            /// LowHills -> Hills
            /// </summary>
            LowHills,

            /// <summary>
            /// 
            /// </summary>
            Mainland,

            /// <summary>
            /// 
            /// </summary>
            Coast,

            /// <summary>
            /// 
            /// </summary>
            ShallowWater,

            /// <summary>
            /// 
            /// </summary>
            DeepWater,

            /// <summary>
            /// Mainland, LowHills and Hills
            /// </summary>
            MainlandPlusAllHills,

            /// <summary>
            /// Hills and Mountains
            /// </summary>
            HillsAndUp,

            /// <summary>
            /// Coast, Mainland, LowHills
            /// </summary>
            CoastToLowHills
        }
    }
}