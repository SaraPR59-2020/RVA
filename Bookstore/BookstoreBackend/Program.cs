using Common.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreBackend
{
    class Program
    {
        static void Main(string[] args)
        {
            IUser user = new Member() { FirstName = "a", LastName = "b", Username = "a", Password = "b"};
            Author author = new Author() { FirstName = "Ivo", LastName = "Andric", AuthorId = 5, ShortDesc = "da" };
            Book book = new Book() { BookId = 1, Author = author.AuthorId, PublishYear = 1939, Title = "Na Drini Cuprija", Member = user.Username};

            Book clone = (Book)book.Clone();

            Console.WriteLine($"{book.Title} {book.PublishYear}");
            Console.WriteLine($"{clone.Title} {clone.PublishYear}");

            using(var db = new BookstoreDbContext())
            {
                List<Book> books = db.Books.ToList();
                foreach(var b in books)
                {
                    Console.WriteLine($"{b.Title}, {b.PublishYear}");
                }
            }



            Console.ReadKey();

        }
    }
}
