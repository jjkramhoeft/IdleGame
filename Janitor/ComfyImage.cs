namespace Janitor
{
    public class ComfyImage(Guid comfyId, int id, Generator.PictureGenerator.MotiveType type)
    {
        public Guid ComfyId { get; set; } = comfyId;
        public int Id { get; set; } = id;
        public Generator.PictureGenerator.MotiveType PictureType { get; set; } = type;
        public DateTimeOffset RequestTime { get; set; } = DateTimeOffset.Now;
        public string Status { get; set; } = "";
        public string ComfyFullPath { get; set; } = "";
        public string StoreFullPath { get; set; } = "";
    }
}