using SQLite;
using LinguaLib.Models;

namespace LinguaLib.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection? _db;

    private async Task InitAsync()
    {
        if (_db is not null) return;
        var path = Path.Combine(FileSystem.AppDataDirectory, "lingualib.db3");
        _db = new SQLiteAsyncConnection(path);
        await _db.CreateTableAsync<Book>();
        await _db.CreateTableAsync<UserWord>();
        await SeedBooksAsync();
    }

    private async Task SeedBooksAsync()
    {
        var count = await _db!.Table<Book>().CountAsync();
        if (count > 0) return;

        var books = new List<Book>
        {
            new() { Title = "The Little Prince", Author = "Antoine de Saint-Exupéry", Language = "en", CoverEmoji = "🌹",
                Content = "Once when I was six years old I saw a magnificent picture in a book about the primeval forest. It was a picture of a boa constrictor in the act of swallowing an animal. Here is a copy of the drawing. In the book it said: 'Boa constrictors swallow their prey whole, without chewing it. After that they are not able to move, and they sleep through the six months that they need for digestion.' I pondered deeply, then, over the adventures of the jungle. And after some work with a colored pencil I succeeded in making my first drawing. I showed my masterpiece to the grown-ups, and asked them whether the drawing frightened them. But they answered: 'Frighten? Why should any one be frightened by a hat?' My drawing was not a picture of a hat. It was a picture of a boa constrictor digesting an elephant." },
            new() { Title = "Alice in Wonderland", Author = "Lewis Carroll", Language = "en", CoverEmoji = "🐇",
                Content = "Alice was beginning to get very tired of sitting by her sister on the bank, and of having nothing to do: once or twice she had peeped into the book her sister was reading, but it had no pictures or conversations in it, 'and what is the use of a book,' thought Alice 'without pictures or conversations?' So she was considering in her own mind (as well as she could, for the hot day made her feel very sleepy and stupid), whether the pleasure of making a daisy-chain would be worth the trouble of getting up and picking the daisies, when suddenly a White Rabbit with pink eyes ran close by her." },
            new() { Title = "Väike Prints", Author = "Antoine de Saint-Exupéry", Language = "et", CoverEmoji = "⭐",
                Content = "Kui ma olin kuueaastane, nägin ma ühes raamatus, mis käsitles algmetsasid, suurepärast pilti. See kujutas boakonstrikterit, kes neelab alla metslooma. Raamatus seisis: 'Boakonstrikterid neelavad oma saagi alla tervena, ilma närimata. Seejärel ei suuda nad enam liikuda ja magavad kuus kuud, mis neil seedimiseks kulub.' Mõtlesin siis pikalt džungliseikluste peale. Ja peale värvipliiatsitega nokitsemist õnnestuski mul teha oma esimene joonistus." },
        };
        await _db.InsertAllAsync(books);
    }

    // Books
    public async Task<List<Book>> GetBooksAsync()
    {
        await InitAsync();
        return await _db!.Table<Book>().ToListAsync();
    }

    public async Task<Book?> GetBookAsync(int id)
    {
        await InitAsync();
        return await _db!.Table<Book>().Where(b => b.Id == id).FirstOrDefaultAsync();
    }

    public async Task<int> SaveBookAsync(Book book)
    {
        await InitAsync();
        return book.Id == 0 ? await _db!.InsertAsync(book) : await _db!.UpdateAsync(book);
    }

    public async Task DeleteBookAsync(Book book)
    {
        await InitAsync();
        await _db!.DeleteAsync(book);
    }

    // UserWords
    public async Task<List<UserWord>> GetUserWordsAsync()
    {
        await InitAsync();
        return await _db!.Table<UserWord>().ToListAsync();
    }

    public async Task<int> SaveUserWordAsync(UserWord word)
    {
        await InitAsync();
        return word.Id == 0 ? await _db!.InsertAsync(word) : await _db!.UpdateAsync(word);
    }

    public async Task DeleteUserWordAsync(UserWord word)
    {
        await InitAsync();
        await _db!.DeleteAsync(word);
    }
}
