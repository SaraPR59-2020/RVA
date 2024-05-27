using Common.Model;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace Common
{
    [ServiceContract]
    public interface IBookstoreService
    {
        [OperationContract]
        bool CreateBook(string title, int publishmentYear, Guid authorId);

        [OperationContract]
        bool CreateAuthor(string firstName, string lastName, string shortDesc);

        [OperationContract]
        bool CreateUser(string firstName, string lastName, string username, string password);

        [OperationContract]
        bool DeleteBook(Guid bookId);

        [OperationContract]
        void CloneBook(Book book);

        [OperationContract]
        bool EditBook(Guid bookId, string title, Guid authorId, int publishmentYear);

        [OperationContract]
        Dictionary<Guid, Book> GetBooks();

        [OperationContract]
        IUser GetMemberInfo(string username);

        [OperationContract]
        bool EditMemberInfo(string username, string firstName, string lastName);

        [OperationContract]
        string LogIn(string username, string password);

        [OperationContract]
        void LogOut();
        
    }
}
