using DevCenter.Models;
using DevCenter.Services;

namespace DevCenter.Repositories
{
    public class DevCommandRepo(AppDbContext db)
    {
        public List<DevCommand> GetAll() =>
        db.DevCommands.OrderByDescending(c => c.Created).ToList();

        public void Add(DevCommand c)
        {
            db.DevCommands.Add(c);
            db.SaveChanges();
        }

        public void Update(DevCommand c)
        {
            db.DevCommands.Update(c);
            db.SaveChanges();
        }

        public void Delete(int id)
        {
            var c = db.DevCommands.Find(id);
            if (c is null) return;
            db.DevCommands.Remove(c);
            db.SaveChanges();
        }
    }
}
