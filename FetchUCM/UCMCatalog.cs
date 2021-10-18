using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FetchUCM.Models;
using Newtonsoft.Json;

namespace FetchUCM
{
    public class UCMCatalog
    {
        private readonly HttpClient _client;
        
        public UCMCatalog(HttpClient client = null)
        {
            _client = client ?? new HttpClient();
        }
        
        public async IAsyncEnumerable<Class> GetAllClasses(int term, string jsessionid)
        {
            if (_client.DefaultRequestHeaders.Contains("cookie"))
                _client.DefaultRequestHeaders.Remove("cookie");
            _client.DefaultRequestHeaders.Add("cookie", $"JSESSIONID={jsessionid}");
            
            const int maxAttempts = 20;
            var pageOffset = 0;
            var sectionsFetchedCount = 1;

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
                            await Console.Error.WriteLineAsync(
                                $"Failed to get class data after {maxAttempts} attempts! Is the JSESSIONID valid?");
                            sectionsFetchedCount = pageOffset; // Exit while loop, jump to the end of method.
                            break;
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
                Console.WriteLine("Got a page!");
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