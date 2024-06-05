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
                logger.Log("Book data querried.", LogLevel.DEBUG, "");
                return db.Books.Include("Author").ToDictionary(b => b.BookId); //.Include("Member");
            }
        }

        public bool CreateBook(string title, int publishmentYear, int authorId, string token)
        {
            Member member = userService.GetLoggedInUser(token);
            if (member == null)
            {
                logger.Log("Failed to create book.", LogLevel.WARN, "");
                return false;
            }
            if (!member.IsAdmin)
            {
                logger.Log("Failed to create book.", LogLevel.WARN, member.Username);
                return false;
            }

            using (var db = new BookstoreDbContext())
            {
                if (db.Books.FirstOrDefault(b => b.Title == title && b.AuthorId == authorId) != null)
                {
                    logger.Log("Failed to create book.", LogLevel.WARN, member.Username);
                    return false;
                }

                db.Books.Add(new Book() { Title = title, AuthorId = authorId, PublishYear = publishmentYear }); // When you assign an ID it automatically connects it to the existing author
                db.SaveChanges();

                logger.Log(member.Username + " successfully added book.", LogLevel.INFO, member.Username);
                return true;
            }
        }

        public bool DeleteBook(Book book, string token)
        {
            Member member = userService.GetLoggedInUser(token);
            if (member == null)
            {
                logger.Log("Failed to delete book.", LogLevel.WARN, "");
                return false;
            }
            if (!member.IsAdmin)
            {
                logger.Log("Failed to delete book.", LogLevel.WARN, member.Username);
                return false;
            }

            using (var db = new BookstoreDbContext())
            {
                Book b = db.Books.Find(book.BookId);

                if (b == null || b.Member != null)
                {
                    logger.Log("Failed to delete book.", LogLevel.WARN, member.Username);
                    return false;
                }

                db.Books.Remove(b);
                db.SaveChanges();

                logger.Log(member.Username + " successfully deleted book.", LogLevel.INFO, member.Username);
                return true;
            }
        }

        public void CloneBook(Book book, string token)
        {
            Member member = userService.GetLoggedInUser(token);
            if (member == null)
            {
                logger.Log("Failed to duplicate book.", LogLevel.WARN, "");
                return;
            }
            if (!member.IsAdmin)
            {
                logger.Log("Failed to duplicate book.", LogLevel.WARN, member.Username);
                return;
            }

            using (var db = new BookstoreDbContext())
            {
                Dictionary<int, Book> books = GetBooks();

                if (books.Values.FirstOrDefault(b => b.BookId == book.BookId) == null)
                {
                    logger.Log("Failed to duplicate book.", LogLevel.WARN, member.Username);
                    return;
                }

                db.Books.Add(new Book() { Title = book.Title, AuthorId = book.AuthorId, PublishYear = book.PublishYear }); // When you assign an ID it automatically connects it to the existing author
                db.SaveChanges();
                logger.Log(member.Username + " successfully duplicated a book.", LogLevel.INFO, member.Username);
            }
        }

        public bool EditBook(int bookId, string title, int publishYear, int authorId, string token)
        {
            Member member = userService.GetLoggedInUser(token);
            if (member == null)
            {
                logger.Log("Failed to edit book.", LogLevel.WARN, "");
                return false;
            }
            if (!member.IsAdmin)
            {
                logger.Log("Failed to edit book.", LogLevel.WARN, member.Username);
                return false;
            }

            using (var db = new BookstoreDbContext())
            {
                Book b = db.Books.Find(bookId);

                if (b == null)
                {
                    logger.Log("Failed to edit book.", LogLevel.WARN, member.Username);
                    return false;
                }

                b.Title = title;
                b.PublishYear = publishYear;
                b.AuthorId = authorId;
                db.SaveChanges();

                logger.Log(member.Username + " successfully edited a book.", LogLevel.INFO, member.Username);
                return true;
            }
        }

        public bool LeaseBook(Book book, string token)
        {
            Member member = userService.GetLoggedInUser(token);
            if (member == null)
            {
                logger.Log("Failed to lease a book.", LogLevel.WARN, "");
                return false;
            }
            if (book.Member != null)
            {
                logger.Log("Failed to lease a book.", LogLevel.WARN, member.Username);
                return false;
            }

            using (var db = new BookstoreDbContext())
            {
                Book b = db.Books.Find(book.BookId);

                if (b == null)
                {
                    logger.Log("Failed to lease a book.", LogLevel.WARN, member.Username);
                    return false;
                }

                b.Username = member.Username;
                db.SaveChanges();

                logger.Log(member.Username + " successfully leased a book.", LogLevel.INFO, member.Username);
                return true;
            }
        }

        public bool ReturnBook(Book book, string token)
        {
            Member member = userService.GetLoggedInUser(token);
            if (member == null)
            {
                logger.Log("Failed to return a book.", LogLevel.WARN, "");
                return false;
            }

            using (var db = new BookstoreDbContext())
            {
                Book b = db.Books.Find(book.BookId);

                if (b == null)
                {
                    logger.Log("Failed to return a book.", LogLevel.WARN, member.Username);
                    return false;
                }

                b.Username = null;
                db.SaveChanges();

                logger.Log(member.Username + " successfully returned a book.", LogLevel.INFO, member.Username);
                return true;
            }
        }

        #endregion

        #region AUTHOR
        public Author CreateAuthor(string firstName, string lastName, string shortDesc, string token)
        {
            Member member = userService.GetLoggedInUser(token);
            if (member == null)
            {
                logger.Log("Failed to create author.", LogLevel.WARN, "");
                return null;
            }
            if (!member.IsAdmin)
            {
                logger.Log("Failed to create author.", LogLevel.WARN, member.Username);
                return null;
            }

            using (var db = new BookstoreDbContext())
            {
                if (db.Authors.FirstOrDefault(a => a.FirstName == firstName && a.LastName == lastName) != null)
                {
                    logger.Log("Failed to create author.", LogLevel.WARN, member.Username);
                    return null;
                }

                Author author = new Author() { FirstName = firstName, LastName = lastName, ShortDesc = shortDesc };

                db.Authors.Add(author);
                db.SaveChanges();

                logger.Log(member.Username + " successfully created author.", LogLevel.INFO, member.Username);
                return author;
            }
        }

        public List<Author> GetAuthors()
        {
            using (var db = new BookstoreDbContext())
            {
                logger.Log("Author data querried.", LogLevel.DEBUG, "");
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
            if (member == null)
            {
                logger.Log("Failed to create user " + username + ".", LogLevel.WARN, username);
                return false;
            }
            if (!member.IsAdmin)
            {
                logger.Log("Failed to create user " + username + ".", LogLevel.WARN, username);
                return false;
            }

            using (var db = new BookstoreDbContext())
            {
                if (db.Members.Find(username) != null)
                {
                    logger.Log("Failed to create user " + username + ".", LogLevel.WARN, username);
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
            if (member == null)
            {
                logger.Log("Failed to edit member info.", LogLevel.WARN, "");
                return false;
            }

            using (var db = new BookstoreDbContext())
            {
                Member user = db.Members.FirstOrDefault(m => m.Username == member.Username);
                if (user == null)
                {
                    logger.Log("Failed to edit member info.", LogLevel.WARN, member.Username);
                    return false;
                }

                user.FirstName = firstName;
                user.LastName = lastName;
                db.SaveChanges();

                logger.Log(member.Username + " info edited.", LogLevel.DEBUG, member.Username);
                return true;
            }
        }

        public Member GetMemberInfo(string token)
        {
            var member = userService.GetLoggedInUser(token);
            if (member == null)
            {
                logger.Log("Failed to get member info.", LogLevel.WARN, "");
                return null;
            }

            using (var db = new BookstoreDbContext())
            {
                Member user = db.Members.FirstOrDefault(m => m.Username == member.Username);
                logger.Log("Retrieved info of " + member.Username + ".", LogLevel.DEBUG, member.Username);
                return user;
            }
        }

        #endregion
    }
}
