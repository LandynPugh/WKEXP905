using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DataAccessClient.Entities;
using System.Runtime.CompilerServices;
using System.Runtime.Caching;

namespace DataAccessClient.Services
{
    internal class ApiServiceCached
    {
        static HttpClient client = new HttpClient();
        // Get all users in User table
        public static async Task<List<User>> GetAllUsersAsync()
        {
            ObjectCache cache = MemoryCache.Default;
            if (cache.Contains("users"))
            {
                return (List<User>)cache["users"];
            }
            else
            {

                string baseURL = "https://localhost:7161/api/Users";
                List<User> users = new List<User>();
                try
                {
                    HttpResponseMessage resp = await client.GetAsync(baseURL);
                    if (resp.IsSuccessStatusCode)
                    {
                        users = await resp.Content.ReadAsAsync<List<User>>();
                    }

                    CacheItemPolicy policy = new CacheItemPolicy();
                    policy.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(1);
                    cache.Add("users", users, policy);

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);

                }
                return users;

            }



        }
        // Get all faculties in Faculty table
        public static async Task<List<Faculty>> GetAllFacultiesAsync()
        {
            ObjectCache cache = MemoryCache.Default;
            if (cache.Contains("faculties"))
            {
                return (List<Faculty>)cache["faculties"];
            }
            else
            {
                string baseURL = "https://localhost:7161/api/Faculties";
                List<Faculty> faculties = new List<Faculty>();
                try
                {
                    HttpResponseMessage resp = await client.GetAsync(baseURL);
                    if (resp.IsSuccessStatusCode)
                    {
                        faculties = await resp.Content.ReadAsAsync<List<Faculty>>();
                    }

                    CacheItemPolicy policy = new CacheItemPolicy();
                    policy.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(1);
                    cache.Add("faculties", faculties, policy);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                return faculties;
            }
        }
        // Get all colleges in College table
        public static async Task<List<College>> GetAllCollegesAsync()
        {
            ObjectCache cache = MemoryCache.Default;
            if (cache.Contains("colleges"))
            {
                return (List<College>)cache["colleges"];
            }
            else
            {
                string baseURL = "https://localhost:7161/api/Colleges";
                List<College> colleges = new List<College>();
                try
                {
                    HttpResponseMessage resp = await client.GetAsync(baseURL);
                    if (resp.IsSuccessStatusCode)
                    {
                        colleges = await resp.Content.ReadAsAsync<List<College>>();
                    }

                    CacheItemPolicy policy = new CacheItemPolicy();
                    policy.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(1);
                    cache.Add("colleges", colleges, policy);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                return colleges;

            }

        }

        // Enter new user into User table
        public static async Task<List<User>> CreateNewUsers(List<User> users)
        {
            string baseURL = "https://localhost:7161/api/Users";
            List<User> results = new List<User>();
            try
            {
                foreach (User user in users)
                {
                    HttpResponseMessage resp = await client.PostAsJsonAsync(baseURL, user);
                    if (resp.IsSuccessStatusCode)
                    {
                        results.Add(await resp.Content.ReadAsAsync<User>());
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return results;
        }


    }
}
