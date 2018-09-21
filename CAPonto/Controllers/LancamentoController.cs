using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CAPonto.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CAPonto.Controllers
{
    public class LancamentoController : Controller
    {
        private IHostingEnvironment _hostingEnvironment;
        private string _pathBase;
        private string _pathBaseUpload;
        private string _nomeArquivoPeriodo = "periodo.json";
        private string _pathPeriodo;

        public LancamentoController(IHostingEnvironment environment)
        {
            _hostingEnvironment = environment;
            _pathBase = Path.Combine(_hostingEnvironment.ContentRootPath, "Json\\");
            _pathBaseUpload = Path.Combine(_hostingEnvironment.ContentRootPath, "Upload\\");
            _pathPeriodo = Path.Combine(_hostingEnvironment.ContentRootPath, "Json\\" + _nomeArquivoPeriodo);
        }

        // GET: Lancamento
        public ActionResult Index(int? ano, int? mes)
        {
            List<LancamentoDia> lancamentosDia = ListarLancamentosDia(ano, mes);

            return View(lancamentosDia.OrderBy(_ => _.Data));
        }

        [HttpPost]
        public ActionResult Index(IFormCollection collection)
        {
            int ano = Int32.Parse(collection["ano"].ToString());
            int mes = Int32.Parse(collection["mes"].ToString());

            List<LancamentoDia> lancamentosDia = ListarLancamentosDia(ano, mes);

            return View(lancamentosDia.OrderBy(_ => _.Data));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(IFormCollection collection, string submitButton)
        {
            switch (submitButton)
            {
                case "Edit":
                    return Edit(collection);
                case "Delete":
                    return Delete(collection);
            }

            return View();
        }

        public ActionResult Edit(IFormCollection collection)
        {
            List<LancamentoDia> lancamentosDia = new List<LancamentoDia>();
            List<Lancamento> lancamentos = new List<Lancamento>();
            try
            {
                int ordem = 0;
                var dataAtual = "";
                var data = "";
                var sulfix = "";
                var hora = "";
                bool ehFeriado = false;

                foreach (var item in collection)
                {
                    if (item.Key.Contains("RECORD_DATA_"))
                    {
                        data = item.Value;
                        sulfix = item.Key.Replace("RECORD_DATA_", "");
                        hora = collection["RECORD_" + sulfix];

                        if (!dataAtual.Equals(data))
                        {
                            if (!dataAtual.Equals(string.Empty))
                            {
                                ehFeriado = !string.IsNullOrEmpty(collection["RECORD_FERIADO_" + dataAtual].ToString());

                                lancamentosDia.Add(new LancamentoDia()
                                {
                                    Matricula = HttpContext.Session.GetString("_MATRICULA"),
                                    Data = Convert.ToDateTime(dataAtual),
                                    DiaSemana = Convert.ToDateTime(dataAtual).DayOfWeek.ToString(),
                                    DiaSemanaReduzido = Convert.ToDateTime(dataAtual).ToString("ddd", new CultureInfo("en-US")),
                                    EhFeriado = ehFeriado,
                                    Lancamentos = lancamentos
                                });
                                lancamentos = new List<Lancamento>();
                            }
                            ordem = 0;
                            dataAtual = data;
                        }

                        lancamentos.Add(new Lancamento()
                        {
                            Data = Convert.ToDateTime(data + " " + hora),
                            Ordem = ordem
                        });

                        ordem++;
                    }
                }

                if (!data.Equals(string.Empty))
                {
                    ehFeriado = !string.IsNullOrEmpty(collection["RECORD_FERIADO_" + data].ToString());

                    lancamentosDia.Add(new LancamentoDia()
                    {
                        Matricula = HttpContext.Session.GetString("_MATRICULA"),
                        Data = Convert.ToDateTime(data),
                        DiaSemana = Convert.ToDateTime(data).DayOfWeek.ToString(),
                        DiaSemanaReduzido = Convert.ToDateTime(data).ToString("ddd", new CultureInfo("en-US")),
                        EhFeriado = ehFeriado,
                        Lancamentos = lancamentos
                    });

                    GravarLancamentosDia(lancamentosDia, collection["RECORD_ANO"].ToString(), collection["RECORD_MES"].ToString());
                }

            }
            catch
            {
            }

            return RedirectToAction(nameof(Index), new { ano = Convert.ToInt32(collection["RECORD_ANO"].ToString()), mes = Convert.ToInt32(collection["RECORD_MES"].ToString()) });
        }

        [HttpPost]
        public async Task<IActionResult> Import(IFormFile arquivo)
        {
            if (arquivo != null && arquivo.Length > 0)
            {
                var _path = _pathBaseUpload + arquivo.FileName;

                using (var stream = new FileStream(_path, FileMode.Create))
                {
                    await arquivo.CopyToAsync(stream);
                }

                using (StreamReader sr = new StreamReader(_path, Encoding.GetEncoding("iso-8859-1"), true))
                {
                    string line = "";
                    bool ehProprietario = false;

                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Contains("Matrícula:;"))
                        {
                            var matricula = line.Split("Matrícula:;")[1].Replace(";", "");

                            if (matricula.Equals(HttpContext.Session.GetString("_MATRICULA")))
                                ehProprietario = true;
                        }

                        if (ehProprietario)
                        {
                            DateTime data;
                            string[] dados = line.Split(";");

                            if (DateTime.TryParse(dados[0].Replace(";", ""), out data))
                            {
                                //decobrir periodo pela data
                                List<Periodo> periodos = new List<Periodo>();
                                Periodo periodo = null;

                                if (System.IO.File.Exists(_pathPeriodo))
                                {
                                    using (StreamReader srp = new StreamReader(_pathPeriodo))
                                    {
                                        periodos = JsonConvert.DeserializeObject<List<Periodo>>(srp.ReadToEnd());
                                    }
                                    periodo = periodos.Where(_ => _.DataInicio <= data && _.DataFim >= data).FirstOrDefault();
                                }

                                //pegar arquivo do colaborador no período
                                List<LancamentoDia> lancamentosDia = new List<LancamentoDia>();

                                if (periodo != null)
                                {
                                    _path = _pathBase + "lancamentoDia-" +
                                                HttpContext.Session.GetString("_MATRICULA") + "-" +
                                                periodo.Ano.ToString() + "-" +
                                                periodo.Mes.ToString() + ".json";

                                    if (System.IO.File.Exists(_path))
                                    {
                                        using (StreamReader src = new StreamReader(_path))
                                        {
                                            lancamentosDia = JsonConvert.DeserializeObject<List<LancamentoDia>>(src.ReadToEnd());
                                        }
                                    }

                                    List<DateTime> listaFeriados = lancamentosDia.Where(_ => _.EhFeriado).Select(_ => _.Data).ToList();

                                    //apagar regristro do dia
                                    if (!lancamentosDia.Count.Equals(0))
                                        lancamentosDia.RemoveAll(_ => _.Data.Equals(data));

                                    int ordem = 0;
                                    List<Lancamento> lancamentos = new List<Lancamento>();

                                    for (int i = 2; i < dados.Length - 2; i = i + 2)
                                    {
                                        //pegar lançamentos 
                                        if (!dados[i].Equals("-"))
                                        {
                                            lancamentos.Add(new Lancamento()
                                            {
                                                Data = Convert.ToDateTime(data.ToString("dd/MM/yyyy") + " " + dados[i]),
                                                Ordem = ordem
                                            });
                                            ordem++;
                                        }

                                        if (!dados[i + 1].Equals("-"))
                                        {
                                            lancamentos.Add(new Lancamento()
                                            {
                                                Data = Convert.ToDateTime(data.ToString("dd/MM/yyyy") + " " + dados[i + 1]),
                                                Ordem = ordem
                                            });
                                            ordem++;
                                        }
                                    }

                                    //inserir dados que faltaram em branco
                                    for (int o = ordem; o < 6; o++)
                                    {
                                        lancamentos.Add(new Lancamento()
                                        {
                                            Data = Convert.ToDateTime(data),
                                            Ordem = o
                                        });
                                    }

                                    //cadastrar lançamentos do dia
                                    lancamentosDia.Add(new LancamentoDia()
                                    {
                                        Matricula = HttpContext.Session.GetString("_MATRICULA"),
                                        Data = Convert.ToDateTime(data),
                                        DiaSemana = Convert.ToDateTime(data).DayOfWeek.ToString(),
                                        DiaSemanaReduzido = Convert.ToDateTime(data).ToString("ddd", new CultureInfo("en-US")),
                                        EhFeriado = listaFeriados.Exists(_ => _.Equals(Convert.ToDateTime(data))),
                                        Lancamentos = lancamentos
                                    });

                                    GravarLancamentosDia(lancamentosDia, periodo.Ano.ToString(), periodo.Mes.ToString());

                                }
                            }
                        }
                    }
                }
            }

            return RedirectToAction(nameof(Index), new { ano = HttpContext.Session.GetString(this.ToString() + "_ANO"), mes = HttpContext.Session.GetString(this.ToString() + "_MES") });
        }

        public ActionResult Delete(IFormCollection collection)
        {
            try
            {
                var _path = _pathBase + "lancamentoDia-" +
                            HttpContext.Session.GetString("_MATRICULA") + "-" +
                            collection["RECORD_ANO"].ToString() + "-" +
                            collection["RECORD_MES"].ToString() + ".json";

                System.IO.File.Delete(_path);
            }
            catch
            {
            }

            return RedirectToAction(nameof(Index), new { ano = Convert.ToInt32(collection["RECORD_ANO"].ToString()), mes = Convert.ToInt32(collection["RECORD_MES"].ToString()) });
        }

        private List<LancamentoDia> ListarLancamentosDia(int? ano, int? mes)
        {
            List<LancamentoDia> lancamentosDia = new List<LancamentoDia>();
            List<Lancamento> lancamentos = new List<Lancamento>();
            List<Periodo> periodos = new List<Periodo>();
            Periodo periodo = null;

            if (System.IO.File.Exists(_pathPeriodo))
            {
                using (StreamReader sr = new StreamReader(_pathPeriodo))
                {
                    periodos = JsonConvert.DeserializeObject<List<Periodo>>(sr.ReadToEnd());
                }
            }

            if (ano != null && mes != null)
                periodo = periodos.Where(_ => _.Ano.Equals(ano) && _.Mes.Equals(mes)).FirstOrDefault();
            else
                periodo = periodos.Where(_ => _.DataInicio <= DateTime.Now && _.DataFim >= DateTime.Now).FirstOrDefault();

            if (periodo != null)
            {
                HttpContext.Session.SetString(this.ToString() + "_ANO", periodo.Ano.ToString());
                HttpContext.Session.SetString(this.ToString() + "_MES", periodo.Mes.ToString());
                ViewBag.Ano = periodo.Ano;
                ViewBag.Mes = periodo.Mes;
                ViewBag.Pares = 3;

                var _path = _pathBase + "lancamentoDia-" +
                            HttpContext.Session.GetString("_MATRICULA") + "-" +
                            periodo.Ano.ToString() + "-" +
                            periodo.Mes.ToString() + ".json";

                if (System.IO.File.Exists(_path))
                {
                    using (StreamReader sr = new StreamReader(_path))
                    {
                        lancamentosDia = JsonConvert.DeserializeObject<List<LancamentoDia>>(sr.ReadToEnd());
                    }
                }

                if (lancamentosDia.Count.Equals(0))
                {
                    for (DateTime dataInicio = periodo.DataInicio;
                        dataInicio <= periodo.DataFim;
                        dataInicio = dataInicio.AddDays(1))
                    {
                        lancamentos = new List<Lancamento>();
                        for (int i = 0; i < 6; i++)
                        {
                            lancamentos.Add(new Lancamento()
                            {
                                Data = dataInicio,
                                Ordem = i
                            });
                        }
                        lancamentosDia.Add(new LancamentoDia()
                        {
                            Data = dataInicio,
                            DiaSemana = dataInicio.DayOfWeek.ToString(),
                            DiaSemanaReduzido = dataInicio.ToString("ddd", new CultureInfo("en-US")),
                            EhFeriado = false,
                            Matricula = HttpContext.Session.GetString("_MATRICULA"),
                            Lancamentos = lancamentos
                        });
                    }
                }
            }

            return lancamentosDia;
        }

        private void GravarLancamentosDia(List<LancamentoDia> lancamentosDia, string ano, string mes)
        {
            foreach (LancamentoDia dia in lancamentosDia)
            {
                double total = 0;
                double diferenca = 0;
                double minutosDia = Convert.ToDouble(HttpContext.Session.GetString("_MINUTOS_DIA"));
                double limiteDia = Convert.ToDouble(HttpContext.Session.GetString("_LIMITE_DIA"));

                dia.DiaSemana = dia.Data.DayOfWeek.ToString();
                dia.DiaSemanaReduzido = dia.Data.ToString("ddd", new CultureInfo("en-US"));

                if (dia.EhFeriado || dia.Data.DayOfWeek.Equals(DayOfWeek.Saturday) || dia.Data.DayOfWeek.Equals(DayOfWeek.Sunday))
                {
                    minutosDia = 0;
                    limiteDia = 0;
                }

                for (int d = 0; d < dia.Lancamentos.Count; d = d + 2)
                {
                    var totalDupla = (dia.Lancamentos[d + 1].Data.Subtract(dia.Lancamentos[d].Data)).TotalMinutes;
                    total += totalDupla;
                    if (!totalDupla.Equals(0))
                        diferenca += (totalDupla - minutosDia);
                }

                dia.Total = total;

                if (diferenca >= 0)
                {
                    dia.Extra = diferenca;
                    if (diferenca <= limiteDia)
                        dia.ExtraAjustado = 0;
                    else
                        dia.ExtraAjustado = diferenca;
                }
                else
                {
                    dia.Devendo = diferenca * (-1);
                    if (diferenca * (-1) <= limiteDia)
                        dia.DevendoAjustado = 0;
                    else
                        dia.DevendoAjustado = diferenca * (-1);
                }
            }

            var _pathDia = _pathBase + "lancamentoDia-" +
                        HttpContext.Session.GetString("_MATRICULA") + "-" +
                        ano + "-" +
                        mes + ".json";

            string jsonDia = JsonConvert.SerializeObject(lancamentosDia, Formatting.Indented);

            System.IO.File.WriteAllText(_pathDia, jsonDia);
        }

    }
}