using Common.Model;
using System.Collections.Generic;
using System.ServiceModel;

namespace Common
{
    [ServiceContract]
    public interface IBookstoreService
    {
        [OperationContract]
        bool CreateBook(string title, int publishmentYear, int authorId, string token);

        [OperationContract]
        bool CreateAuthor(string firstName, string lastName, string shortDesc, string token);

        [OperationContract]
        bool CreateUser(string firstName, string lastName, string username, string password);

        [OperationContract]
        bool DeleteBook(int bookId, string token);

        [OperationContract]
        void CloneBook(Book book, string token);

        [OperationContract]
        bool EditBook(int bookId, string title, int authorId, int publishmentYear, string token);

        [OperationContract]
        Dictionary<int, Book> GetBooks();

        [OperationContract]
        Member GetMemberInfo(string username);

        [OperationContract]
        bool EditMemberInfo(string token, string firstName, string lastName);

        [OperationContract]
        string LogIn(string username, string password);

        [OperationContract]
        void LogOut(string token);
        
    }
}
