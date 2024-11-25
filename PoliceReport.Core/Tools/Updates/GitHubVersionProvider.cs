namespace PoliceReport.Core.Tools.Updates
{
    public class GitHubVersionProvider : IVersionProvider
    {
        private readonly string _apiRepoUrl;
        private readonly HttpClient _httpClient;

        public GitHubVersionProvider(string apiRepoUrl)
        {
            _apiRepoUrl = apiRepoUrl;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "request");
        }

        public async Task<string> GetLatestReleaseInfoAsync()
        {
            return await _httpClient.GetStringAsync(_apiRepoUrl);
        }

        public string ExtractDownloadUrl(string releaseInfo)
        {
            return releaseInfo.Split(new string[] { "\"browser_download_url\":" }, StringSplitOptions.None)[1]
                .Split(',')[0]
                .Trim()
                .Replace("\"", "")
                .TrimEnd('}');
        }
    }
}
