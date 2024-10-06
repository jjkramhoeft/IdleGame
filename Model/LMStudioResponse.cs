namespace Model
{
    /// 
    public class LMStudioResponse()
    {
        /// 
        public string id { get; set; }="";
        /// 
        public string model { get; set; }="";
        /// 
        public LMStudioChoice[] choices { get; set; }=[];

    }
}