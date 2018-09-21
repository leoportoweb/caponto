using System;
using System.Web;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CAPonto.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

namespace CAPonto.Controllers
{
    public class ConfigController : Controller
    {
        private IHostingEnvironment _hostingEnvironment;
        private string _nomeArquivo = "config.json";
        private string _path;

        public ConfigController(IHostingEnvironment environment)
        {
            _hostingEnvironment = environment;
            _path = Path.Combine(_hostingEnvironment.ContentRootPath, "Json\\" + _nomeArquivo);
        }

        // GET: Config
        public ActionResult Index()
        {
            Config config = new Config();

            if (System.IO.File.Exists(_path))
            {
                using (StreamReader sr = new StreamReader(_path))
                {
                    config = JsonConvert.DeserializeObject<Config>(sr.ReadToEnd());
                }
            }

            return View(config);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(IFormCollection collection)
        {
            Config config = new Config();

            config.HorasDia = Convert.ToInt32(collection["HorasDia"]);
            config.LimiteAjuste = Convert.ToInt32(collection["LimiteAjuste"]);

            string json = JsonConvert.SerializeObject(config, Formatting.Indented);

            System.IO.File.WriteAllText(_path, json);

            return View(config);
        }

    }
}