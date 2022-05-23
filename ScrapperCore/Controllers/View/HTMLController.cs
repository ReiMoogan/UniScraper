using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ScrapperCore.Utilities;

namespace ScrapperCore.Controllers.View;

public abstract class HTMLController : ControllerBase
{
    private readonly ILogger<HTMLController> _logger;
    private readonly ScrapperConfig _config;
        
    protected readonly Dictionary<string, string> Router = new();

    protected HTMLController(ILogger<HTMLController> logger, ScrapperConfig config)
    {
        _logger = logger;
        _config = config;
    }

    protected abstract void SetupRouter();

    protected void AddFolderToRouter(string root, string folder)
    {
        if (!Directory.Exists(folder))
        {
            _logger.LogWarning("The folder {Folder} does not exist; the router cannot handle this!", folder);
            return;
        }
        var files = Directory.EnumerateFiles(folder);
        foreach (var file in files)
        {
            var key = (string.IsNullOrWhiteSpace(root) ? "" : root + '/') + Path.GetFileName(file);
            key = key.ToLower();
            var value = $"{file}".Replace('\\', '/');
            var success = Router.TryAdd(key, value);
            if (!success)
                _logger.LogWarning("Router found a duplicate route for {Key}:\n- Old: {RouterValue}\n- New: {Value}", key, Router[key], value);
        }
        var directories = Directory.EnumerateDirectories(folder);
        foreach (var directory in directories)
            AddFolderToRouter((root == "" ? "" : root + '/') + Path.GetDirectoryName(directory)?.Replace('\\', '/'), $"{directory}");
    }

    [HttpGet]
    public virtual async Task<IActionResult> Get(string page)
    {
        if(Router.Count == 0)
            SetupRouter();
            
        page ??= "";
        page = page.ToLower();
        try
        {
            var found = Router.TryGetValue(page, out var file);
            if (!found)
                throw new FileNotFoundException();
                
            if(file.EndsWith("html"))
                return base.Content(await Templater(file), GetMIME(file));
            return base.PhysicalFile(Path.GetFullPath(file), GetMIME(file));
        }
        catch (FileNotFoundException)
        {
            Response.StatusCode = 404;
            const string file404 = "StaticViews/views/error/404.html";
            return System.IO.File.Exists(file404) ? base.Content(await Templater(file404), GetMIME(file404)) : base.Content("Error 404!!");
        }
    }

    public static string GetMIME(string file)
    {
        var provider = new FileExtensionContentTypeProvider();
        return !provider.TryGetContentType(file, out var contentType) ? "application/octet-stream" : contentType;
    }
        
    private enum TemplateCode { Fail, Include, Title, Description, Config }

    private async Task<string> Templater(string file)
    {
        var text = await System.IO.File.ReadAllTextAsync(file);
        if (!file.EndsWith("html"))
            return text;
            
        string title = null;
        string description = null;
            
        var evaluator = new MatchEvaluator(match =>
        {
            var (code, output) = TemplateInterpreter(match);
            switch (code)
            {
                case TemplateCode.Include:
                    return output;
                case TemplateCode.Title:
                    title = output;
                    return "";
                case TemplateCode.Description:
                    description = output;
                    return "";
                case TemplateCode.Config:
                    return output;
                default:
                    return "";
            }
        });
            
        var newPage = Regex.Replace(text, "{({.*})}", evaluator,
            RegexOptions.IgnorePatternWhitespace,
            Regex.InfiniteMatchTimeout);
        var html = new HtmlDocument();
        html.LoadHtml(newPage);
            
        var titleNode = html.DocumentNode.SelectSingleNode("//title");
        var titleOGNode = html.DocumentNode.SelectSingleNode("//meta[@property='og:title']");
        var descriptionNode = html.DocumentNode.SelectSingleNode("//meta[@name='description']");
        var descrptionOGNode = html.DocumentNode.SelectSingleNode("//meta[@property='og:description']");
            
        if (title != null)
        {
            if (titleNode != null)
                titleNode.InnerHtml = title;
            titleOGNode?.SetAttributeValue("content", title);
        }
            
        if (description != null)
        {
            descriptionNode?.SetAttributeValue("content", description);
            descrptionOGNode?.SetAttributeValue("content", description);
        }

        return html.DocumentNode.OuterHtml;
    }

    private (TemplateCode Code, string Output) TemplateInterpreter(Match match)
    {
        var json = match.Result("$1");
        try
        {
            var obj = JObject.Parse(json);
                
            var file = obj["include"]?.ToString();
            if (file != null)
                return (TemplateCode.Include, System.IO.File.ReadAllText(file));
                
            var title = obj["title"]?.ToString();
            if (title != null)
                return (TemplateCode.Title, title);
                
            var description = obj["description"]?.ToString();
            if (description != null)
                return (TemplateCode.Description, description);
                
            var config = obj["config"]?.ToString();
            if (config != null)
                return (TemplateCode.Config, _config.RawData[config]?.ToString());
                
            throw new JsonException("Could not understand command!");
        }
        catch (JsonException e)
        {
            _logger.Log(LogLevel.Error, "Failed to parse JSON!\n{@Exception}", e);
            return (TemplateCode.Fail, "");
        }
    }
}