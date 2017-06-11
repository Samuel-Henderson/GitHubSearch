using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GitHubSearch.Github.Service
{
    public static class GitHubApiTypes
    {
        public enum GitHubServiceType
        {
            OfficalGitHubApi = 1,
            Nothing
        }

        /// <summary>
        /// This will get the service type to use from appsettings.json
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static GitHubServiceType GetGitHubServiceType(string type)
        { 
            switch (type.ToLower())
            {
                case "offical":
                    return GitHubServiceType.OfficalGitHubApi;
                default:
                    return GitHubServiceType.Nothing;
            }
        }

        /// <summary>
        /// Gets the name of the service type from the enum
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetGitHubServiceName(GitHubServiceType type)
        {
            switch (type)
            {
                case GitHubServiceType.OfficalGitHubApi:
                    return "Offical";
                case GitHubServiceType.Nothing:
                default:
                    return "Nothing";
            }
        }
    }
}
