using Immortal.Communication;
using System;
using System.Threading.Tasks;

namespace Immortal
{
    class Program
    {
        static void Main(string[] args) =>
            new Program()
            .StartServerAsync()
            .GetAwaiter()
            .GetResult();

        private protected async Task StartServerAsync()
        {
           _ = Task.Run(() =>
           {
               new CommunicationHandler();
           });

            await Task.Delay(-1);
        }
    }
}
