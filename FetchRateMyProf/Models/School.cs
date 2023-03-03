using System;
using System.Collections.Generic;
using System.Net.Http;
using FetchRateMyProf.Queries;

namespace FetchRateMyProf.Models;

public class School
{
    private readonly string _schoolId;
    private readonly HttpClient _client;

    internal School(string schoolId, HttpClient? client = null)
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
        => GetPageable<ProfessorPaging, Professor>("professors", new ProfessorQuery("", _schoolId, 100));

    /// <summary>
    /// Get all reviews for a specific professor.
    /// </summary>
    /// <param name="professorId">The Rate My Professor ID for the professor.</param>
    /// <returns>A list of reviews.</returns>
    public IAsyncEnumerable<Review> GetAllReviews(string professorId) 
        => GetPageable<ReviewPaging, Review>("ratings", new RatingQuery(professorId, 100));

    /// <summary>
    /// Go through all pages of a formatted URL.
    /// </summary>
    /// <typeparam name="T">The paged type. Must implement <see cref="IPageable{T}"/>.</typeparam>
    /// <typeparam name="TU">The item to return from the paged type. Must be the same generic implementation.</typeparam>
    /// <returns>A list of items of type <see cref="TU"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown if Rate My Professor gives us invalid data.</exception>
    private async IAsyncEnumerable<TU> GetPageable<T, TU>(string queryType, IRMPQuery variables) where T : IPageable<TU>
    {
        while (true)
        {
            var page = await _client.QueryGraphQLAsync<T>(GraphQLResources.GraphQLQueries[queryType], variables);
            
            if (page == null)
                throw new InvalidOperationException("Did not get a JSON response from RateMyProfessors!");
            foreach (var item in page.Items)
                yield return item;

            variables.After = page.EndCursor;
            if (!page.HasNextPage)
                break;
        }
    }
}