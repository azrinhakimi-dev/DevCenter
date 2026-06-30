using DevCenter.Models;
using DevCenter.Services;

namespace DevCenter.Repositories
{
    public class SnippetRepo(AppDbContext db)
    {
        public List<Snippet> GetAll() =>
            db.Snippets.OrderByDescending(s => s.Created).ToList();

        public void Add(Snippet s)
        {
            db.Snippets.Add(s);
            db.SaveChanges();
        }

        public void Update(Snippet s)
        {
            db.Snippets.Update(s);
            db.SaveChanges();
        }

        public void Delete(int id)
        {
            var s = db.Snippets.Find(id);
            if (s is null) return;
            db.Snippets.Remove(s);
            db.SaveChanges();
        }
    }
}
