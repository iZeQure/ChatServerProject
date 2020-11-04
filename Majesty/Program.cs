using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace Majesty
{
    class Program
    {
        static void Main(string[] args) => 
            new Program()
            .StartServerAsync()
            .GetAwaiter()
            .GetResult();

        private async Task StartServerAsync()
        {
            Thread simpleThread = new Thread(SimpleListener);
            
            Console.WriteLine("Hello World!");

            await Task.Delay(-1);
        }

        void SimpleListener()
        {

        }
    }
}
