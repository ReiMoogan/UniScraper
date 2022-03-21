using System;
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
        private static readonly CookieContainer Cookies;
        private readonly HttpClient _client;

        static UCMCatalog()
        {
            Cookies = new CookieContainer();
            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = true,
                UseCookies = true,
                CookieContainer = Cookies
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
        
        public async IAsyncEnumerable<Class> GetAllClasses(int term)
        {
            const int maxAttempts = 5;
            var pageOffset = 0;
            var sectionsFetchedCount = 1;
            await GetCookie(term);

            while (pageOffset < sectionsFetchedCount)
            {
                for (var attempt = 0; attempt < maxAttempts; ++attempt)
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
                    {
                        if (attempt == maxAttempts - 1)
                        {
                            throw new TimeoutException(
                                $"Failed to get class data after {maxAttempts} attempts! Is the JSESSIONID valid?");
                        }

                        await Task.Delay(750); // Ugh, this API sucks.
                        continue; // Run to next attempt, do not try to process null.
                    }
                    sectionsFetchedCount = data.SectionsFetchedCount;
                    foreach (var item in data.Items)
                    {
                        yield return item;
                        ++pageOffset;
                    }

                    break;
                }
            }
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