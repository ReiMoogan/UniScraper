using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;

namespace FetchRateMyProf.Models;

public class School
{
    private readonly int _schoolId;
    private readonly HttpClient _client;

    internal School(int schoolId, HttpClient? client = null)
    {
        _schoolId = schoolId;
        _client = client ?? new HttpClient();
    }

    /// <summary>
    /// Get all professors for the school.
    /// </summary>
    /// <exception cref="HttpRequestException">RateMyProfessors returned a bad status code (possibly hit limit?)</exception>
    /// <exception cref="InvalidOperationException">RateMyProfessors did not return JSON (for some reason)</exception>
    public IAsyncEnumerable<Professor> GetAllProfessors()
        => GetPageable<ProfessorPaging, Professor>(
            $"https://www.ratemyprofessors.com/filter/professor/?&page={{0}}&filter=teacherlastname_sort_s+asc&query=*%3A*&queryoption=TEACHER&queryBy=schoolId&sid={_schoolId}");

    /// <summary>
    /// Get all reviews for a specific professor.
    /// </summary>
    /// <param name="professorId">The Rate My Professor ID for the professor.</param>
    /// <returns>A list of reviews.</returns>
    public async IAsyncEnumerable<Review> GetAllReviews(int professorId)
    {
        var enumerable = GetPageable<ReviewPaging, Review>(
            $"https://www.ratemyprofessors.com/paginate/professors/ratings?tid={professorId}&page={{}}");
        await foreach (var item in enumerable)
            if (item.SchoolId == _schoolId)
                yield return item;
    }

    /// <summary>
    /// Go through all pages of a formatted URL.
    /// </summary>
    /// <param name="url">A URL, with one {} to inject a page number in (for string.Format).</param>
    /// <typeparam name="T">The paged type. Must implement <see cref="IPageable{T}"/>.</typeparam>
    /// <typeparam name="TU">The item to return from the paged type. Must be the same generic implementation.</typeparam>
    /// <returns>A list of items of type <see cref="TU"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown if Rate My Professor gives us invalid data.</exception>
    private async IAsyncEnumerable<TU> GetPageable<T, TU>(string url) where T : IPageable<TU>
    {
        var pageNum = 1;
        var remaining = 1;
        while (remaining > 0)
        {
            var pagedUrl = string.Format(url, pageNum);
            var response = await _client.GetAsync(pagedUrl);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var page = JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings
            {
                Error = (_, args) =>
                {
                    Console.Error.WriteLine(args.ErrorContext.Error);
                    args.ErrorContext.Handled = true; // Ignore because sometimes RateMyProfessor gives us N/A instead of a number >:[
                }
            });
            if (page == null)
                throw new InvalidOperationException("Did not get a JSON response from RateMyProfessors!");
            remaining = page.Remaining;
            foreach (var item in page.Items)
                yield return item;
            ++pageNum;
        }
    }
}