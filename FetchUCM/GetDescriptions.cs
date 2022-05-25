using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using FetchUCM.Models;
using Newtonsoft.Json;

namespace FetchUCM;

public partial class UCMCatalog
{
    /// <summary>
    /// Descriptions may be cut off; use <see cref="GetCourseDescription"/> for the full thing.
    /// </summary>
    /// <param name="term"></param>
    /// <returns></returns>
    public async Task<IEnumerable<Description>> GetAllDescriptions(int term)
    {
        await GetCookie(term);
        var sections = await CountCourses(term);
        var output = new ConcurrentBag<Description>();
        var fetchTasks = new List<Task>();
        for (var i = 0; i < sections / 500 + 1; ++i)
        {
            fetchTasks.Add(GetDescriptionPagination(term, i * 500, output));
        }
        await Task.WhenAll(fetchTasks);
        return output;
    }
        
    private async Task<int> CountCourses(int term)
    {
        var classesUrl = GenerateURL("courseSearchResults/courseSearchResults", term, 0, 1);
        var response = await _client.GetAsync(classesUrl);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<DescriptionPaging>(json, new ClassJsonConverter());
        EnsureValidPagination(data);
        return data!.SectionsFetchedCount;
    }

    private async Task GetDescriptionPagination(int term, int pageOffset, ConcurrentBag<Description> output)
    {
        var classesUrl = GenerateURL("courseSearchResults/courseSearchResults", term, pageOffset, 500);
        var response = await _client.GetAsync(classesUrl);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<DescriptionPaging>(json, new ClassJsonConverter());
        EnsureValidPagination(data);
        foreach (var item in data!.Items)
        {
            output.Add(item);
        }
    }
}