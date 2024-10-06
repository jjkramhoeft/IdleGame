namespace Model
{
    /// 
    public class ComfyHistoryResponse()
    {
        /// 
        public Dictionary<int,ComfyImages>? outputs {get;set;}
        /// 
        public ComfyStatus? status {get;set;} 

    }
}