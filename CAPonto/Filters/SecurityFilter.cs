using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;

namespace CAPonto.Filters
{
    public class SecurityFilter : Attribute, IActionFilter, IOrderedFilter
    {
        public int Order { get; set; }

        public bool Adm { get; set; }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (Adm && !context.HttpContext.Session.GetString("GLOBAL_ADMINISTRADOR").ToLower().Equals("true"))
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "Index", controller = "Home" }));
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            
        }
    }
}
