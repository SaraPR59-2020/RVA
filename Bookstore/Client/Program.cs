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

            Console.ReadKey();
            proxy.LogIn("moco26", "moco26");
        }
    }
}
