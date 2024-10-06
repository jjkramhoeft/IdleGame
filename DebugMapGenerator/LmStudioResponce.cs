namespace DebugMapGenerator
{
    public class LmStudioResponce()
    {
        public string Id { get; set; } = "";
        public string Object { get; set; } = "";
        public long Created { get; set; }
        public string Model { get; set; } = "";
        public List<Choise> Choises { get; set; } = [];
        public Usage Usage { get; set; } = new();

    }

    public class Choise()
    {
        public int Index { get; set; }
        public Message Message { get; set; } = new();
        public string Finish_reason { get; set; } = "";
    }

    public class Message()
    {
        public string Role { get; set; } = "";
        public string Content { get; set; } = "";
    }

    public class Usage()
    {
        public int Prompt_tokens { get; set; }
        public int Completion_tokens { get; set; }
        public int Total_tokens { get; set; }
    }
}
