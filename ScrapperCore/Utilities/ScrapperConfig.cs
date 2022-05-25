using System;
using System.Data.SqlClient;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ScrapperCore.Utilities;

public class ScrapperConfig
{
    [JsonProperty] public int Port { get; private set; }
    [JsonProperty] public string SqlServer { get; private set; }
    [JsonProperty] public int SqlPort { get; private set; }
    [JsonProperty] public string SqlPassword { get; private set; }
    [JsonIgnore]
    public string SqlConnection
    {
        get
        {
            var output = new SqlConnectionStringBuilder
            {
                DataSource = $"{SqlServer},{SqlPort}",
                IntegratedSecurity = false,
                UserID = "UniScraper",
                Password = SqlPassword,
                InitialCatalog = "UniScraper",
                MultipleActiveResultSets = true
            };
            return output.ConnectionString;
        }
    }
    [JsonIgnore] public JObject RawData { get; private set; }

    private ScrapperConfig()
    {
        Port = 5003;
        SqlPort = 1433;
    }
        
    public static ScrapperConfig Load(string file = "config.json")
    {
        var json = "{}";
        if (File.Exists(file))
            json = File.ReadAllText(file);
        var config = new ScrapperConfig();
        JsonConvert.PopulateObject(json, config);
        config.RawData = JObject.Parse(json);
        // Update config, so we can bring in new changes if there are updates.
        config.Save(file);
        return config;
    }

    public void Save(string file = "config.json")
    {
        File.WriteAllText(file ?? "config.json", JsonConvert.SerializeObject(this, Formatting.Indented));
    }

    public ScrapperConfig Verify()
    {
        if (Port is not (>= 1 and <= 65535))
            throw new InvalidOperationException($"Invalid port number in config! ({Port}");
        return this;
    }
}