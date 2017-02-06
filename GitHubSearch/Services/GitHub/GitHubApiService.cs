using GitHubSearch.Models.SearchViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GitHubSearch.Services.GitHub
{
    public class GitHubApiService
    {

        //This string is the base for the get. We will fill in the user name from the selected user.
        private const string APIURL = "https://api.github.com/";
        //A string for the users part of the URL
        private const string APIUSERS = "users/";
        //a string for the user search. This can be used like 'APIURL + APISEARCHUSER + User to find'.
        private const string APISEARCHUSER = "search/users?q=";

        //Makes sure to search for the user's name and limits to 10 per page.
        private const string APISEARCHUSERSLIMIT = "+in:name+type:Users&per_page=10";

        public async Task<string> GetUserDetails(string Username)
        {
            //This will fill in the URL to be the GitHub Api URL and the username passed to it.
            HttpWebRequest request = SetUpGitHub($"{APIUSERS}{Username}");

            if (request == null)
                throw new Exception("Request was not able to be set up.");

            return await CallGitHubApi(request);
        }

        private async Task<string> CallGitHubApi(HttpWebRequest Request)
        {
            if (Request == null)
                throw new Exception("Request was not able to be set up.");

            string jsonResult = string.Empty;

            WebResponse response = await Request.GetResponseAsync();

            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                //Returns a Json string.
                jsonResult = await streamReader.ReadToEndAsync();
            }           

            return jsonResult;


        }

        /// <summary>
        /// Gets users with in regards to the nameToSearch limited to the APISEARCHUSERSLIMIT
        /// </summary>
        /// <param name="NameToSearch"></param>
        /// <returns></returns>
        public async Task<string> FindUserName(string NameToSearch)
        {
            HttpWebRequest request = SetUpGitHub($"{APISEARCHUSER}{NameToSearch}{APISEARCHUSERSLIMIT}");

            if (request == null)
                throw new Exception("Request was not able to be set up.");

            return await CallGitHubApi(request);
        }

        /// <summary>
        /// Will bring back all their repos.
        /// </summary>
        /// <param name="NameToSearch"></param>
        /// <returns></returns>
        public async Task<string> GetUserRepos(string UserRepoUrl)
        {
            HttpWebRequest request = SetUpGitHub($"{UserRepoUrl}", false);

            if (request == null)
                throw new Exception("Request was not able to be set up.");
            return await CallGitHubApi(request);
        }



        /// <summary>
        /// This will set up a HttpWebRequest to work with GitHub. 
        /// It already has the API link in (Https://api.github.com/). 
        /// So supply the rest. 
        /// </summary>
        /// <param name="UrlToUse"></param>
        /// <returns></returns>
        private HttpWebRequest SetUpGitHub(string UrlToUse, bool UseDefaultUrl = true)
        {
            HttpWebRequest request = null;
            if (UseDefaultUrl)
            {
                //This will fill in the URL to be the GitHub Api URL and the username passed to it.
                request = WebRequest.Create($"{APIURL}{UrlToUse}") as HttpWebRequest;
            }
            else
            {
                request = WebRequest.Create($"{UrlToUse}") as HttpWebRequest;
            }

            if (request == null)
                throw new Exception("Was not able to create HttpWebRequest");


            //The HttpWebRequest needs to have these set otherwise GitHub Api will just reject the connection.
            //This makes sure we will use V3 of the API
            request.Accept = "application/vnd.github.v3+json";
            //Making sure we are getting json only.
            request.ContentType = "application/json";
            //Using a GET on the API
            request.Method = "GET";

            //This was the final peice to get this to work. Without a User-Agent, the API rejects the call.
            request.Headers["User-Agent"] = "GitHub API testing application : Samuel-Henderson";

            //If you run out of attempts. It is like 60 per house. Use personal Access Token to increase the amount of requests
            //request.Headers["Authorization"] = "token INSERT TOKEN HERE";

            return request;
        }
    }
}
