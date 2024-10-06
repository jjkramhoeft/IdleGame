namespace Model
{
    ///
    public class RegionRequest
    {
        ///
        public required Box Box { get; set; }

        ///        
        public GenerativeState CurrentGenerativeState { get; set; }
    }
}