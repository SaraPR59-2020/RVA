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
        bool CreateBook(string title, int publishmentYear, Author author, string token);

        [OperationContract]
        Author CreateAuthor(string firstName, string lastName, string shortDesc, string token);

        [OperationContract]
        bool CreateUser(string firstName, string lastName, string username, string password, string token);

        [OperationContract]
        bool DeleteBook(Book book, string token);

        [OperationContract]
        void CloneBook(Book book, string token);

        [OperationContract]
        bool EditBook(Book book, string token);

        [OperationContract]
        Dictionary<int, Book> GetBooks();

        [OperationContract]
        Member GetMemberInfo(string token);

        [OperationContract]
        bool EditMemberInfo(string firstName, string lastName, string token);

        [OperationContract]
        string LogIn(string username, string password);

        [OperationContract]
        void LogOut(string token);
        
    }
}
