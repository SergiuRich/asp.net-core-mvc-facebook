using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FacebookAuth.Services;
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

            string scope = "email,manage_pages,publish_pages,publish_to_groups";//user_posts,publish_pages,user_videos
            var redirectUri = HttpContext?.Request?.GetDisplayUrl();

            var tokenServcice = new AccessTokenService();
            if (!string.IsNullOrEmpty(code))
            {
                accessTokenViewModel.Code = code;

                var accessToken = tokenServcice.GetAccessToken(scope, redirectUri, code);

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