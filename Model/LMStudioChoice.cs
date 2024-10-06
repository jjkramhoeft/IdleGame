namespace Model
{
    /// 
    public class LMStudioChoice()
    {
        /// 
        public int index { get; set; }
        /// 
        public LMStudioMessage message { get; set; } = new();
        ///
        public string finish_reason { get; set; } = "";

    }
}