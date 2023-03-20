using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;

namespace ScrapperCore.Controllers.View;

public abstract class HTMLController : ControllerBase
{
    private readonly ILogger<HTMLController> _logger;
        
    protected readonly Dictionary<string, string> Router = new();

    protected HTMLController(ILogger<HTMLController> logger)
    {
        _logger = logger;
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
            AddFolderToRouter((root == "" ? "" : root + '/') + Path.GetFileName(directory).Replace('\\', '/'), $"{directory}");
    }

    [HttpGet]
    public IActionResult Get(string? page)
    {
        if(Router.Count == 0)
            SetupRouter();
        
        page ??= "";
        page = page.ToLower();
        try
        {
            var found = Router.TryGetValue(page, out var file);
            if (!found || file == null)
                throw new FileNotFoundException();
            
            return base.PhysicalFile(Path.GetFullPath(file), GetMIME(file));
        }
        catch (FileNotFoundException)
        {
            Response.StatusCode = 404;
            return base.Content("you are truly lost");
        }
    }

    private static string GetMIME(string file)
    {
        var provider = new FileExtensionContentTypeProvider();
        return !provider.TryGetContentType(file, out var contentType) ? "application/octet-stream" : contentType;
    }
}