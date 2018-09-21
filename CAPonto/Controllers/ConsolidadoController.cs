using System;
using System.Collections.Generic;
using System.Globalization;
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
    public class ConsolidadoController : Controller
    {
        private IHostingEnvironment _hostingEnvironment;
        private string _nomeArquivoPeriodo = "periodo.json";
        private string _pathPeriodo;
        private string _pathBase;

        public ConsolidadoController(IHostingEnvironment environment)
        {
            _hostingEnvironment = environment;
            _pathPeriodo = Path.Combine(_hostingEnvironment.ContentRootPath, "Json\\" + _nomeArquivoPeriodo);
            _pathBase = Path.Combine(_hostingEnvironment.ContentRootPath, "Json\\");
        }

        // GET: Consolidado
        public ActionResult Index(int? ano)
        {
            List<Consolidado> consolidacao = new List<Consolidado>();
            List<Periodo> periodos = new List<Periodo>();
            List<LancamentoDia> lancamentosDia = new List<LancamentoDia>();

            if (System.IO.File.Exists(_pathPeriodo))
            {
                using (StreamReader sr = new StreamReader(_pathPeriodo))
                {
                    periodos = JsonConvert.DeserializeObject<List<Periodo>>(sr.ReadToEnd());
                }

                ViewBag.Anos = (List<int>)periodos.Select(_ => _.Ano).Distinct().ToList();
            }

            if (ano == null)
                ano = DateTime.Now.Year;

            for (int mes = 1; mes <= 12; mes++)
            {
                lancamentosDia = new List<LancamentoDia>();

                var _path = _pathBase + "lancamentoDia-" +
                            HttpContext.Session.GetString("GLOBAL_MATRICULA") + "-" +
                            ano.ToString() + "-" +
                            mes.ToString() + ".json";

                if (System.IO.File.Exists(_path))
                {
                    using (StreamReader sr = new StreamReader(_path))
                    {
                        lancamentosDia = JsonConvert.DeserializeObject<List<LancamentoDia>>(sr.ReadToEnd());
                    }
                }

                if (lancamentosDia.Any())
                {
                    double totalExtra = lancamentosDia.Sum(_ => _.Extra);
                    double totalDevendo = lancamentosDia.Sum(_ => _.Devendo);
                    double totalExtraAjustado = lancamentosDia.Sum(_ => _.ExtraAjustado);
                    double totalDevendoAjustado = lancamentosDia.Sum(_ => _.DevendoAjustado);

                    if (totalExtra >= totalDevendo)
                    {
                        totalExtra = totalExtra - totalDevendo;
                        totalDevendo = 0;
                    }
                    else
                    {
                        totalDevendo = totalDevendo - totalExtra;
                        totalExtra = 0;
                    }

                    if (totalExtraAjustado >= totalDevendoAjustado)
                    {
                        totalExtraAjustado = totalExtraAjustado - totalDevendoAjustado;
                        totalDevendoAjustado = 0;
                    }
                    else
                    {
                        totalDevendoAjustado = totalDevendoAjustado - totalExtraAjustado;
                        totalExtraAjustado = 0;
                    }

                    consolidacao.Add(new Consolidado()
                    {
                        Matricula = HttpContext.Session.GetString("GLOBAL_MATRICULA"),
                        Ano = ano ?? DateTime.Now.Year,
                        Mes = mes,
                        MesDescricao = new CultureInfo("en-US").DateTimeFormat.GetMonthName(mes),
                        Extra = totalExtra,
                        Devendo = totalDevendo,
                        ExtraAjustado = totalExtraAjustado,
                        DevendoAjustado = totalDevendoAjustado,
                        ExtraDecimal = totalExtra / 60,
                        DevendoDecimal = totalDevendo / 60,
                        ExtraAjustadoDecimal = totalExtraAjustado / 60,
                        DevendoAjustadoDecimal = totalDevendoAjustado / 60
                    });
                }
                else
                {
                    consolidacao.Add(new Consolidado()
                    {
                        Matricula = HttpContext.Session.GetString("GLOBAL_MATRICULA"),
                        Ano = ano ?? DateTime.Now.Year,
                        Mes = mes,
                        MesDescricao = new CultureInfo("en-US").DateTimeFormat.GetMonthName(mes)
                    });
                }

                var totalExtraFinal = consolidacao.Sum(_ => _.Extra);
                var totalDevendoFinal = consolidacao.Sum(_ => _.Devendo);
                var totalExtraAjustadoFinal = consolidacao.Sum(_ => _.ExtraAjustado);
                var totalDevendoAjustadoFinal = consolidacao.Sum(_ => _.DevendoAjustado);

                if (totalExtraFinal >= totalDevendoFinal)
                {
                    totalExtraFinal = totalExtraFinal - totalDevendoFinal;
                    totalDevendoFinal = 0;
                }
                else
                {
                    totalDevendoFinal = totalDevendoFinal - totalExtraFinal;
                    totalExtraFinal = 0;
                }

                if (totalExtraAjustadoFinal >= totalDevendoAjustadoFinal)
                {
                    totalExtraAjustadoFinal = totalExtraAjustadoFinal - totalDevendoAjustadoFinal;
                    totalDevendoAjustadoFinal = 0;
                }
                else
                {
                    totalDevendoAjustadoFinal = totalDevendoAjustadoFinal - totalExtraAjustadoFinal;
                    totalExtraAjustadoFinal = 0;
                }

                ViewBag.DiffExtra = totalExtraFinal;
                ViewBag.DiffDevendo = totalDevendoFinal;
                ViewBag.DiffExtraAjustado = totalExtraAjustadoFinal;
                ViewBag.DiffDevendoAjustado = totalDevendoAjustadoFinal;
                ViewBag.DiffExtraDecimal = totalExtraFinal / 60;
                ViewBag.DiffDevendoDecimal = totalDevendoFinal / 60;
                ViewBag.DiffExtraAjustadoDecimal = totalExtraAjustadoFinal / 60;
                ViewBag.DiffDevendoAjustadoDecimal = totalDevendoAjustadoFinal / 60;
            }

            return View(consolidacao);
        }

    }
}