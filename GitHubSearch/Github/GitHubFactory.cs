using GitHubSearch.Github.Service;
using GitHubSearch.Models.SearchViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GitHubSearch.Github
{
    public class GitHubSettings
    {
        public string Service { get; set; }
    }

    public class GitHubFactory
    {
        private IGitHub _GitHubService;

        public GitHubFactory(GitHubApiTypes.GitHubServiceType serviceType)
        {
            switch (serviceType)
            {
                case GitHubApiTypes.GitHubServiceType.OfficalGitHubApi:
                    _GitHubService = new GitHubApiService();
                    break;
                default:
                    throw new Exception($"Unknown GitHub Service Type: {serviceType}");
            }
        }

        public async Task<GitHubUserSearchViewModel> GetUserDetails(string UserName)
        {
            //Get the result
            var jsonResult = await _GitHubService.GetUserDetails(UserName);

            //return a new GitHubUserSearchViewModel from the results
            return new GitHubUserSearchViewModel(jsonResult);
        }

        public async Task<List<GitHubUserRepoViewModel>> GetUserRepos(string RepoUrl)
        {
            //Get the result
            var jsonResult = await _GitHubService.GetUserRepos(RepoUrl);

            //Parse into a list GitHubUserRepoViewModels
            List<GitHubUserRepoViewModel> results = JsonConvert.DeserializeObject<List<GitHubUserRepoViewModel>>(jsonResult);

            return results;
        }

        public async Task<string> FindUserName(string NameToSearch)
        {
            return await _GitHubService.FindUserName(NameToSearch);
        }

    }
}
