using System;
using System.Collections.Generic;
using System.ServiceModel;
using Common;
using Common.Model;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ChannelFactory<IBookstoreService> cf = new ChannelFactory<IBookstoreService>(new NetTcpBinding(), "net.tcp://localhost:9000");
            IBookstoreService proxy = cf.CreateChannel();

            proxy.CreateAuthor("a", "a", "a");
            proxy.CreateBook("a", 0000, 0);

            Dictionary<int, Book> books = proxy.GetBooks();
            foreach(var book in books.Values)
            {
                Console.WriteLine(book.BookId);
            }

        }
    }
}
