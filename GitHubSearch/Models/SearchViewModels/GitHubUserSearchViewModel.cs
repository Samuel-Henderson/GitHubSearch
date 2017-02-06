using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GitHubSearch.Models.SearchViewModels
{
    public class GitHubUserSearchViewModel
    {
        public GitHubUserSearchViewModel()
        {
            //Do nothing as a default.
        }

        /// <summary>
        /// As we are just using one user I like this a bit better so the model can keep the camel case
        /// without spliting words with an underscore. Could always use JsonConvert to deserialise it.
        /// </summary>
        /// <param name="JsonString"></param>
        public GitHubUserSearchViewModel(string JsonString)
        {
            JObject jObj = JObject.Parse(JsonString);

            Name = jObj["name"].ToString();
            Login = jObj["login"].ToString();
            Location = jObj["location"].ToString();
            AvatarUrl = jObj["avatar_url"].ToString();
            ReposUrl = jObj["repos_url"].ToString();
        }

        //User's name
        public string Name { get; set; }
        //UserName
        [Display(Name = "User Name")]
        public string Login { get; set; }
        public string Location { get; set; }
        public string AvatarUrl { get; set; }
        public string ReposUrl { get; set; }
        public List<GitHubUserRepoViewModel> Repos{ get; set; }


    }
}
