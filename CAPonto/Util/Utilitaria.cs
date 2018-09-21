using Microsoft.AspNetCore.Http;
using System;
using System.Net;

namespace CAPonto.Util
{
    public class Utilitaria
    {
        #region Cookie

        public static void CarregaCookie(Microsoft.AspNetCore.Http.HttpResponse response, ISession session)
        {
            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(1);

            foreach (string key in session.Keys)
            {
                try
                {
                    if (key.ToUpper().StartsWith("GLOBAL_"))
                        response.Cookies.Append(key, session.GetString(key), option);
                }
                catch (Exception ex)
                {
                    string strMessage = ex.Message;
                }
            }
        }

        public static bool VerificaCookie(Microsoft.AspNetCore.Http.HttpRequest request, ISession session)
        {
            bool lRetorno = false;

            foreach (var cookie in request.Cookies)
            {
                try
                {
                    if (cookie.Key.ToUpper().StartsWith("GLOBAL_"))
                    {
                        lRetorno = true;
                        session.SetString(cookie.Key, cookie.Value);
                    }
                }
                catch (Exception ex)
                {
                    string strMessage = ex.Message;
                }
            }

            return lRetorno;
        }

        public static void ApagaCookie(Microsoft.AspNetCore.Http.HttpResponse response, Microsoft.AspNetCore.Http.HttpRequest request)
        {
            foreach (var cookie in request.Cookies)
            {
                try
                {
                    if (cookie.Key.ToUpper().StartsWith("GLOBAL_"))
                        response.Cookies.Delete(cookie.Key);
                }
                catch (Exception ex)
                {
                    string strMessage = ex.Message;
                }
            }
        }

        #endregion
    }
}
