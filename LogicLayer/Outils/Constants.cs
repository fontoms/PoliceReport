namespace LogicLayer.Outils
{
    public static class Constants
    {
        //=== PoliceReport ===\\
        // Main
        public const string Author = "Fontom's (677154388065910822)";
        public const string RepoUrl = $"https://github.com/Fontom71/{RepoName}";

        // MiseAJour
        public const string RepoOwner = "Fontom71";
        public const string RepoName = "PoliceReport";
        public const string ApiRepoUrl = $"https://api.github.com/repos/{RepoOwner}/{RepoName}/releases/latest";

        // Connexion
        public const string AdminPassword = "06c13f0db1e5f86ef5adc575da5a1d89528a0fdd93a568abdbe93145dc7e5d28";

        //=== StorageLayer ===\\
        public const string FolderBdd = "StorageLayer";
        public const string ApiContentUrl = $"https://api.github.com/repos/{RepoOwner}/{RepoName}/contents/{FolderBdd}";
    }
}
