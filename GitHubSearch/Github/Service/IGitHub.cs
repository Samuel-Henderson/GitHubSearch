using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GitHubSearch.Github.Service
{
    interface IGitHub
    {

        /// <summary>
        /// This will take the username and will get all the details needed from github
        /// </summary>
        /// <param name="Username">Username to get details for</param>
        /// <returns></returns>
        Task<string> GetUserDetails(string Username);

        /// <summary>
        /// This will proved a JSON string with a number of results for a name which has been search.
        /// The gives us a list of usernames for a name the user searches. Limits to 10 per search.
        /// </summary>
        /// <param name="NameToSearch">A persons name to search for</param>
        /// <returns></returns>
        Task<string> FindUserName(string NameToSearch);

        /// <summary>
        /// Takes the Users repo URL and pulls back repos for that user.
        /// </summary>
        /// <param name="UserRepoUrl">URL provided from JSON details from GetUserDetails</param>
        /// <returns></returns>
        Task<string> GetUserRepos(string UserRepoUrl);

    }
}
