using CAPonto.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System.IO;

namespace CAPonto
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (!File.Exists("Json/config.json"))
            {
                Config config = new Config()
                {
                    HorasDia = 9,
                    LimiteAjuste = 10
                };

                string json = JsonConvert.SerializeObject(config, Formatting.Indented);

                File.WriteAllText("Json/config.json", json);
            }

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                //.UseContentRoot(Directory.GetCurrentDirectory())
                //.UseIISIntegration()
                .UseStartup<Startup>();
    }
}
