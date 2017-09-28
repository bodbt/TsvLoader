using System;
using System.Threading;

namespace TsvLoader
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var loader = new TsvLoader();

                loader.Load();

                Environment.Exit(0);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Thread.Sleep(3000);
                Environment.Exit(1);
            }
        }
    }
}
