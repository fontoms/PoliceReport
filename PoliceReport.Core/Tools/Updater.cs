using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Net;
using System.Reflection;

namespace PoliceReport.Core.Tools
{
    public class Updater
    {
        private readonly string githubContentsUrl = Constants.ApiContentUrl;
        public event EventHandler<double> ProgressChanged;

        protected virtual void OnProgressChanged(double progress)
        {
            ProgressChanged?.Invoke(this, progress);
        }

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

        public async Task<bool> CheckIfUpdated()
        {
            try
            {
                // Récupère la version du fichier local et ses dates
                var localFile = GetLocalDatabaseVersion();
                string version = localFile.version;
                DateTime localCreateDate = localFile.localCreateDate;
                DateTime localModifiedDate = localFile.localModifiedDate;

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", "request");
                string githubContentsJson = await client.GetStringAsync(githubContentsUrl);
                JArray githubContentsArray = JArray.Parse(githubContentsJson);

                foreach (JObject content in githubContentsArray)
                {
                    string remoteName = content["name"].ToString();

                    // Vérifie si le fichier est une base de données avec le bon format de nom
                    if (remoteName.StartsWith("PR_BDD_") && remoteName.EndsWith(".db"))
                    {
                        // Extrait la version du nom de fichier (format attendu : PR_BDD_x.x.x.db)
                        string[] nameParts = remoteName.Split('_', '.');
                        if (nameParts.Length >= 4)
                        {
                            string remoteVersionStr = $"{nameParts[2]}.{nameParts[3]}.{nameParts[4]}";
                            Version remoteVersion = new Version(remoteVersionStr);

                            // Convertit la version locale en objet Version pour la comparaison
                            Version localVersion = new Version(version);

                            OnProgressChanged(githubContentsArray.IndexOf(content) * 100 / githubContentsArray.Count);

                            // Compare les versions
                            if (remoteVersion > localVersion)
                            {
                                ConfigurationManager.AppSettings["LatestUpdate"] = DateTime.Now.ToString(); // Met à jour la date de dernière mise à jour
                                return true; // La base de données est mise à jour, donc retourne true
                            }
                        }
                    }
                }

                // Compare la date de dernière mise à jour avec la date de modification locale du fichier de BDD
                DateTime localDate = localModifiedDate.AddMinutes(-1); // Ajoute 5 minutes pour compenser le décalage horaire
                if (localDate > DateTime.Parse(ConfigurationManager.AppSettings["LatestUpdate"]))
                {
                    ConfigurationManager.AppSettings["LatestUpdate"] = DateTime.Now.ToString(); // Met à jour la date de dernière mise à jour
                    return true; // La base de données est mise à jour, donc retourne true
                }

                return false; // Aucune mise à jour disponible ou nécessaire
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<string> Update()
        {
            try
            {
                // Vérifie d'abord s'il y a une mise à jour disponible
                bool updateAvailable = await CheckIfUpdated();

                if (updateAvailable)
                {
                    // Récupère la liste des fichiers dans le dossier GitHub
                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Add("User-Agent", "request");
                    string githubFilesJson = await client.GetStringAsync(githubContentsUrl);
                    JArray githubFilesArray = JArray.Parse(githubFilesJson);

                    foreach (JObject file in githubFilesArray)
                    {
                        // Récupère le nom du fichier
                        string fileName = Path.GetFileNameWithoutExtension(file["name"].ToString());
                        string fileNameWithExtension = file["name"].ToString();

                        // Vérifie si le fichier est une base de données avec le bon format de nom
                        if (fileName.Contains("PR_BDD_"))
                        {
                            // Sépare les parties du nom de fichier pour obtenir la version
                            string[] fileNameParts = fileName.Split('_', '.');
                            string fileVersionStr = string.Join(".", fileNameParts.Skip(2));

                            Version fileVersion = new Version(fileVersionStr);

                            OnProgressChanged(githubFilesArray.IndexOf(file) * 100 / githubFilesArray.Count);

                            // Supprime l'ancienne base de données
                            string[] localDbFiles = Directory.GetFiles(Environment.CurrentDirectory, "PR_BDD_*.db");
                            string firstFile = localDbFiles.OrderByDescending(f => f).LastOrDefault();
                            if (firstFile != null)
                            {
                                File.Delete(firstFile);
                            }

                            // Télécharge le fichier depuis GitHub
                            string fileUrl = file["download_url"].ToString();
                            using (var response = await client.GetAsync(fileUrl, HttpCompletionOption.ResponseHeadersRead))
                            {
                                response.EnsureSuccessStatusCode();
                                var totalBytes = response.Content.Headers.ContentLength;

                                using (var contentStream = await response.Content.ReadAsStreamAsync())
                                using (var fileStream = new FileStream(Path.Combine(Environment.CurrentDirectory, fileNameWithExtension), FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                                {
                                    var totalRead = 0L;
                                    var buffer = new byte[8192];
                                    var isMoreToRead = true;

                                    do
                                    {
                                        var read = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                                        if (read == 0)
                                        {
                                            isMoreToRead = false;
                                            OnProgressChanged(100);
                                            continue;
                                        }

                                        await fileStream.WriteAsync(buffer, 0, read);

                                        totalRead += read;

                                        if (totalBytes.HasValue)
                                        {
                                            var progress = (totalRead * 1d) / (totalBytes.Value * 1d) * 100;
                                            OnProgressChanged(progress);
                                        }
                                    }
                                    while (isMoreToRead);
                                }
                            }

                            // Retourne la version mise à jour
                            return fileVersion.ToString();
                        }
                    }
                }
                else
                {
                    // Aucune mise à jour disponible
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

            return null; // Aucune mise à jour effectuée
        }

        private (string version, DateTime localCreateDate, DateTime localModifiedDate) GetLocalDatabaseVersion()
        {
            string localDbFolderPath = Environment.CurrentDirectory;
            string[] localDbFiles = Directory.GetFiles(localDbFolderPath, "PR_BDD_*.db");

            if (localDbFiles.Length > 0)
            {
                string latestFile = localDbFiles.OrderByDescending(f => f).FirstOrDefault();
                if (latestFile != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(latestFile);
                    FileInfo fileInfo = new FileInfo(latestFile);
                    string[] fileNameParts = fileName.Split('_', '.');
                    if (fileNameParts.Length > 2)
                    {
                        string fileVersionStr = string.Join(".", fileNameParts.Skip(2));
                        return (version: fileVersionStr, localCreateDate: fileInfo.CreationTime, localModifiedDate: fileInfo.LastWriteTime);
                    }
                }
            }

            return ("0.0.0", DateTime.MinValue, DateTime.MinValue);
        }
    }
}
