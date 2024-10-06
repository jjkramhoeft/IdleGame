namespace Model
{
    /// <summary>
    /// 
    /// </summary>
    public enum GenerativeState
    {
        /// <summary>
        /// Pre creation
        /// </summary>
        None = 0,

        /// <summary>
        /// Initialization and Procedural math done
        /// </summary>
        Base = 2,

        /// <summary>
        /// First picture genrated
        /// </summary>
        PictureQueued = 3,

        /// <summary>
        /// Description for photogen done by LLM
        /// </summary>
        LlmPromptAdded = 4,

        /// <summary>
        /// First picture genrated
        /// </summary>
        PictureAdded = 5,

        /// <summary>
        /// All done - finnished.
        /// </summary>
        Done = 100
    }
}