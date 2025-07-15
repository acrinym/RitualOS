namespace RitualOS.Models
{
    /// <summary>
    /// Represents a document loaded for viewing within the app.
    /// </summary>
    public class DocumentFile
    {
        public string FilePath { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}
