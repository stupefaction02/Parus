using System;
using System.Net.Http;

namespace DummyPublisher
{
    class Program
    {
        static void Main(string[] args)
        {
            new Publisher().Start();

            Console.ReadKey();
        }
    }
}
