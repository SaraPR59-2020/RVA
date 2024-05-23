using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Model;

namespace BookstoreBackend
{
    public class BookstoreDbContext : DbContext
    {
        public DbSet<Author> Authors { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Book> Books { get; set; }

        protected override void OnModelCreating(DbModelBuilder dbModelBuilder)
        {
            Database.SetInitializer<BookstoreDbContext>(new CreateDatabaseIfNotExists<BookstoreDbContext>()); // null instead of CreateIfNotExist?
            base.OnModelCreating(dbModelBuilder);
        }
    }
}
