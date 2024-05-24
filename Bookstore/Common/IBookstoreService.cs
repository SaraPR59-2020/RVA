using Common.Model;
using System.Collections.Generic;
using System.ServiceModel;

namespace Common
{
    [ServiceContract]
    public interface IBookstoreService
    {
        [OperationContract]
        bool CreateBook(string title, int publishmentYear, int authorId);

        [OperationContract]
        bool CreateAuthor(string firstName, string lastName, string shortDesc);

        [OperationContract]
        bool CreateUser(string firstName, string lastName, string username, string password);

        [OperationContract]
        bool DeleteBook(int bookId);

        [OperationContract]
        void CloneBook(Book book);

        [OperationContract]
        bool EditBook(int bookId, string title, int authorId, int publishmentYear);

        [OperationContract]
        Dictionary<int, Book> GetBooks();

        [OperationContract]
        Member GetMemberInfo(string username);

        [OperationContract]
        bool EditMemberInfo(string username, string firstName, string lastName);

        [OperationContract]
        bool LogIn(string username, string password);

        [OperationContract]
        void LogOut(string username);
        
    }
}
