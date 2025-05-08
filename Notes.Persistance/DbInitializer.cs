using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notes.Persistance
{
    public class DbInitializer
    {
        public static void Initialize(NotesDbContext context)
        {
            context.Database.EnsureCreated();
            //if (context.Notes.Any())
            //{
            //    return; // DB has been seeded
            //}
            //var notes = new List<Note>
            //{
            //    new Note { Title = "First Note", Content = "This is the first note." },
            //    new Note { Title = "Second Note", Content = "This is the second note." }
            //};
            //context.Notes.AddRange(notes);
            //context.SaveChanges();
        }
    }
}
