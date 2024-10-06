namespace Model
{
    ///
    public enum TravelMode
    {
        /// <summary>
        /// Travel not posible
        /// </summary>
        None = 0,

        /// <summary>
        /// Needs roads, steppe, savanna or plains 
        /// </summary>
        Carriage,

        /// <summary>
        /// Needed for steep clifs, Glatisers and tall moutains
        /// </summary>
        Mountaineering,

        /// <summary>
        /// used for disrupted travel
        /// </summary>
        Returning,

        /// <summary>
        /// Faster than walking, but not always posible
        /// </summary>
        Riding,

        /// <summary>
        /// For crosing the sea
        /// </summary>
        Sailing,

        /// <summary>
        /// Only for short destances
        /// </summary>
        Swiming,

        /// <summary>
        /// The default travelmode
        /// </summary>
        Walking,
    }
}