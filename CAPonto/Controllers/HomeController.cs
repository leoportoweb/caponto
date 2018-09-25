using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CAPonto.Models;
using CAPonto.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace CAPonto.Controllers
{
    public class HomeController : Controller
    {
        private IHostingEnvironment _hostingEnvironment;
        private string _nomeArquivoColaborador = "colaborador.json";
        private string _pathColaborador;
        private string _nomeArquivoConfig = "config.json";
        private string _pathConfig;

        public HomeController(IHostingEnvironment environment)
        {
            _hostingEnvironment = environment;
            _pathColaborador = Path.Combine(_hostingEnvironment.ContentRootPath, "Json\\" + _nomeArquivoColaborador);
            _pathConfig = Path.Combine(_hostingEnvironment.ContentRootPath, "Json\\" + _nomeArquivoConfig);
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(IFormCollection collection)
        {
            List<Colaborador> colaboradores = new List<Colaborador>();
            Colaborador colaborador = null;

            if (System.IO.File.Exists(_pathColaborador))
            {
                using (StreamReader sr = new StreamReader(_pathColaborador))
                {
                    colaboradores = JsonConvert.DeserializeObject<List<Colaborador>>(sr.ReadToEnd());
                }

                colaborador = colaboradores.Where(_ => _.Matricula.Equals(collection["matricula"])).FirstOrDefault();
            }

            if (colaborador != null)
            {
                HttpContext.Session.SetString("GLOBAL_MATRICULA", colaborador.Matricula);
                HttpContext.Session.SetString("GLOBAL_NOME", colaborador.Nome);
                HttpContext.Session.SetString("GLOBAL_ADMINISTRADOR", colaborador.Administrador.ToString());
                HttpContext.Session.SetString("GLOBAL_MINUTOS_DIA", "540");
                HttpContext.Session.SetString("GLOBAL_LIMITE_DIA", "10");

                Config config = new Config();

                if (System.IO.File.Exists(_pathConfig))
                {
                    using (StreamReader sr = new StreamReader(_pathConfig))
                    {
                        config = JsonConvert.DeserializeObject<Config>(sr.ReadToEnd());
                    }

                    if (config != null)
                    {
                        HttpContext.Session.SetString("GLOBAL_MINUTOS_DIA", (config.HorasDia * 60).ToString());
                        HttpContext.Session.SetString("GLOBAL_LIMITE_DIA", config.LimiteAjuste.ToString());
                    }
                }

                Utilitaria.CarregaCookie(Response, HttpContext.Session);
                
                return RedirectToAction(nameof(Index), "Consolidado");
            }
            else
            {
                ViewBag.MensagemAlerta = "Acesso inválido!";
            }

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.SetString("GLOBAL_MATRICULA", string.Empty);
            HttpContext.Session.SetString("GLOBAL_NOME", string.Empty);
            HttpContext.Session.SetString("GLOBAL_ADMINISTRADOR", string.Empty);

            Utilitaria.ApagaCookie(Response, Request);

            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
