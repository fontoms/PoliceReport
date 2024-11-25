namespace PoliceReport.Core.Outils
{
    public static class Constants
    {
        //=== PoliceReport ===\\
        // Main
        public const string Author = "Fontom's (677154388065910822)";
        public const string RepoUrl = $"https://github.com/{RepoOwner}/{RepoName}";

        // MiseAJour
        public const string RepoOwner = "fontoms";
        public const string RepoName = "PoliceReport";
        public const string ApiRepoUrl = $"https://api.github.com/repos/{RepoOwner}/{RepoName}/releases/latest";

        // Connexion
        public static List<Utilisateur.Utilisateur> Users = [
            new("admin", "8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918", 1),
            new("editor", "262121c5372be8af3ae6ff0d3d138d9e6e1249335222c7f0e02535e35073bb0b", 2),
            new("reader", "3316348dbadfb7b11c7c2ea235949419e23f9fa898ad2c198f999617912a9925", 3)
            ];

        //=== PoliceReport.Database ===\\
        public const string FolderBdd = "PoliceReport.Database";
        public const string ApiContentUrl = $"https://api.github.com/repos/{RepoOwner}/{RepoName}/contents/{FolderBdd}";
    }
}
