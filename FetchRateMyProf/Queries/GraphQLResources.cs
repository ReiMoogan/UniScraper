using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FetchRateMyProf.Queries;

internal static class GraphQLResources
{
    private const string Url = "https://www.ratemyprofessors.com/graphql";
    private const string Username = "test";
    private const string Password = "test";
    
    private const string Credentials = $"{Username}:{Password}";
    private static readonly string BasicAuth = Convert.ToBase64String(Encoding.UTF8.GetBytes(Credentials));
    
    internal static readonly Dictionary<string, string> GraphQLQueries;

    static GraphQLResources()
    {
        GraphQLQueries = new Dictionary<string, string>();
        
        var assembly = Assembly.GetExecutingAssembly();
        var resources = assembly.GetManifestResourceNames();
        
        var graphQLResources = resources.Where(o => o.EndsWith(".graphql"));

        foreach (var resource in graphQLResources)
        {
            using var stream = assembly.GetManifestResourceStream(resource);
            if (stream == null)
                throw new InvalidOperationException($"Failed to load resource {resource}!");
            using var reader = new StreamReader(stream);
            
            // Extract the main file name - hopefully they don't have extra periods.
            var extension = resource.LastIndexOf('.');
            var main = resource[..extension];
            var beforeFileName = main.LastIndexOf('.');

            GraphQLQueries[main[(beforeFileName + 1)..]] = reader.ReadToEnd();
        }
    }

    public static async Task<T> QueryGraphQLAsync<T>(this HttpClient client, string query, object variables, CancellationToken cancellationToken = default)
    {
        var message = new HttpRequestMessage(HttpMethod.Post, Url);
        message.Headers.Authorization = new AuthenticationHeaderValue("Basic", BasicAuth);
        message.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
        message.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
        message.Content = new StringContent(JsonConvert.SerializeObject(new
        {
            query, variables
        }), Encoding.UTF8, "application/json");

        var response = await client.SendAsync(message, cancellationToken);
        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonConvert.DeserializeObject<T>(json) ?? throw new InvalidOperationException("Response was null");
    }
}