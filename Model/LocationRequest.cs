namespace Model
{
    ///
    public class LocationRequest
    {
        ///
        public required Box Box { get; set; }

        ///        
        public GenerativeState CurrentGenerativeState { get; set; }

        ///
        public DateTime? RequestTime { get; set; }
    }
}