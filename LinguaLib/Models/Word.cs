using SQLite;

namespace LinguaLib.Models;

[Table("Words")]
public class Word
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Original { get; set; } = string.Empty;
    public string Translation { get; set; } = string.Empty;
    public string Transcription { get; set; } = string.Empty;
    public string Language { get; set; } = "en";
    public int BookId { get; set; }
}
