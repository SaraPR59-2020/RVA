using Common;
using Common.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookstoreBackend
{
    // verovatno treba izmeniti neke metode i dodati logger
    class BookstoreService : IBookstoreService
    {
        //private static List<string> activeUsers = new List<string>();
        private static UserService userService = UserService.GetInstance();

        public bool CreateBook(string title, int publishmentYear, Guid authorId)
        {
            using (var db = new BookstoreDbContext())
            {
                if (db.Books.FirstOrDefault(b => b.Title == title && b.Author == authorId) != null)
                {
                    return false;
                }

                db.Books.Add(new Book() { Title = title, Author = authorId, PublishYear = publishmentYear, Member = string.Empty });
                db.SaveChanges();

                return true;
            }
        }

        public bool CreateAuthor(string firstName, string lastName, string shortDesc)
        {
            using (var db = new BookstoreDbContext())
            {
                if (db.Authors.FirstOrDefault(a => a.FirstName == firstName && a.LastName == lastName) != null)
                {
                    return false;
                }

                db.Authors.Add(new Author() { FirstName = firstName, LastName = lastName, ShortDesc = shortDesc });
                db.SaveChanges();

                return true;
            }
        }

        public bool CreateUser(string firstName, string lastName, string username, string password)
        {
            using (var db = new BookstoreDbContext())
            {
                if (db.Members.Find(username) != null)
                {
                    return false;
                }

                Member m = new Member() { Username = username, Password = password, FirstName = firstName, LastName = lastName };
                db.Members.Add(m);
                db.SaveChanges();

                return true;
            }
        }

        public bool DeleteBook(Guid bookId)
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

        public void CloneBook(Book book)
        {
            using (var db = new BookstoreDbContext())
            {
                Dictionary<Guid, Book> books = GetBooks();

                Book toClone = books.Values.FirstOrDefault(b => b.BookId == book.BookId);
                if (toClone == null)
                {
                    return;
                }

                Book clone = (Book)toClone.Clone();
                clone.Title += " (Copy)";

                db.Books.Add(clone);
                db.SaveChanges();
            }
        }

        public bool EditBook(Guid bookId, string title, Guid authorId, int publishmentYear)
        {
            using (var db = new BookstoreDbContext())
            {
                Book b = db.Books.Find(bookId);

                if (b == null)
                {
                    return false;
                }

                b.Title = title;
                b.PublishYear = publishmentYear;
                b.Author = authorId;
                db.SaveChanges();

                return true;
            }
        }

        public Dictionary<Guid, Book> GetBooks()
        {
            using (var db = new BookstoreDbContext())
            {
                return db.Books.ToDictionary(b => b.BookId);
            }
        }

        public IUser GetMemberInfo(string username)
        {
            using (var db = new BookstoreDbContext())
            {
                IUser user = db.Members.FirstOrDefault(m => m.Username == username);

                if (user == null)
                {
                    user = db.Admins.FirstOrDefault(m => m.Username == username);
                }

                return user;
            }
        }

        public bool EditMemberInfo(string username, string firstName, string lastName)
        {
            using (var db = new BookstoreDbContext())
            {
                IUser user = db.Members.FirstOrDefault(m => m.Username == username);

                if (user == null)
                {
                    user = db.Admins.FirstOrDefault(m => m.Username == username);
                }
                if (user == null)
                {
                    return false;
                }

                user.FirstName = firstName;
                user.LastName = lastName;
                db.SaveChanges();

                return true;
            }
        }

        public string LogIn(string username, string password)
        {
            try
            {
                return userService.LoginUser(username, password);
            } 
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public void LogOut()
        {
            return;
        }
    }
}
