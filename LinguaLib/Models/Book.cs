using SQLite;

namespace LinguaLib.Models;

[Table("Books")]
public class Book
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Language { get; set; } = "en";
    public string Content { get; set; } = string.Empty;
    public string CoverEmoji { get; set; } = "📖";
    public string CoverUrl { get; set; } = string.Empty;
    public DateTime AddedDate { get; set; } = DateTime.Now;
    public int ReadProgress { get; set; } = 0;
}
