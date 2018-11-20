using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FacebookAuth.Models;
using FacebookAuth.Services;
using Microsoft.AspNetCore.Http;

namespace FacebookAuth.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> About()
        {
            ViewData["Message"] = "User Profile Details";
            var accessToken = HttpContext.Session.GetString("AccessToken");
            var userAccount = new UserAccountViewModel();
            if (!string.IsNullOrEmpty(accessToken))
            {
                var facebookClient = new FacebookClient();
                var facebookService = new FacebookService(facebookClient);

                userAccount = await facebookService.GetAccountAsync(accessToken);

                //await facebookService.PostOnWallAsync(accessToken, "My first message");


                Facebook facebook = new Facebook(accessToken, "2184830978450551");

                //// 1) if you want to publish a post with attached photo (actually, the other way around)
                //string result = facebook.PublishToFacebook("some text", "http://your.website/images/some.png");
                //Console.WriteLine(result);

                //// 2) if you want just to publish a simple text post
                var rezText = Task.Run(async () =>
                {
                    using (var http = new HttpClient())
                    {
                        return await facebook.PublishSimplePost("First Post here");
                    }
                });
                //// var rezTextJson = JObject.Parse(rezText.Result.Item2);
                //// if (rezText.Result.Item1 != 200)
                //// {
                ////     try // return error from JSON
                ////     {
                ////         Console.WriteLine($"Error posting to Facebook. {rezTextJson["error"]["message"].Value<string>()}");
                ////         return;
                ////     }
                ////     catch (Exception ex) // return unknown error
                ////     {
                ////         // log exception somewhere
                ////         Console.WriteLine($"Unknown error posting to Facebook. {ex.Message}");
                ////         return;
                ////     }
                //// }

            }

            return View(userAccount);
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
