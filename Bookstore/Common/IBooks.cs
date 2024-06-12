using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Model;

namespace Common
{
    public interface IBooks
    {
        Dictionary<int, Book> GetBooks();
        bool CreateBook(string title, int publishmentYear, int authorId, string token);
        bool DeleteBook(Book book, string token);
        void CloneBook(Book book, string token);
        bool EditBook(int bookId, string title, int publishYear, int authorId, string token);
        bool LeaseBook(Book book, string token);
        bool ReturnBook(Book book, string token);
    }
}
