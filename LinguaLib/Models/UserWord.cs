using SQLite;

namespace LinguaLib.Models;

[Table("UserWords")]
public class UserWord
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Original { get; set; } = string.Empty;
    public string Translation { get; set; } = string.Empty;
    public string Transcription { get; set; } = string.Empty;
    public int KnowledgeLevel { get; set; } = 0; // 0=new, 1=learning, 2=known
    public DateTime AddedDate { get; set; } = DateTime.Now;
    public DateTime NextReview { get; set; } = DateTime.Now;
    public bool IsFlipped { get; set; } = false;
}
