using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using FetchUCM.Models;
using Newtonsoft.Json;

namespace FetchUCM;

public partial class UCMCatalog
{
    /// <summary>
    /// Get all available classes for the term.
    /// </summary>
    /// <param name="term">The term as an integer, such as 202230.</param>
    /// <returns>A list of classes.</returns>
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
        var classesUrl = GenerateURL("searchResults/searchResults", term, 0, 1);
        var response = await _client.GetAsync(classesUrl);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<ClassPaging>(json, new ClassJsonConverter());
        EnsureValidPagination(data);
        return data!.SectionsFetchedCount;
    }

    private async Task GetClassPagination(int term, int pageOffset, ConcurrentBag<Class> output)
    {
        var classesUrl = GenerateURL("searchResults/searchResults", term, pageOffset, 500);
        var response = await _client.GetAsync(classesUrl);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<ClassPaging>(json, new ClassJsonConverter());
        EnsureValidPagination(data);
        foreach (var item in data!.Items)
            output.Add(item);
    }
}