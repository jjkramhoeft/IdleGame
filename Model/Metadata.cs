namespace Model
{
    ///
    public class Metadata(int counter, int objId, int dbId, string name, string description, Box box, GenerativeState dbGenerativeState, GenerativeState objGenerativeState, string prompt)
    {
        ///
        public int Counter { get; set; } = counter;

        /// 
        public int ObjId { get; set; } = objId;

        /// 
        public int DbId { get; set; } = dbId;

        ///
        public string Name { get; set; } = name;

        ///
        public string Description { get; set; } = description;

        ///
        public string Prompt { get; set; } = prompt;

        ///
        public Box Box { get; set; } = box;

        ///        
        public GenerativeState DbGenerativeState { get; set; } = dbGenerativeState;

        ///        
        public GenerativeState ObjGenerativeState { get; set; } = objGenerativeState;
    }
}