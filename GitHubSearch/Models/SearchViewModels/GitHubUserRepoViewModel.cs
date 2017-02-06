using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GitHubSearch.Models.SearchViewModels
{
    public class GitHubUserRepoViewModel
    {

        public GitHubUserRepoViewModel()
        {
            //Default constructor to do nothing
        }

        /// <summary>
        /// This can be used if we are not deserialising with JsonConvert and just passing in the Json string
        /// </summary>
        /// <param name="JsonString"></param>
        public GitHubUserRepoViewModel(string JsonString)
        {
            JObject jObj = JObject.Parse(JsonString);

            Name = jObj["name"].ToString();
            Description = jObj["description"].ToString();
            Html_Url = jObj["html_url"].ToString();

            Stargazers_Count = (int)jObj["stargazers_count"];
        }

        [Display(Name="Api Name")]
        public string Name { get; set; }

        public string Description { get; set; }

        [Display(Name="Url")]
        public string Html_Url { get; set; }

        [Display(Name="Stargazers")]
        public int Stargazers_Count { get; set; }
    }
}
