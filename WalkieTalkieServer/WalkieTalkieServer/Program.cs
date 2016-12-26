using Server.Data;
using System;
using System.Threading;

namespace WalkieTalkieServer
{
    class Program
    {
        static void Main(string[] args)
        {
            //new Server();
            Datum a = new Datum("users");
            dynamic b = a.Select("id > 5");
            Console.WriteLine(b.name);
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
