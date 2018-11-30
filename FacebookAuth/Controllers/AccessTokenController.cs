using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FacebookAuth.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace FacebookAuth.Controllers
{
    public class AccessTokenController : Controller
    {
        public class AccessTokenViewModel
        {
            public string Message { get; set; }
            public string Code { get; set; }
            public string AccessToken { get; set; }
            //   public Account UserAccount { get; set; }
        }
        [Route("[controller]/[action]")]
        public IActionResult Index()
        {
            var accessTokenViewModel = new AccessTokenViewModel();

            string code = HttpContext.Request.Query["code"].ToString();

            string scope = Startup.FacebookSettings.Scope;
            var redirectUri = HttpContext?.Request?.GetDisplayUrl();

            var tokenServcice = new AccessTokenService();
            if (!string.IsNullOrEmpty(code))
            {
                accessTokenViewModel.Code = code;

                var accessToken = tokenServcice.GetAccessToken(scope, redirectUri, code);

                //https://stackoverflow.com/questions/49768774/how-to-get-access-token-from-httpcontext-in-net-core-2-0
                //https://csharp.hotexamples.com/examples/Facebook/FacebookClient/GetTaskAsync/php-facebookclient-gettaskasync-method-examples.html

                //var accessToken2 = string.Empty;

                //var accessToken3 = HttpContext.GetTokenAsync("access_token").Result;
                //var accessTokenObj = HttpContext.Items["access_token1"];
                //if (accessTokenObj != null)
                //    accessToken2 = accessTokenObj.ToString();

                if (!string.IsNullOrEmpty(accessToken))
                {
                    accessTokenViewModel.AccessToken = accessToken;
                    HttpContext.Session.SetString("AccessToken", accessToken);
                }
            }
            else
            {
                var url = tokenServcice.GetAuthorizationCodeUrl(scope, redirectUri);
                Response.Redirect(url);
            }

            return View(accessTokenViewModel);
        }
    }
}