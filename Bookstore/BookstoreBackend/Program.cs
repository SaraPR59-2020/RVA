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
            Book book = new Book() { BookId = 1, Author = author, PublishYear = 1939, Title = "Na Drini Cuprija", Member = (Member)user};

            Book clone = (Book)book.Clone();

            Console.WriteLine($"{book.Author.FirstName} {book.Author.LastName}");
            Console.WriteLine($"{clone.Author.FirstName} {clone.Author.LastName}");

            Console.ReadKey();

        }
    }
}
