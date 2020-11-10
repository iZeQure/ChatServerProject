using Immortal.Communication;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Immortal
{
    class Program
    {
        public static IConfiguration Configuration { get; set; }

        static void Main(string[] args)
        {
            Console.WriteLine(Directory.GetCurrentDirectory());
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            Configuration = builder.Build();
            Console.WriteLine(Configuration["ServerConfig:server_ip"]);
        }


        private protected async Task StartServerAsync()
        {
           _ = Task.Run(() =>
           {
               new CommunicationHandler(Configuration);
           });

            await Task.Delay(-1);
        }
    }
}
