using CAPonto.Filters;
using CAPonto.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CAPonto.Controllers
{
    [SecurityFilter(Order = 1, Adm = true)]
    public class PeriodoController : Controller
    {
        private IHostingEnvironment _hostingEnvironment;
        private string _nomeArquivo = "periodo.json";
        private string _path;

        public PeriodoController(IHostingEnvironment environment)
        {
            _hostingEnvironment = environment;
            _path = Path.Combine(_hostingEnvironment.ContentRootPath, "Json\\" + _nomeArquivo);
        }

        // GET: Periodo
        public ActionResult Index(int? ano)
        {
            List<Periodo> periodos = new List<Periodo>();

            if (System.IO.File.Exists(_path))
            {
                using (StreamReader sr = new StreamReader(_path))
                {
                    periodos = JsonConvert.DeserializeObject<List<Periodo>>(sr.ReadToEnd());
                }

                ViewBag.Anos = (List<int>)periodos.Select(_ => _.Ano).Distinct().ToList();
            }

            if (ano == null)
                ano = DateTime.Now.Year;

            return View(periodos.Where(_ => _.Ano.Equals(ano)).OrderByDescending(_ => _.Ano).ThenByDescending(_ => _.Mes));
        }

        // GET: Periodo/Details/5
        public ActionResult Details(int ano, int mes)
        {
            List<Periodo> periodos = new List<Periodo>();
            Periodo periodo = null;

            if (System.IO.File.Exists(_path))
            {
                using (StreamReader sr = new StreamReader(_path))
                {
                    periodos = JsonConvert.DeserializeObject<List<Periodo>>(sr.ReadToEnd());
                }

                periodo = periodos.Where(_ => _.Ano.Equals(ano) && _.Mes.Equals(mes)).FirstOrDefault();
            }

            if (periodo == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(periodo);
        }

        // GET: Periodo/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Periodo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            List<Periodo> periodos = new List<Periodo>();
            Periodo periodo = new Periodo();

            periodo.Ano = Convert.ToInt32(collection["Ano"]);
            periodo.Mes = Convert.ToInt32(collection["Mes"]);
            periodo.DataInicio = Convert.ToDateTime(collection["DataInicio"]);
            periodo.DataFim = Convert.ToDateTime(collection["DataFim"]);

            try
            {
                if (System.IO.File.Exists(_path))
                {
                    using (StreamReader sr = new StreamReader(_path))
                    {
                        periodos = JsonConvert.DeserializeObject<List<Periodo>>(sr.ReadToEnd());
                    }
                    periodos.RemoveAll(_ => _.Ano.Equals(periodo.Ano) && _.Mes.Equals(periodo.Mes));
                }

                periodos.Add(periodo);

                string json = JsonConvert.SerializeObject(periodos, Formatting.Indented);

                System.IO.File.WriteAllText(_path, json);
            }
            catch
            {

            }

            return View(periodo);
        }

        // GET: Periodo/Edit/5
        public ActionResult Edit(int ano, int mes)
        {
            List<Periodo> periodos = new List<Periodo>();
            Periodo periodo = null;

            if (System.IO.File.Exists(_path))
            {
                using (StreamReader sr = new StreamReader(_path))
                {
                    periodos = JsonConvert.DeserializeObject<List<Periodo>>(sr.ReadToEnd());
                }

                periodo = periodos.Where(_ => _.Ano.Equals(ano) && _.Mes.Equals(mes)).FirstOrDefault();
            }

            if (periodo == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(periodo);
        }

        // POST: Periodo/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int ano, int mes, IFormCollection collection)
        {
            List<Periodo> periodos = new List<Periodo>();
            Periodo periodo = new Periodo();

            periodo.Ano = Convert.ToInt32(collection["Ano"]);
            periodo.Mes = Convert.ToInt32(collection["Mes"]);
            periodo.DataInicio = Convert.ToDateTime(collection["DataInicio"]);
            periodo.DataFim = Convert.ToDateTime(collection["DataFim"]);

            try
            {
                if (System.IO.File.Exists(_path))
                {
                    using (StreamReader sr = new StreamReader(_path))
                    {
                        periodos = JsonConvert.DeserializeObject<List<Periodo>>(sr.ReadToEnd());
                    }
                    periodos.RemoveAll(_ => _.Ano.Equals(periodo.Ano) && _.Mes.Equals(periodo.Mes));
                }

                periodos.Add(periodo);

                string json = JsonConvert.SerializeObject(periodos, Formatting.Indented);

                System.IO.File.WriteAllText(_path, json);
            }
            catch
            {
                
            }

            return View(periodo);
        }

        // GET: Periodo/Delete/5
        public ActionResult Delete(int ano, int mes)
        {
            List<Periodo> periodos = new List<Periodo>();
            Periodo periodo = null;

            if (System.IO.File.Exists(_path))
            {
                using (StreamReader sr = new StreamReader(_path))
                {
                    periodos = JsonConvert.DeserializeObject<List<Periodo>>(sr.ReadToEnd());
                }

                periodo = periodos.Where(_ => _.Ano.Equals(ano) && _.Mes.Equals(mes)).FirstOrDefault();
            }

            if (periodo == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(periodo);
        }

        // POST: Periodo/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int ano, int mes, IFormCollection collection)
        {
            List<Periodo> periodos = new List<Periodo>();

            if (System.IO.File.Exists(_path))
            {
                using (StreamReader sr = new StreamReader(_path))
                {
                    periodos = JsonConvert.DeserializeObject<List<Periodo>>(sr.ReadToEnd());
                }

                periodos.RemoveAll(_ => _.Ano.Equals(ano) && _.Mes.Equals(mes));

                string json = JsonConvert.SerializeObject(periodos, Formatting.Indented);

                System.IO.File.WriteAllText(_path, json);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}