using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FetchUCM.Models;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;

namespace FetchUCM;

/// <summary>
/// This class handles the per-class fetching, such as linked courses. All require a CRN and term.
/// </summary>
public partial class UCMCatalog
{
    private FormUrlEncodedContent WithValues(int term, int courseReferenceNumber)
    {
        var values = new Dictionary<string, string>
        {
            { "term", term.ToString() },
            { "courseReferenceNumber", courseReferenceNumber.ToString() },
            { "first", "first" } // ??? what is this
        };

        return new FormUrlEncodedContent(values);
    }
    
    public async Task<string> GetClassDetails(int term, int courseReferenceNumber)
    {
        var courseDescriptionUrl = GenerateURL("searchResults/getClassDetails", post: true);
        var response = await _client.PostAsync(courseDescriptionUrl, WithValues(term, courseReferenceNumber));
        response.EnsureSuccessStatusCode();
        var text = await response.Content.ReadAsStringAsync();
        return text.Trim(); // Output is HTML
    }
    
    public async Task<string> GetSectionBookstoreDetails(int term, int courseReferenceNumber)
    {
        var courseDescriptionUrl = GenerateURL("searchResults/getSectionBookstoreDetails", post: true);
        var response = await _client.PostAsync(courseDescriptionUrl, WithValues(term, courseReferenceNumber));
        response.EnsureSuccessStatusCode();
        var text = await response.Content.ReadAsStringAsync();
        return text.Trim(); // Output is HTML
    }

    /// <summary>
    /// Get a (possibly HTML-encoded) description of a class.
    /// </summary>
    /// <param name="term">The term to search in.</param>
    /// <param name="courseReferenceNumber">The CRN for the course.</param>
    /// <returns></returns>
    public async Task<string> GetCourseDescription(int term, int courseReferenceNumber)
    {
        var courseDescriptionUrl = GenerateURL("searchResults/getCourseDescription", post: true);
        var response = await _client.PostAsync(courseDescriptionUrl, WithValues(term, courseReferenceNumber));
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }
        var text = await response.Content.ReadAsStringAsync();
        return text.Trim(); // Output is HTML
    }
    
    public async Task<string> GetSyllabus(int term, int courseReferenceNumber)
    {
        var courseDescriptionUrl = GenerateURL("searchResults/getSyllabus", post: true);
        var response = await _client.PostAsync(courseDescriptionUrl, WithValues(term, courseReferenceNumber));
        response.EnsureSuccessStatusCode();
        var text = await response.Content.ReadAsStringAsync();
        return text.Trim(); // Output is HTML
    }
    
    public async Task<string> GetRestrictions(int term, int courseReferenceNumber)
    {
        var courseDescriptionUrl = GenerateURL("searchResults/getRestrictions", post: true);
        var response = await _client.PostAsync(courseDescriptionUrl, WithValues(term, courseReferenceNumber));
        response.EnsureSuccessStatusCode();
        var text = await response.Content.ReadAsStringAsync();
        return text.Trim(); // Output is HTML
    }
    
    public async Task<IEnumerable<FacultyMeetingTimes>> GetFacultyMeetingTimes(int term, int courseReferenceNumber)
    {
        // Weird naming, so I'll override with this:
        var courseDescriptionUrl = GenerateURL("searchResults/getFacultyMeetingTimes", post: true) + $"?term={term}&courseReferenceNumber={courseReferenceNumber}";
        
        var response = await _client.GetAsync(courseDescriptionUrl);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var parsed = JObject.Parse(json);
        var array = (JArray) parsed["fmt"];
        return array?.ToObject<IEnumerable<FacultyMeetingTimes>>();
    }
    
    public async Task<string> GetEnrollmentInfo(int term, int courseReferenceNumber)
    {
        var courseDescriptionUrl = GenerateURL("searchResults/getEnrollmentInfo", post: true);
        var response = await _client.PostAsync(courseDescriptionUrl, WithValues(term, courseReferenceNumber));
        response.EnsureSuccessStatusCode();
        var text = await response.Content.ReadAsStringAsync();
        return text.Trim(); // Output is HTML
    }
    
    public async Task<string> GetCorequisites(int term, int courseReferenceNumber)
    {
        var courseDescriptionUrl = GenerateURL("searchResults/getCorequisites", post: true);
        var response = await _client.PostAsync(courseDescriptionUrl, WithValues(term, courseReferenceNumber));
        response.EnsureSuccessStatusCode();
        var text = await response.Content.ReadAsStringAsync();
        return text.Trim(); // Output is HTML
    }
    
    public async Task<string> GetPrerequisites(int term, int courseReferenceNumber)
    {
        var courseDescriptionUrl = GenerateURL("searchResults/getSectionPrerequisites", post: true);
        

        
        var response = await _client.PostAsync(courseDescriptionUrl, WithValues(term, courseReferenceNumber));
        response.EnsureSuccessStatusCode();
        var text = await response.Content.ReadAsStringAsync();
        return text.Trim(); // Output is HTML
    }
    
    public async Task<string> GetMutualExclusions(int term, int courseReferenceNumber)
    {
        var courseDescriptionUrl = GenerateURL("searchResults/getCourseMutuallyExclusions", post: true);
        var response = await _client.PostAsync(courseDescriptionUrl, WithValues(term, courseReferenceNumber));
        response.EnsureSuccessStatusCode();
        var text = await response.Content.ReadAsStringAsync();
        return text.Trim(); // Output is HTML
    }
    
    public async Task<string> GetCrosslistedSections(int term, int courseReferenceNumber)
    {
        var courseDescriptionUrl = GenerateURL("searchResults/getXlstSections", post: true);
        var response = await _client.PostAsync(courseDescriptionUrl, WithValues(term, courseReferenceNumber));
        response.EnsureSuccessStatusCode();
        var text = await response.Content.ReadAsStringAsync();
        return text.Trim(); // Output is HTML
    }
    
    public async Task<IEnumerable<int>> GetLinkedSections(int term, int courseReferenceNumber)
    {
        var courseDescriptionUrl = GenerateURL("searchResults/getLinkedSections", post: true);
        var response = await _client.PostAsync(courseDescriptionUrl, WithValues(term, courseReferenceNumber));
        response.EnsureSuccessStatusCode();
        var text = await response.Content.ReadAsStringAsync();
        
        var document = new HtmlDocument();
        document.LoadHtml(text);
        var table = document.DocumentNode
            .SelectNodes("//table/tbody/tr/td[4]") // List is Title, Schedule Type, Section, and CRN.
            .Select(o => o.InnerHtml)
            .Select(o => int.TryParse(o, out var numeric) ? numeric : -1) // Only get the numerics, if something weird happens.
            .Where(o => o != -1);

        return table; // Output is a list of CRNs
    }
    
    public async Task<string> GetFees(int term, int courseReferenceNumber)
    {
        var courseDescriptionUrl = GenerateURL("searchResults/getFees", post: true);
        var response = await _client.PostAsync(courseDescriptionUrl, WithValues(term, courseReferenceNumber));
        response.EnsureSuccessStatusCode();
        var text = await response.Content.ReadAsStringAsync();
        
        var document = new HtmlDocument();
        document.LoadHtml(text);
        var fees = document.DocumentNode
            .SelectNodes("//text()")
            .First()
            .InnerText;
        
        return fees.Trim(); // Output is either plain-text or HTML. I don't think UCM uses this.
    }
    
    public async Task<string> GetSectionCatalogDetails(int term, int courseReferenceNumber)
    {
        var courseDescriptionUrl = GenerateURL("searchResults/getSectionCatalogDetails", post: true);
        var response = await _client.PostAsync(courseDescriptionUrl, WithValues(term, courseReferenceNumber));
        response.EnsureSuccessStatusCode();
        var text = await response.Content.ReadAsStringAsync();
        return text.Trim(); // Output is HTML
    }
}