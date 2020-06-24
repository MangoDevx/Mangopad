namespace Mangopad.Model
{
    public class NotesModel
    {
        public Note[] Notes {get; set;}
    }
    public class Note
    {
        public string Path { get; set; }
        public string Name {get; set;}
        public bool Default { get; set; }
    }
}
