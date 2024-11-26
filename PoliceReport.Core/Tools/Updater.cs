using System.Net;
using System.Reflection;

namespace PoliceReport.Core.Tools
{
    public class Updater
    {
        public static async Task<VersionInfo> CheckUpdateAvailableAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "request");
                HttpResponseMessage response = await client.GetAsync(Constants.ApiRepoUrl);
                if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    return new VersionInfo
                    {
                        LatestVersion = null,
                        CurrentVersion = Assembly.GetEntryAssembly()?.GetName()?.Version ?? new Version("0.0.0.0")
                    };
                }
                response.EnsureSuccessStatusCode();
                string releaseInfoJson = await response.Content.ReadAsStringAsync();
                string latestVersionStr = releaseInfoJson.Split(new[] { "\"tag_name\":" }, StringSplitOptions.None)[1].Split(',')[0].Trim().Replace("\"", "");
                Version latestVersion = new Version(latestVersionStr);
                Version currentVersion = Assembly.GetEntryAssembly()?.GetName()?.Version ?? new Version("0.0.0.0");
                return new VersionInfo
                {
                    LatestVersion = latestVersion,
                    CurrentVersion = currentVersion
                };
            }
        }
    }
}
