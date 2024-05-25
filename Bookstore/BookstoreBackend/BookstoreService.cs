using Common;
using Common.Model;
using System.Collections.Generic;
using System.Linq;

namespace BookstoreBackend
{
    // verovatno treba izmeniti neke metode i dodati logger
    class BookstoreService : IBookstoreService
    {
        private static List<string> activeUsers = new List<string>();
        //private static UserService = new UserService();
        public void CloneBook(Book book)
        {
            Dictionary<int, Book> books = GetBooks();

            Book toClone = books.Values.FirstOrDefault(b => b.BookId == book.BookId);
            if (toClone == null)
            {
                return;
            }

            Book clone = (Book)toClone.Clone();

            using (var db = new BookstoreDbContext())
            {
                clone.Title += " (Copy)";
                clone.BookId = 0;

                db.Books.Add(clone);
                db.SaveChanges();
            }
        }

        public bool CreateAuthor(string firstName, string lastName, string shortDesc)
        {
            using (var db = new BookstoreDbContext())
            {
                db.Authors.Add(new Author() { AuthorId = 0, FirstName = firstName, LastName = lastName, ShortDesc = shortDesc });
                return true;
            }
        }

        public bool CreateBook(string title, int publishmentYear, int authorId)
        {
            using (var db = new BookstoreDbContext())
            {
                /*if (db.Books.Find(title) != null)
                {
                    return false;
                }*/

                db.Books.Add(new Book() { BookId = 0, Title = title, Author = authorId, PublishYear = publishmentYear, Member = string.Empty });
                db.SaveChanges();

                return true;
            }
        }

        public bool CreateUser(string firstName, string lastName, string username, string password)
        {
            using (var db = new BookstoreDbContext())
            {
                if (db.Members.Find(username) != null)
                    return false;

                Member m = new Member() { Username = username, Password = password, FirstName = firstName, LastName = lastName };
                db.Members.Add(m);
                db.SaveChanges();
            }

            return true;
        }

        public bool DeleteBook(int bookId)
        {
            using (var db = new BookstoreDbContext())
            {
                Book b = db.Books.Find(bookId);

                if (b == null)
                {
                    return false;
                }

                db.Books.Remove(b);
                db.SaveChanges();

                return true;
            }
        }

        public bool EditBook(int bookId, string title, int authorId, int publishmentYear)
        {
            using (var db = new BookstoreDbContext())
            {
                Book b = db.Books.Find(bookId);

                if (b == null)
                {
                    return false;
                }

                db.Books.Remove(b);
                db.SaveChanges();

                return CreateBook(title, authorId, publishmentYear);
            }
        }

        public bool EditMemberInfo(string username, string firstName, string lastName)
        {
            IUser user = null;

            using (var db = new BookstoreDbContext())
            {
                user = db.Members.FirstOrDefault(m => m.Username == username);

                if (user == null)
                    user = db.Admins.FirstOrDefault(m => m.Username == username);

                if (user == null)
                    return false;

                user.FirstName = firstName;
                user.LastName = lastName;

                db.SaveChanges();

                return true;
            }
        }

        public Dictionary<int, Book> GetBooks()
        {
            using (var db = new BookstoreDbContext())
            {
                return db.Books.Include("Author").ToDictionary(b => b.BookId);
            }
        }

        public Member GetMemberInfo(string username)
        {
            IUser user = null;

            using (var db = new BookstoreDbContext())
            {
                user = db.Members.FirstOrDefault(m => m.Username == username);

                if (user == null)
                    user = db.Admins.FirstOrDefault(m => m.Username == username);
            }

            if (user == null)
                return null;

            return new Member() { Username = user.Username, FirstName = user.FirstName, LastName = user.LastName };
        }

        public bool LogIn(string username, string password)
        {
            using (var db = new BookstoreDbContext())
            {
                IUser user = db.Members.FirstOrDefault(u => u.Username.Equals(username));

                if (user == null)
                    user = db.Admins.FirstOrDefault(a => a.Username.Equals(username));

                if (user == null)
                    return false;

                if (activeUsers.Contains(username))
                    return false;

                if (user.Password == password)
                {
                    activeUsers.Add(username);

                    return true;
                }

                return false;
            }
        }

        public void LogOut(string username)
        {
            activeUsers.Remove(username);
        }
    }
}
