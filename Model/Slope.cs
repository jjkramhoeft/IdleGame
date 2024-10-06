namespace Model
{
    /// <summary>
    /// For filters.    
    /// </summary>
    public class Slope
    {
        /// <summary>
        /// Higst posibel in this world 
        /// </summary>
        public const float Max = 1.8f;// more for real but caped

        /// <summary>
        /// Lowest posibel in this world 
        /// </summary>
        public const float Min = 0f;

        /// <summary>
        /// 
        /// </summary>
        public const float FlatNormalSplit = 0.2f;

        /// <summary>
        /// 
        /// </summary>
        public const float NormalSteepSplit = 1.2f;

        /// <summary>
        /// for filters    
        /// </summary>
        public static float GetIntervalLow(NamedInterval interval) => interval switch
        {
            NamedInterval.All => Min,
            NamedInterval.Flat => Min,
            NamedInterval.Normal => FlatNormalSplit,
            NamedInterval.FlatOrNormal => Min,
            NamedInterval.Steep => NormalSteepSplit,
            _ => Min
        };

        /// <summary>
        /// for filters    
        /// </summary>
        public static float GetIntervalHigh(NamedInterval interval) => interval switch
        {
            NamedInterval.All => Max,
            NamedInterval.Flat => FlatNormalSplit,
            NamedInterval.Normal => NormalSteepSplit,
            NamedInterval.FlatOrNormal => NormalSteepSplit,
            NamedInterval.Steep => Max,
            _ => Max
        };

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
            Flat,

            /// <summary>
            /// 
            /// </summary>
            Normal,

            /// <summary>
            /// 
            /// </summary>
            Steep,

            /// <summary>
            /// 
            /// </summary>
            FlatOrNormal,
        }
    }
}