using Common;
using Common.Log;
using Common.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookstoreBackend
{
    // verovatno treba izmeniti neke metode i dodati logger
    class BookstoreService : IBookstoreService
    {
        private static UserService userService = UserService.GetInstance();
        private Logger logger;

        public BookstoreService()
        {
            logger = new Logger("LogData.txt");
        }

        #region BOOK
        public Dictionary<int, Book> GetBooks()
        {
            using (var db = new BookstoreDbContext())
            {
                return db.Books.Include("Author").ToDictionary(b => b.BookId); //.Include("Member");
            }
        }

        public Dictionary<int, Book> GetAvailableBooks()
        {
            using (var db = new BookstoreDbContext())
            {
                return db.Books.Include("Author").Where(b => b.Member == null).ToDictionary(b => b.BookId);
            }
        }

        public bool CreateBook(string title, int publishmentYear, int authorId, string token)
        {
            Member member = userService.GetLoggedInUser(token);
            if (member == null) return false;
            if (!member.IsAdmin) return false;

            using (var db = new BookstoreDbContext())
            {
                if (db.Books.FirstOrDefault(b => b.Title == title && b.AuthorId == authorId) != null)
                {
                    return false;
                }

                db.Books.Add(new Book() { Title = title, AuthorId = authorId, PublishYear = publishmentYear }); // When you assign an ID it automatically connects it to the existing author
                db.SaveChanges();

                return true;
            }
        }

        public bool DeleteBook(Book book, string token)
        {
            Member member = userService.GetLoggedInUser(token);
            if (member == null) return false;
            if (!member.IsAdmin) return false;

            using (var db = new BookstoreDbContext())
            {
                Book b = db.Books.Find(book.BookId);

                if (b == null || b.Member != null)
                {
                    return false;
                }

                db.Books.Remove(b);
                db.SaveChanges();

                return true;
            }
        }

        public void CloneBook(Book book, string token)
        {
            Member member = userService.GetLoggedInUser(token);
            if (member == null) return;
            if (!member.IsAdmin) return;

            using (var db = new BookstoreDbContext())
            {
                Dictionary<int, Book> books = GetBooks();

                if (books.Values.FirstOrDefault(b => b.BookId == book.BookId) == null)
                {
                    return;
                }

                db.Books.Add(new Book() { Title = book.Title, AuthorId = book.AuthorId, PublishYear = book.PublishYear }); // When you assign an ID it automatically connects it to the existing author
                db.SaveChanges();
            }
        }

        public bool EditBook(Book book, string token)
        {
            Member member = userService.GetLoggedInUser(token);
            if (member == null) return false;
            if (!member.IsAdmin) return false;

            using (var db = new BookstoreDbContext())
            {
                Book b = db.Books.Find(book.BookId);

                if (b == null)
                {
                    return false;
                }

                Author author = db.Authors.Find(book.Author.AuthorId);
                if (author == null)
                {
                    author = new Author() { FirstName = book.Author.FirstName, LastName = book.Author.LastName, ShortDesc = book.Author.ShortDesc };
                }

                b.Title = book.Title;
                b.PublishYear = book.PublishYear;
                b.Author = author;
                db.SaveChanges();

                return true;
            }
        }

        public bool LeaseBook(Book book, string token)
        {
            if(book.Member != null) return false;

            Member member = userService.GetLoggedInUser(token);
            if (member == null) return false;

            using (var db = new BookstoreDbContext())
            {
                Book b = db.Books.Find(book.BookId);

                if (b == null)
                {
                    return false;
                }

                b.Member = member;
                db.SaveChanges();

                return true;
            }
        }

        public bool ReturnBook(Book book, string token)
        {
            Member member = userService.GetLoggedInUser(token);
            if (member == null) return false;

            using (var db = new BookstoreDbContext())
            {
                Book b = db.Books.Find(book.BookId);

                if (b == null)
                {
                    return false;
                }

                b.Member = null;
                db.SaveChanges();

                return true;
            }
        }

        #endregion

        #region AUTHOR
        public Author CreateAuthor(string firstName, string lastName, string shortDesc, string token)
        {
            Member member = userService.GetLoggedInUser(token);
            if (member == null) return null;
            if (!member.IsAdmin) return null;

            using (var db = new BookstoreDbContext())
            {
                if (db.Authors.FirstOrDefault(a => a.FirstName == firstName && a.LastName == lastName) != null)
                {
                    return null;
                }

                Author author = new Author() { FirstName = firstName, LastName = lastName, ShortDesc = shortDesc };

                db.Authors.Add(author);
                db.SaveChanges();

                return author;
            }
        }

        public List<Author> GetAuthors()
        {
            using (var db = new BookstoreDbContext())
            {
                return db.Authors.ToList();
            }
        }

        #endregion

        #region USER

        public string LogIn(string username, string password)
        {
            try
            {
                string user = userService.LoginUser(username, password);
                logger.Log("User " + username + " connected.", LogLevel.INFO, username);
                return user;
            } 
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                logger.Log("Failed login attempt for user " + username, LogLevel.WARN, username);
                return null;
            }
        }

        public void LogOut(string token)
        {
            if (userService.IsMemberLoggedIn(token))
            {
                Member user = userService.GetLoggedInUser(token);
                userService.LogoutUser(token);
                logger.Log("User " + user.Username + " has disconnected.", LogLevel.INFO, user.Username);
            }
        }

        public bool CreateUser(string firstName, string lastName, string username, string password, bool admin, string token)
        {
            Member member = userService.GetLoggedInUser(token);
            if (member == null) return false;
            if (!member.IsAdmin) return false;

            using (var db = new BookstoreDbContext())
            {
                if (db.Members.Find(username) != null)
                {
                    return false;
                }

                Member m = new Member() { Username = username, Password = password, FirstName = firstName, LastName = lastName, IsAdmin = admin };
                db.Members.Add(m);
                db.SaveChanges();

                logger.Log("User " + username + " created.", LogLevel.INFO, username);

                return true;
            }
        }

        public bool EditMemberInfo(string firstName, string lastName, string token)
        {
            var member = userService.GetLoggedInUser(token);
            if (member == null) return false;

            using (var db = new BookstoreDbContext())
            {
                Member user = db.Members.FirstOrDefault(m => m.Username == member.Username);
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

        public Member GetMemberInfo(string token)
        {
            var member = userService.GetLoggedInUser(token);
            if (member == null) return null;

            using (var db = new BookstoreDbContext())
            {
                Member user = db.Members.FirstOrDefault(m => m.Username == member.Username);
                return user;
            }
        }

        #endregion
    }
}
