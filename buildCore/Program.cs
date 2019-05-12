using System;
using System.Threading;

namespace buildCore
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World Docker");

            //平常为了让窗口不关闭，会如下代码，但在docker中是不行的
            Console.ReadLine();

            //解决方案:
            Thread.Sleep(Timeout.Infinite);

        }
    }
}
