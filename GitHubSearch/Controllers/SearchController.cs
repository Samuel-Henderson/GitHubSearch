using GitHubSearch.Github;
using GitHubSearch.Github.Service;
using GitHubSearch.Models.SearchViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace GitHubSearch.Controllers
{
    public class SearchController : Controller
    {
        private readonly GitHubSettings _GitHubSettings;
        private readonly GitHubFactory _Factory;

        public SearchController(IOptions<GitHubSettings> githubSettings)
        {
            //Gets the details from appsettings.json
            _GitHubSettings = githubSettings.Value;

            //Creates the factory with with the service type from appsettings.json
            _Factory = new GitHubFactory(GitHubApiTypes.GetGitHubServiceType(_GitHubSettings.Service));
        }

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

            var githubServiceType = GitHubApiTypes.GetGitHubServiceType(_GitHubSettings.Service);

            GitHubUserSearchViewModel model = null;

            try
            {

                model = await _Factory.GetUserDetails(Login);

                List<GitHubUserRepoViewModel> userRepos = await _Factory.GetUserRepos(model.ReposUrl);

                //We are now ordering the list by Stargazers
                List<GitHubUserRepoViewModel> userReposSorted = userRepos.OrderByDescending(m => m.Stargazers_Count).ToList();

                //Now we are only taking the top 5
                userReposSorted = userReposSorted.Take(5).ToList();

                model.Repos = userReposSorted;

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
            var jsonResult = await _Factory.FindUserName(NameToFind);
            
            //We are using a dynamic as this does not really need its own class. 
            //We are not saving any data we are not even storing it here we are just turning it 
            //one element to a list of strings.
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
