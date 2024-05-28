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
        private static UserService userService = UserService.GetInstance();

        #region BOOK
        public Dictionary<int, Book> GetBooks()
        {
            using (var db = new BookstoreDbContext())
            {
                return db.Books.Include("Author").ToDictionary(b => b.BookId); //.Include("Member");
            }
        }

        public bool CreateBook(string title, int publishmentYear, Author author, string token)
        {
            Member member = userService.GetLoggedInUser(token);
            if (member == null) return false;
            if (!member.IsAdmin) return false;

            using (var db = new BookstoreDbContext())
            {
                if (db.Books.FirstOrDefault(b => b.Title == title && b.AuthorId == author.AuthorId) != null)
                {
                    return false;
                }

                db.Books.Add(new Book() { Title = title, Author = author, PublishYear = publishmentYear });
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

                if (b == null)
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

        #endregion

        #region USER

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

        public void LogOut(string token)
        {
            if (userService.IsMemberLoggedIn(token))
            {
                userService.LogoutUser(token);
            }
        }

        public bool CreateUser(string firstName, string lastName, string username, string password, string token)
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

                Member m = new Member() { Username = username, Password = password, FirstName = firstName, LastName = lastName };
                db.Members.Add(m);
                db.SaveChanges();

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
