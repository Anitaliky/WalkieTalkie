using Data;
using System;
using System.Threading;

namespace WalkieTalkieServer
{
    class Program
    {
        static void Main(string[] args)
        {
            new Server();
            
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
