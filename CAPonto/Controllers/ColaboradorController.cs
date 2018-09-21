using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CAPonto.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CAPonto.Controllers
{
    public class ColaboradorController : Controller
    {
        private IHostingEnvironment _hostingEnvironment;
        private string _nomeArquivo = "colaborador.json";
        private string _path;

        public ColaboradorController(IHostingEnvironment environment)
        {
            _hostingEnvironment = environment;
            _path = Path.Combine(_hostingEnvironment.ContentRootPath, "Json\\" + _nomeArquivo);
        }

        // GET: Colaborador
        public ActionResult Index()
        {
            List<Colaborador> colaboradores = new List<Colaborador>();

            if (System.IO.File.Exists(_path))
            {
                using (StreamReader sr = new StreamReader(_path))
                {
                    colaboradores = JsonConvert.DeserializeObject<List<Colaborador>>(sr.ReadToEnd());
                }
            }

            return View(colaboradores.OrderBy(_ => _.Nome));
        }

        // GET: Colaborador/Details/5
        public ActionResult Details(string matricula)
        {
            List<Colaborador> colaboradores = new List<Colaborador>();
            Colaborador colaborador = null;

            if (System.IO.File.Exists(_path))
            {
                using (StreamReader sr = new StreamReader(_path))
                {
                    colaboradores = JsonConvert.DeserializeObject<List<Colaborador>>(sr.ReadToEnd());
                }

                colaborador = colaboradores.Where(_ => _.Matricula.Equals(matricula)).FirstOrDefault();
            }

            if (colaborador == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(colaborador);
        }

        // GET: Colaborador/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Colaborador/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            List<Colaborador> colaboradores = new List<Colaborador>();
            Colaborador colaborador = new Colaborador();

            colaborador.Matricula = collection["Matricula"];
            colaborador.Nome = collection["Nome"];
            colaborador.Administrador = Convert.ToBoolean(collection["Administrador"][0]);

            try
            {
                if (System.IO.File.Exists(_path))
                {
                    using (StreamReader sr = new StreamReader(_path))
                    {
                        colaboradores = JsonConvert.DeserializeObject<List<Colaborador>>(sr.ReadToEnd());
                    }
                    colaboradores.RemoveAll(_ => _.Matricula.Equals(colaborador.Matricula));
                }

                colaboradores.Add(colaborador);

                string json = JsonConvert.SerializeObject(colaboradores, Formatting.Indented);

                System.IO.File.WriteAllText(_path, json);
            }
            catch
            {

            }

            return View(colaborador);
        }

        // GET: Colaborador/Edit/5
        public ActionResult Edit(string matricula)
        {
            List<Colaborador> colaboradores = new List<Colaborador>();
            Colaborador colaborador = null;

            if (System.IO.File.Exists(_path))
            {
                using (StreamReader sr = new StreamReader(_path))
                {
                    colaboradores = JsonConvert.DeserializeObject<List<Colaborador>>(sr.ReadToEnd());
                }

                colaborador = colaboradores.Where(_ => _.Matricula.Equals(matricula)).FirstOrDefault();
            }

            if (colaborador == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(colaborador);
        }

        // POST: Colaborador/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            List<Colaborador> colaboradores = new List<Colaborador>();
            Colaborador colaborador = new Colaborador();

            colaborador.Matricula = collection["Matricula"];
            colaborador.Nome = collection["Nome"];
            colaborador.Administrador = Convert.ToBoolean(collection["Administrador"][0]);

            try
            {
                if (System.IO.File.Exists(_path))
                {
                    using (StreamReader sr = new StreamReader(_path))
                    {
                        colaboradores = JsonConvert.DeserializeObject<List<Colaborador>>(sr.ReadToEnd());
                    }
                    colaboradores.RemoveAll(_ => _.Matricula.Equals(colaborador.Matricula));
                }

                colaboradores.Add(colaborador);

                string json = JsonConvert.SerializeObject(colaboradores, Formatting.Indented);

                System.IO.File.WriteAllText(_path, json);
            }
            catch
            {

            }

            return View(colaborador);
        }

        // GET: Colaborador/Delete/5
        public ActionResult Delete(string matricula)
        {
            List<Colaborador> colaboradores = new List<Colaborador>();
            Colaborador colaborador = null;

            if (System.IO.File.Exists(_path))
            {
                using (StreamReader sr = new StreamReader(_path))
                {
                    colaboradores = JsonConvert.DeserializeObject<List<Colaborador>>(sr.ReadToEnd());
                }

                colaborador = colaboradores.Where(_ => _.Matricula.Equals(matricula)).FirstOrDefault();
            }

            if (colaborador == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(colaborador);
        }

        // POST: Colaborador/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string matricula, IFormCollection collection)
        {
            List<Colaborador> colaboradores = new List<Colaborador>();

            if (System.IO.File.Exists(_path))
            {
                using (StreamReader sr = new StreamReader(_path))
                {
                    colaboradores = JsonConvert.DeserializeObject<List<Colaborador>>(sr.ReadToEnd());
                }

                colaboradores.RemoveAll(_ => _.Matricula.Equals(matricula));

                string json = JsonConvert.SerializeObject(colaboradores, Formatting.Indented);

                System.IO.File.WriteAllText(_path, json);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}