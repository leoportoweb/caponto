using CAPonto.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Net.Http;
using Microsoft.AspNetCore.Routing;

namespace CAPonto.Filters
{
    public class SessionFilter : Attribute, IActionFilter, IOrderedFilter
    {
        public int Order { get; set; }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            bool lVerificaSessao = true;

            if (context.ActionDescriptor.DisplayName.Contains("HomeController.Index"))
                lVerificaSessao = false;

            if (lVerificaSessao)
            {
                if (!Utilitaria.VerificaCookie(context.HttpContext.Request, context.HttpContext.Session))
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "Index", controller = "Home" }));
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            //throw new NotImplementedException();
        }
    }
}
