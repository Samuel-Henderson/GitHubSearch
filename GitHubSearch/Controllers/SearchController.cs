using GitHubSearch.Models.SearchViewModels;
using GitHubSearch.Services.GitHub;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace GitHubSearch.Controllers
{
    public class SearchController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> UserDetails(string Login)
        {
            if (string.IsNullOrEmpty(Login))
            {
                ViewData["Message"] = "Please supply a username";
                return View("Index");
            }

            GitHubApiService service = new GitHubApiService();
            GitHubUserSearchViewModel model = null;

            try
            {
                var jsonResult = await service.GetUserDetails(Login);
                //This will partiall fill in the Model. We still need to get the repos
                model = new GitHubUserSearchViewModel(jsonResult);

                jsonResult = await service.GetUserRepos(model.ReposUrl);

                var jsonList = JsonConvert.DeserializeObject<List<GitHubUserRepoViewModel>>(jsonResult);

                //We are now ordering the list by Stargazers
                List<GitHubUserRepoViewModel> userRepos = jsonList.OrderByDescending(m => m.Stargazers_Count).ToList();

                //Now we are only taking the top 5
                userRepos = userRepos.Take(5).ToList();

                model.Repos = userRepos;

            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError && e.Response != null)
                {
                    HttpWebResponse exceptionResponse = (HttpWebResponse)e.Response;

                    //If we have a 404 error we want to tell the user that we have not found a user for that username.
                    if (exceptionResponse.StatusCode == HttpStatusCode.NotFound)
                    {
                        ViewData["Error Message"] = "A user with this name was not found.";
                        return View("Index");
                    }
                    else if (exceptionResponse.StatusCode == HttpStatusCode.Forbidden)
                    {
                        //This can likely be due to the limit that the GitHub Api has on searches. It is 30 searches per minute.
                        ViewData["Error Message"] = "It is possible that you have reached the limit for browsing users.";
                        ViewData["Dev Error Message"] = "To increase the limit, you need to use authorisation in the connection";
                        return View("Index");
                    }
                    else
                    {
                        //Other return the message coming back from the server.
                        ViewData["Dev Error Message"] = e.Message;
                        return View("Index");
                    }
                }
            }
            catch (Exception e)
            {
                ViewData["Dev Error Message"] = e.Message;
                return View("Index");
            }

            //We want to return the Index view as the UserDetails is a partial view. And we want to keep it in index
            return View("Index", model);
        }

        [HttpPost]
        public async Task<JsonResult> FindUserAutoComplete(string NameToFind)
        {
            GitHubApiService service = new GitHubApiService();
            var jsonResult = await service.FindUserName(NameToFind);

            //var jsonList = JsonConvert.DeserializeObject<List<GitHubUserSearchViewModel>>(jsonResult);

            
            dynamic jsonParsed = JsonConvert.DeserializeObject(jsonResult);

            List<string> logins = new List<string>();
            foreach (var login in jsonParsed.items)
            {
                if (login.login != null)
                {
                    string value = login.login;
                    logins.Add(value);
                }
            }
            

            return Json(logins);
        }
    }
}
