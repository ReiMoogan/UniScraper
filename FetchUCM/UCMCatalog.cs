using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FetchUCM.Models;
using Newtonsoft.Json;

namespace FetchUCM;

public partial class UCMCatalog
{
    private static readonly HttpClient Client;
    private readonly HttpClient _client;
    private bool _cookieSet;

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
        
    public UCMCatalog(HttpClient? client = null)
    {
        _client = client ?? Client;
    }

    private async Task GetCookie(int term)
    {
        if (_cookieSet)
            return;
        
        await Client.GetAsync(
            $"https://reg-prod.ec.ucmerced.edu/StudentRegistrationSsb/ssb/term/search?mode=courseSearch&term={term}&studyPath=&studyPathText=&startDatepicker=&endDatepicker=");
        await Client.GetAsync(
            "https://reg-prod.ec.ucmerced.edu/StudentRegistrationSsb/ssb/courseSearch/courseSearch");
        _cookieSet = true;
    }

    private static string GenerateURL(string method, int? term = null, int? pageOffset = null, int? pageMaxSize = null, bool post = false)
    {
        var baseUrl = $"https://reg-prod.ec.ucmerced.edu/StudentRegistrationSsb/ssb/{method}";
        if (post)
            return baseUrl;
        var args = "";
        if (term != null)
            args += $"txt_term={term}&";
        if (pageOffset != null)
            args += $"pageOffset={pageOffset}&";
        if (pageMaxSize != null)
            args += $"pageMaxSize={pageMaxSize}&";
        return $"{baseUrl}?{args}sortColumn=subjectDescription&sortDirection=asc";
    }

    private static void EnsureValidPagination<T>(IPageable<T>? data)
    {
        if (data == null)
            throw new InvalidOperationException("Did not get a JSON response querying for classes!");
        if (!data.Success)
            throw new InvalidOperationException($"Response indicated failure!");
        if (data.Items == null)
            throw new InvalidOperationException("Data was invalid!");
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