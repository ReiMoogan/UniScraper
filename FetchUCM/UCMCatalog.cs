using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FetchUCM.Models;
using Newtonsoft.Json;

namespace FetchUCM
{
    public class UCMCatalog
    {
        private static readonly HttpClient Client;
        private readonly HttpClient _client;

        static UCMCatalog()
        {
            var cookies = new CookieContainer();
            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = true,
                UseCookies = true,
                CookieContainer = cookies,
                Proxy = null,
                UseProxy = false
            };
            Client = new HttpClient(handler);
            Client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/98.0.4758.102 Safari/537.36");
        }
        
        public UCMCatalog(HttpClient client = null)
        {
            _client = client ?? Client;
        }

        private async Task GetCookie(int term)
        {
            await Client.GetAsync(
                $"https://reg-prod.ec.ucmerced.edu/StudentRegistrationSsb/ssb/term/search?mode=courseSearch&term={term}&studyPath=&studyPathText=&startDatepicker=&endDatepicker=");
            await Client.GetAsync(
                "https://reg-prod.ec.ucmerced.edu/StudentRegistrationSsb/ssb/courseSearch/courseSearch");
        }
        
        public async Task<IEnumerable<Class>> GetAllClasses(int term)
        {
            await GetCookie(term);
            var sections = await CountSections(term);
            var output = new ConcurrentBag<Class>();
            var fetchTasks = new List<Task>();
            for (var i = 0; i < sections / 500 + 1; ++i)
            {
                fetchTasks.Add(GetClassPagination(term, i * 500, output));
            }
            await Task.WhenAll(fetchTasks);
            return output;
        }
        
        private async Task<int> CountSections(int term)
        {
            var classesUrl =
                $"https://reg-prod.ec.ucmerced.edu/StudentRegistrationSsb/ssb/searchResults/searchResults?txt_term={term}&pageOffset=0&pageMaxSize=1&sortColumn=subjectDescription&sortDirection=asc";
            var response = await _client.GetAsync(classesUrl);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<ClassPaging>(json, new ClassJsonConverter());
            if (data == null)
                throw new InvalidOperationException("Did not get a JSON response querying for classes!");
            if (!data.Success)
                throw new InvalidOperationException($"Response indicated failure!\n{data}");
            return data.SectionsFetchedCount;
        }

        private async Task GetClassPagination(int term, int pageOffset, ConcurrentBag<Class> output)
        {
            var classesUrl =
                $"https://reg-prod.ec.ucmerced.edu/StudentRegistrationSsb/ssb/searchResults/searchResults?txt_term={term}&pageOffset={pageOffset}&pageMaxSize=500&sortColumn=subjectDescription&sortDirection=asc";
            var response = await _client.GetAsync(classesUrl);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<ClassPaging>(json, new ClassJsonConverter());
            if (data == null)
                throw new InvalidOperationException("Did not get a JSON response querying for classes!");
            if (!data.Success)
                throw new InvalidOperationException($"Response indicated failure!\n{data}");
            if (data.Items == null)
                throw new InvalidOperationException("Data was invalid!");
            foreach (var item in data.Items)
                output.Add(item);
        }

        public async Task<Term[]> GetAllTerms()
        {
            const string termsURL = "https://reg-prod.ec.ucmerced.edu/StudentRegistrationSsb/ssb/classSearch/getTerms?searchTerm=&offset=1&max=100";
            var response = await _client.GetAsync(termsURL);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<Term[]>(json);
            if (data == null)
                throw new InvalidOperationException("Did not get a JSON response querying for terms!");
            return data;
        }
    }
}