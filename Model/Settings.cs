using System.Text.Json.Serialization;

namespace Model
{
    /// <summary>
    /// Cross solution configurations
    /// </summary>
    public static class Settings
    {
        /// <summary>
        /// Indicates if the game front-end is running in debug mode (no calls to WorldApi)
        /// Default should be: false
        /// No implemented yet
        /// </summary>        
        public static bool GameUIDebugMode { get; } = false;

        /// <summary>
        /// Local and Private LLM Server is on and running
        /// Default should be: true
        /// </summary>        
        public static bool IsOnLLM { get; } = true;

        /// <summary>
        /// Local and Private AI Image Generator is on and running
        /// Currently using Comfy UI
        /// Default should be: true
        /// </summary>        
        public static bool IsOnComfyUI { get; } = true;

        /// <summary>
        /// Location type and person generation is overwritten to facilitat debugging of many populated locations and large variation of people
        /// Default should be: false
        /// </summary>        
        public static bool ExtremeVariance { get; } = true;

        /// <summary>
        /// Location generation for unpopulated locations are skipped (to speed up debuging)
        /// - will force uninhabited locations to 'Done', to save LLM and Picture run time
        /// Default should be: false
        /// </summary>        
        public static bool SkipUninhabitedLocations { get; } = false;

        /// <summary>
        /// Backend used for LLM
        /// Default should be: LMStudio
        /// </summary>        
        public static LLMServerTypes LLMServerType { get; } = LLMServerTypes.LMStudio;
        
        /// <summary>
        /// Backend used for AI Image Generation
        /// Default should be: Standalone
        /// </summary>        
        public static ComfyUISetups ComfyUISetup { get; } = ComfyUISetups.Standalone;
    }

    ///
    public enum LLMServerTypes
    {
        /// 
        None = 0,
        ///   
        Ollama,
        ///     
        LMStudio,
    }

    ///
    public enum ComfyUISetups
    {
        /// 
        None = 0,
        ///   
        Standalone,
        ///     
        StandaloneNextSchnell,
    }
}