﻿using Canvas.Clients.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Canvas.Clients
{
    public class AccountsClient : ClientBase
    {
        public AccountsClient() : base($"accounts") { }

        /// <summary>
        /// Will return the first user in a list of users.
        /// </summary>
        /// <param name="searchTerm">Search term can be a user id or email address</param>
        /// <returns>User if found.  Null otherwise.</returns>
        public async Task<User> GetUserAsync(string searchTerm)
        {
            ApiPath = ApiController + $@"/self/users?search_term={searchTerm}";
            var result = JArray.Parse(await ExecuteGet(ApiPath));

            User user = result.Count > 0 ? JsonConvert.DeserializeObject<User>(result.First().ToString()) : null;

            return user;
        }

        public async Task<List<Role>> GetAccountRolesForUserAsync(string accountId, string userId)
        {
            var roles = new List<Role>();

            // Get the account
            var account = await Get<Account>(accountId);

            // Get roles for account
            ApiPath = ApiController + $@"/{accountId}/admins?user_id[]={userId}";
            var result = JArray.Parse(await ExecuteGet(ApiPath));
            if(result.Count > 0)
            {
                foreach(var role in result.Children<JObject>())
                {
                    roles.Add(JsonConvert.DeserializeObject<Role>(role.ToString()));
                }
            }

            // Get roles for parent account (recursive)
            if (!string.IsNullOrWhiteSpace(account.ParentAccountId))
            {
                roles.AddRange(await GetAccountRolesForUserAsync(account.ParentAccountId, userId));
            }

            return roles;
        }

        public async Task<List<EnrollmentTerm>> GetEnrollmentTerms(string accountId)
        {
            ApiPath = ApiController + $@"/{accountId}/terms?per_page=50";
            var result = await ExecuteGet(ApiPath);
            var termsJson = JObject.Parse(result)["enrollment_terms"].ToString();
            var termsList = JsonConvert.DeserializeObject<List<EnrollmentTerm>>(termsJson);

            // Remove Default Term, Migration, Orientation, and Sandbox EnrollmentTerms
            termsList.RemoveAll(x => x.Id == "1" || x.Id == "35" || x.Id == "38" || x.Id == "39");

            termsList.Sort();
            termsList.Reverse();

            return termsList;
        }

        public async Task<IEnumerable<Course>> GetUnpublishedCoursesForTerm(string accountId, string termId)
        {
            ApiPath = ApiController + $@"/{accountId}/courses?published=false&enrollment_term_id={termId}";
            return await ExecuteGetAll<Course>(ApiPath);
        }

        public async Task<IEnumerable<Course>> GetAllCourses(string accountId, string query)
        {
            ApiPath = ApiController + $@"/{accountId}/courses?search_by=course&search_term={query}";
            return await ExecuteGetAll<Course>(ApiPath);
        }
    }
}
