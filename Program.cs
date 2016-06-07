using System;

namespace Sampler
{
    public class Program
    {
        #if DOTNETCORE
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
        #endif
    }
}
