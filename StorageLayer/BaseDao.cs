using LogicLayer.Outils;
using Newtonsoft.Json.Linq;
using System.Data.SQLite;
using System.Net;

namespace StorageLayer
{
    public class BaseDao
    {
        private SQLiteConnection connection;
        private readonly string githubContentsUrl = Constants.ApiContentUrl;
        public event EventHandler<double> ProgressChanged;

        /// <summary>
        /// Constructor
        /// </summary>
        public BaseDao()
        {
            // chemin absolu vers la base de données 
            string localDbFolderPath = Environment.CurrentDirectory;
            string[] localDbFiles = Directory.GetFiles(localDbFolderPath, "PR_BDD_*.db");
            var path = localDbFiles.OrderByDescending(f => f).FirstOrDefault();
            if (path != null)
            {
                // chaîne de connexion
                var connectionString = "Data Source=" + path + ";Version=3;";
                // création de la connexion
                connection = new SQLiteConnection(connectionString);
            }
            else
            {
                connection = new SQLiteConnection("Data Source=:memory:;Version=3;");
            }
        }

        protected virtual void OnProgressChanged(double progress)
        {
            ProgressChanged?.Invoke(this, progress);
        }

        #region Mise à jour
        public async Task<bool> CheckIfUpdated()
        {
            try
            {
                // Récupère la version du fichier local et ses dates
                (string version, DateTime localCreateDate, DateTime localModifiedDate) localFile = GetLocalDatabaseVersion();

                WebClient client = new WebClient();
                client.Headers.Add("User-Agent", "request");
                client.DownloadProgressChanged += (sender, e) => OnProgressChanged(e.ProgressPercentage);
                string githubContentsJson = await client.DownloadStringTaskAsync(githubContentsUrl);
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
                            Version localVersion = new Version(localFile.version);

                            OnProgressChanged(githubContentsArray.IndexOf(content) * 100 / githubContentsArray.Count);

                            // Compare les versions
                            if (remoteVersion > localVersion)
                            {
                                Settings.Default.LatestUpdate = DateTime.Now; // Met à jour la date de dernière mise à jour
                                Settings.Default.Save();
                                return true; // La base de données est mise à jour, donc retourne true
                            }
                        }
                    }
                }

                // Compare la date de dernière mise à jour avec la date de modification locale du fichier de BDD
                DateTime localDate = localFile.localModifiedDate.AddMinutes(-1); // Ajoute 5 minutes pour compenser le décalage horaire
                if (localDate > Settings.Default.LatestUpdate)
                {
                    Settings.Default.LatestUpdate = DateTime.Now; // Met à jour la date de dernière mise à jour
                    Settings.Default.Save();
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
                    WebClient client = new WebClient();
                    client.Headers.Add("User-Agent", "request");
                    client.DownloadProgressChanged += (sender, e) => OnProgressChanged(e.ProgressPercentage);
                    string githubFilesJson = await client.DownloadStringTaskAsync(githubContentsUrl);
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
                            client.DownloadProgressChanged += (sender, e) => OnProgressChanged(e.ProgressPercentage);
                            await client.DownloadFileTaskAsync(fileUrl, Environment.CurrentDirectory + "\\" + fileNameWithExtension);

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
        #endregion

        #region CRUD
        /// <summary>
        /// Open the connection
        /// </summary>
        /// <param name="req">SQLiteCommand</param>
        public void ExecuteNonQuery(string req)
        {
            try
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = req;
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception)
            {
                throw new Exception("Erreur avec la base de données :\n" + req);
            }
        }

        /// <summary>
        /// Execute a query
        /// </summary>
        /// <param name="req">SQLiteCommand</param>
        /// <returns>SQLiteDataReader</returns>
        public SQLiteDataReader ExecuteReader(string req)
        {
            try
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = req;
                var reader = command.ExecuteReader();
                return reader;
            }
            catch (Exception)
            {
                throw new Exception("Erreur avec la base de données :\n" + req);
            }
        }

        /// <summary>
        /// Close the connection
        /// </summary>
        public void CloseConnection()
        {
            connection.Close();
        }

        public List<string> GetTables()
        {
            List<string> tables = new List<string>();
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%' ORDER BY name";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                tables.Add(reader.GetString(0));
            }
            connection.Close();
            return tables;
        }

        public List<(string ColumnName, Type ColumnType)> GetColumnsOfTable(string tableName)
        {
            List<(string ColumnName, Type ColumnType)> columns = new List<(string ColumnName, Type ColumnType)>();
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = $"PRAGMA table_info({tableName})";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                string columnName = reader.GetString(1);
                string columnType = reader.GetString(2);
                columns.Add((columnName, MapSQLiteTypeToCSharp(columnType)));
            }
            connection.Close();
            return columns;
        }

        private Type MapSQLiteTypeToCSharp(string sqliteType)
        {
            switch (sqliteType.ToUpper())
            {
                case "INTEGER":
                    return typeof(int);
                case "REAL":
                    return typeof(double);
                case "TEXT":
                    return typeof(string);
                case "BLOB":
                    return typeof(byte[]);
                case "NUMERIC":
                    return typeof(decimal);
                default:
                    return typeof(string);
            }
        }
        #endregion

        #region Initialisation
        private void CreateDatabase()
        {
            if (connection == null)
            {
                SQLiteConnection.CreateFile("PR_BDD.db");
                connection = new SQLiteConnection("Data Source=PR_BDD.db;Version=3;");

                List<string> createTableQueries = new List<string>()
                {
                    @"CREATE TABLE ""Grades"" (""Id"" INTEGER NOT NULL, ""Type"" TEXT NOT NULL, ""Nom"" TEXT NOT NULL, PRIMARY KEY(""Id"" AUTOINCREMENT));",
                    @"CREATE TABLE ""Specialisations"" (""Id"" INTEGER NOT NULL, ""Type"" TEXT NOT NULL, ""Nom"" TEXT NOT NULL, PRIMARY KEY(""Id"" AUTOINCREMENT));",
                    @"CREATE TABLE ""Infractions"" (""Id"" INTEGER NOT NULL, ""Type"" TEXT NOT NULL, ""Nom"" TEXT NOT NULL, PRIMARY KEY(""Id"" AUTOINCREMENT));",
                    @"CREATE TABLE ""Roles"" (""Id"" INTEGER NOT NULL, ""Type"" TEXT NOT NULL, ""Nom"" TEXT NOT NULL, PRIMARY KEY(""Id"" AUTOINCREMENT));",
                    @"CREATE TABLE ""Actions"" (""Id"" INTEGER NOT NULL, ""Nom"" TEXT NOT NULL, ""ActInfraction"" INTEGER NOT NULL, FOREIGN KEY(""ActInfraction"") REFERENCES ""Infractions""(""Id""), PRIMARY KEY(""Id"" AUTOINCREMENT));",
                    @"CREATE TABLE ""Effectifs"" (""Id"" INTEGER NOT NULL, ""IdDiscord"" TEXT NOT NULL, ""Nom"" TEXT NOT NULL, ""Prenom"" TEXT NOT NULL, ""EffGrade"" INTEGER NOT NULL, FOREIGN KEY(""EffGrade"") REFERENCES ""Grades""(""Id""), PRIMARY KEY(""Id"" AUTOINCREMENT));",
                    @"CREATE TABLE ""Unites"" (""Id"" INTEGER NOT NULL, ""Type"" TEXT NOT NULL, ""Nom"" TEXT NOT NULL, ""UnitSpecialisation"" INTEGER NOT NULL, FOREIGN KEY(""UnitSpecialisation"") REFERENCES ""Specialisations""(""Id""), PRIMARY KEY(""Id"" AUTOINCREMENT));",
                    @"CREATE TABLE ""Vehicules"" (""Id"" INTEGER NOT NULL, ""VehSpecialisation"" INTEGER NOT NULL, ""Nom"" TEXT NOT NULL, FOREIGN KEY(""VehSpecialisation"") REFERENCES ""Specialisations""(""Id""), PRIMARY KEY(""Id"" AUTOINCREMENT));",
                    @"CREATE TABLE ""Utilisateurs"" (""Id"" INTEGER NOT NULL, ""Username"" TEXT NOT NULL, ""Password"" TEXT NOT NULL, ""Role"" INTEGER NOT NULL, FOREIGN KEY(""Role"") REFERENCES ""Roles""(""Id""), PRIMARY KEY(""Id"" AUTOINCREMENT));"
                };

                foreach (string createQuerie in createTableQueries)
                    ExecuteNonQuery(createQuerie);
            }
        }

        private void InsertDatabase()
        {
            List<string> insertTableQueries = new List<string>()
            {
                @"INSERT INTO ""Grades"" (""Id"",""Type"",""Nom"") VALUES (1,'CommissaireGeneral','Commissaire Général');",
                @"INSERT INTO ""Grades"" (""Id"",""Type"",""Nom"") VALUES (2,'CommissaireDivisionnaire','Commissaire Divisionnaire');",
                @"INSERT INTO ""Grades"" (""Id"",""Type"",""Nom"") VALUES (3,'Commissaire','Commissaire');",
                @"INSERT INTO ""Grades"" (""Id"",""Type"",""Nom"") VALUES (4,'CommandantDivisionnaire','Commandant Divisionnaire');",
                @"INSERT INTO ""Grades"" (""Id"",""Type"",""Nom"") VALUES (5,'Commandant','Commandant');",
                @"INSERT INTO ""Grades"" (""Id"",""Type"",""Nom"") VALUES (6,'Capitaine','Capitaine');",
                @"INSERT INTO ""Grades"" (""Id"",""Type"",""Nom"") VALUES (7,'Lieutenant','Lieutenant');",
                @"INSERT INTO ""Grades"" (""Id"",""Type"",""Nom"") VALUES (8,'LieutenantStagiaire','Lieutenant Stagiaire');",
                @"INSERT INTO ""Grades"" (""Id"",""Type"",""Nom"") VALUES (9,'MajorRULP','Major RULP');",
                @"INSERT INTO ""Grades"" (""Id"",""Type"",""Nom"") VALUES (10,'MajorEEX','Major à Echelon Exceptionnel');",
                @"INSERT INTO ""Grades"" (""Id"",""Type"",""Nom"") VALUES (11,'Major','Major');",
                @"INSERT INTO ""Grades"" (""Id"",""Type"",""Nom"") VALUES (12,'BrigadierChef','Brigadier Chef');",
                @"INSERT INTO ""Grades"" (""Id"",""Type"",""Nom"") VALUES (14,'GardienDeLaPaix','Gardien de la Paix');",
                @"INSERT INTO ""Grades"" (""Id"",""Type"",""Nom"") VALUES (15,'GardienDeLaPaixStagiaire','Gardien de la Paix Stagiaire');",
                @"INSERT INTO ""Grades"" (""Id"",""Type"",""Nom"") VALUES (16,'PolicierAdj','Policier Adjoint');",
                @"INSERT INTO ""Specialisations"" (""Id"",""Type"",""Nom"") VALUES (1,'PS','Police Secours');",
                @"INSERT INTO ""Specialisations"" (""Id"",""Type"",""Nom"") VALUES (2,'CRS','Compagnie Républicaine de Sécurité');",
                @"INSERT INTO ""Specialisations"" (""Id"",""Type"",""Nom"") VALUES (3,'SDSS','Sous-Direction des Services Spécialisés');",
                @"INSERT INTO ""Specialisations"" (""Id"",""Type"",""Nom"") VALUES (4,'DRPJ','Direction Régionale de la Police Judiciaire');",
                @"INSERT INTO ""Specialisations"" (""Id"",""Type"",""Nom"") VALUES (5,'OPJ','Officier de Police Judiciaire');",
                @"INSERT INTO ""Specialisations"" (""Id"",""Type"",""Nom"") VALUES (6,'BRI','Brigade de Recherche et d''Intervention');",
                @"INSERT INTO ""Infractions"" (""Id"",""Type"",""Nom"") VALUES (1,'Vitesse','Vitesse');",
                @"INSERT INTO ""Infractions"" (""Id"",""Type"",""Nom"") VALUES (2,'Stationnement','Stationnement');",
                @"INSERT INTO ""Infractions"" (""Id"",""Type"",""Nom"") VALUES (3,'Autre','Autre');",
                @"INSERT INTO ""Infractions"" (""Id"",""Type"",""Nom"") VALUES (4,'Circulation','Circulation');",
                @"INSERT INTO ""Infractions"" (""Id"",""Type"",""Nom"") VALUES (5,'Criminalite','Criminalité');",
                @"INSERT INTO ""Infractions"" (""Id"",""Type"",""Nom"") VALUES (6,'Drogue','Drogue');",
                @"INSERT INTO ""Infractions"" (""Id"",""Type"",""Nom"") VALUES (7,'Violence','Violence');",
                @"INSERT INTO ""Roles"" (""Id"",""Type"",""Nom"") VALUES (1,'Admin','Admin');",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (13,'Circulation avec des plaques sales, illisibles ou non conformes',4);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (14,'Circulation dans une voie de bus',4);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (15,'Circulation d’un véhicule en marche normale à une vitesse anormalement réduite',4);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (16,'Circulation sur la voie du milieu ou sur la gauche sur l’autoroute',4);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (17,'Circuler à moins de 80 km/h sur la voie de gauche sur l’autoroute',4);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (18,'Conduite sans le signe A pour un conducteur novice',4);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (19,'Contrôle technique hors délai',3);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (20,'Défaut d’assurance',3);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (21,'Défaut de carte grise',3);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (22,'Défaut de gilet et de triangle',3);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (23,'Fumer au volant',3);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (24,'Infractions liées au stationnement payant (non-paiement ou temps dépassé)',2);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (25,'Non changement de propriétaire sur la carte grise',3);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (26,'Non changement d’adresse de la carte grise',3);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (27,'Non désignation d’un conducteur auteur d’une infraction',3);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (28,'Non-port du casque à vélo pour les enfants de moins de 12 ans',4);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (29,'Non-port de la ceinture par un passager majeur',4);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (30,'Non présentation de la carte grise',3);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (31,'Non présentation de l’attestation assurance',3);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (32,'Non réalisation d''un stage de sensibilisation à la sécurité routière obligatoire en permis probatoire',4);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (33,'Non-respect du feu orange',4);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (34,'Pneus usés',4);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (35,'Refus d’acquittement du péage',4);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (36,'Stationnement gênant ou abusif ou sur une place « handicapé »',2);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (37,'Stationnement et l’arrêt sur la bande d’arrêt d’urgence',2);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (38,'Transport d''un enfant sans dispositif de retenue adapté à son âge',4);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (39,'Usage abusif des feux de route',4);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (40,'Usage abusif du klaxon',4);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (41,'Vitesse excessive eu égard aux circonstances',4);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (42,'Chevauchement de la ligne continue',4);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (43,'Excès de vitesse de 20 km/h hors agglomération',1);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (44,'Franchissement ou chevauchement d’une ligne délimitant une bande d’arrêt d’urgence',4);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (45,'Non-respect du port des gants à moto',4);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (46,'Accélération lors d’un dépassement',4);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (47,'Circulation ou stationnement sur le terre-plein central de l’autoroute',4);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (48,'Excès de vitesse compris entre 20 et 29 km/h',1);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (49,'Arrêt ou stationnement dangereux',2);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (50,'Changement de direction sans clignotant',4);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (51,'Circulation sans motif sur la partie gauche de la chaussée',4);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (52,'Circulation sur la bande d’arrêt d’urgence',4);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (53,'Conduire un véhicule sans respecter les conditions de validité ou les restrictions d’usage du permis de conduire',4);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (54,'Dépassement dangereux',4);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (55,'Dépassement par la droite',4);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (56,'Excès de vitesse compris entre 30 et 39 km/h',1);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (57,'Franchissement de la ligne continue',4);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (58,'Non-port de la ceinture de sécurité',4);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (59,'Non-port du casque',4);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (60,'Non-respect des distances de sécurité',4);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (61,'Port d’oreillettes, d’écouteurs et kits mains-libres pendant la conduite',4);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (62,'Présence dans le champ de vision du conducteur d’un écran',4);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (63,'Stationnement sur la chaussée la nuit ou par temps de brouillard, en un lieu dépourvu d’éclairage public, d’un véhicule sans éclairage ni signalisation',2);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (64,'Téléphoner au volant',4);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (65,'Vitres teintées avec une teinte de moins de 30%',4);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (66,'Circulation de nuit ou par temps de brouillard en un lieu dépourvu d’éclairage public, d’un véhicule sans éclairage ni signalisation',4);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (67,'Circulation en sens interdit',4);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (68,'Excès de vitesse compris entre 40 et 49 km/h',1);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (69,'Marche arrière ou demi-tour sur autoroute',4);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (70,'Non-respect de l’arrêt au feu',4);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (71,'Non-respect de l’arrêt au stop',4);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (72,'Non-respect de priorité d’un véhicule prioritaire',4);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (73,'Non-respect du cédez-le-passage',4);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (74,'Refus de priorité',4);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (75,'Franchissement d’un passage à niveau',4);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (76,'Circulation sur une barrière de dégel',4);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (77,'Conduite après usage de stupéfiants',6);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (78,'Conduite en état alcoolique (taux d’alcoolémie supérieur ou égal à 0,5g/l et inférieur à 0,8g/l)',6);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (79,'Conduite en état d’ivresse manifeste',6);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (80,'Conduite malgré un retrait de permis (rétention, suspension, annulation, invalidation du permis de conduire)',6);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (81,'Délit de fuite',7);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (82,'Excès de vitesse supérieur à 50 km/h par rapport à la vitesse maximale autorisée',1);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (83,'Gêne ou entrave à la circulation',4);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (84,'Homicide ou blessures involontaires entraînant une incapacité totale de travail',7);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (85,'Non-respect du cédez-le-passage à un piéton sur un passage clouté',4);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (86,'Refus d’obtempérer',4);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (87,'Refus de se soumettre à un test de dépistage de stupéfiants',6);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (88,'Refus de se soumettre à une vérification de présence d’alcool dans le sang',6);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (89,'Usage volontaire de fausses plaques d’immatriculation, défaut volontaire de plaques et fausses déclarations',5);",
                @"INSERT INTO ""Actions"" (""Id"",""Nom"",""ActInfraction"") VALUES (90,'Utilisation d''un détecteur de radar, d''un avertisseur ou d''un système antiradar',4);",
                @"INSERT INTO ""Unites"" (""Id"",""Type"",""Nom"",""UnitSpecialisation"") VALUES (1,'TN750','TN 750',1);",
                @"INSERT INTO ""Unites"" (""Id"",""Type"",""Nom"",""UnitSpecialisation"") VALUES (2,'PS120A','PS 120 A',1);",
                @"INSERT INTO ""Unites"" (""Id"",""Type"",""Nom"",""UnitSpecialisation"") VALUES (3,'PS120B','PS 120 B',1);",
                @"INSERT INTO ""Unites"" (""Id"",""Type"",""Nom"",""UnitSpecialisation"") VALUES (4,'PS120C','PS 120 C',1);",
                @"INSERT INTO ""Unites"" (""Id"",""Type"",""Nom"",""UnitSpecialisation"") VALUES (5,'TM120A','TM 120 A',1);",
                @"INSERT INTO ""Unites"" (""Id"",""Type"",""Nom"",""UnitSpecialisation"") VALUES (6,'TM120B','TM 120 B',1);",
                @"INSERT INTO ""Unites"" (""Id"",""Type"",""Nom"",""UnitSpecialisation"") VALUES (7,'MIKE1','MIKE 1',2);",
                @"INSERT INTO ""Unites"" (""Id"",""Type"",""Nom"",""UnitSpecialisation"") VALUES (8,'MIKE2','MIKE 2',2);",
                @"INSERT INTO ""Unites"" (""Id"",""Type"",""Nom"",""UnitSpecialisation"") VALUES (9,'MIKE3','MIKE 3',2);",
                @"INSERT INTO ""Unites"" (""Id"",""Type"",""Nom"",""UnitSpecialisation"") VALUES (10,'MIKE4','MIKE 4',2);",
                @"INSERT INTO ""Unites"" (""Id"",""Type"",""Nom"",""UnitSpecialisation"") VALUES (11,'PEGASE1','PEGASE 1',2);",
                @"INSERT INTO ""Unites"" (""Id"",""Type"",""Nom"",""UnitSpecialisation"") VALUES (12,'PEGASE2','PEGASE 2',2);",
                @"INSERT INTO ""Unites"" (""Id"",""Type"",""Nom"",""UnitSpecialisation"") VALUES (13,'A52A1','52 A1',3);",
                @"INSERT INTO ""Unites"" (""Id"",""Type"",""Nom"",""UnitSpecialisation"") VALUES (14,'A52A11','52 A11',3);",
                @"INSERT INTO ""Unites"" (""Id"",""Type"",""Nom"",""UnitSpecialisation"") VALUES (15,'A52D2','52 D2',3);",
                @"INSERT INTO ""Unites"" (""Id"",""Type"",""Nom"",""UnitSpecialisation"") VALUES (16,'A52D21','52 D21',3);",
                @"INSERT INTO ""Unites"" (""Id"",""Type"",""Nom"",""UnitSpecialisation"") VALUES (17,'TM51A','TM 51 A',3);",
                @"INSERT INTO ""Unites"" (""Id"",""Type"",""Nom"",""UnitSpecialisation"") VALUES (18,'TM51B','TM 51 B',3);",
                @"INSERT INTO ""Unites"" (""Id"",""Type"",""Nom"",""UnitSpecialisation"") VALUES (19,'MONFORT40','MONFORT 40',2);",
                @"INSERT INTO ""Unites"" (""Id"",""Type"",""Nom"",""UnitSpecialisation"") VALUES (20,'MONFORT41','MONFORT 41',2);",
                @"INSERT INTO ""Unites"" (""Id"",""Type"",""Nom"",""UnitSpecialisation"") VALUES (21,'MONFORT42','MONFORT 42',2);",
                @"INSERT INTO ""Unites"" (""Id"",""Type"",""Nom"",""UnitSpecialisation"") VALUES (22,'MONFORT70','MONFORT 70',2);",
                @"INSERT INTO ""Unites"" (""Id"",""Type"",""Nom"",""UnitSpecialisation"") VALUES (23,'MONFORT71','MONFORT 71',2);",
                @"INSERT INTO ""Unites"" (""Id"",""Type"",""Nom"",""UnitSpecialisation"") VALUES (24,'MONFORT72','MONFORT 72',2);",
                @"INSERT INTO ""Unites"" (""Id"",""Type"",""Nom"",""UnitSpecialisation"") VALUES (25,'MONFORT80','MONFORT 80',2);",
                @"INSERT INTO ""Unites"" (""Id"",""Type"",""Nom"",""UnitSpecialisation"") VALUES (26,'MONFORT81','MONFORT 81',2);",
                @"INSERT INTO ""Unites"" (""Id"",""Type"",""Nom"",""UnitSpecialisation"") VALUES (27,'MONFORT82','MONFORT 82',2);",
                @"INSERT INTO ""Unites"" (""Id"",""Type"",""Nom"",""UnitSpecialisation"") VALUES (28,'MONFORT85','MONFORT 85',2);",
                @"INSERT INTO ""Unites"" (""Id"",""Type"",""Nom"",""UnitSpecialisation"") VALUES (29,'DRPJ','DRPJ',4);",
                @"INSERT INTO ""Unites"" (""Id"",""Type"",""Nom"",""UnitSpecialisation"") VALUES (30,'PJUNITE','PJ Unite',5);",
                @"INSERT INTO ""Unites"" (""Id"",""Type"",""Nom"",""UnitSpecialisation"") VALUES (31,'PJALPHA','PJ Alpha',5);",
                @"INSERT INTO ""Unites"" (""Id"",""Type"",""Nom"",""UnitSpecialisation"") VALUES (32,'PJBRAVO','PJ Bravo',5);",
                @"INSERT INTO ""Unites"" (""Id"",""Type"",""Nom"",""UnitSpecialisation"") VALUES (33,'TOPAZ1','TOPAZ 1',6);",
                @"INSERT INTO ""Unites"" (""Id"",""Type"",""Nom"",""UnitSpecialisation"") VALUES (34,'TOPAZ2','TOPAZ 2',6);",
                @"INSERT INTO ""Unites"" (""Id"",""Type"",""Nom"",""UnitSpecialisation"") VALUES (35,'TOPAZ3','TOPAZ 3',6);",
                @"INSERT INTO ""Unites"" (""Id"",""Type"",""Nom"",""UnitSpecialisation"") VALUES (36,'TOPAZ4','TOPAZ 4',6);",
                @"INSERT INTO ""Unites"" (""Id"",""Type"",""Nom"",""UnitSpecialisation"") VALUES (37,'TOPAZ5','TOPAZ 5',6);",
                @"INSERT INTO ""Unites"" (""Id"",""Type"",""Nom"",""UnitSpecialisation"") VALUES (38,'TOPAZ6','TOPAZ 6',6);",
                @"INSERT INTO ""Unites"" (""Id"",""Type"",""Nom"",""UnitSpecialisation"") VALUES (39,'TOPAZ7','TOPAZ 7',6);",
                @"INSERT INTO ""Unites"" (""Id"",""Type"",""Nom"",""UnitSpecialisation"") VALUES (40,'TOPAZ8','TOPAZ 8',6);",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (1,3,'Yamaha XTZ 1200 Ténéré banalisée');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (2,1,'Yamaha XTZ 1200 Ténéré');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (3,3,'Yamaha XTZ 1200 Ténéré');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (4,2,'Yamaha Tracer 900 CRS');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (5,1,'Yamaha Tracer 900');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (6,3,'Yamaha Tracer 900');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (7,3,'Yamaha MT09 banalisée');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (8,3,'Yamaha FJR 1300 banalisée');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (9,2,'Yamaha FJR 1300 CRS');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (10,1,'Yamaha FJR 1300');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (11,3,'Yamaha FJR 1300');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (12,6,'Volkswagen Transporter T6');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (13,3,'Volkswagen Sharan II basse visibilité');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (14,3,'Volkswagen Sharan II banalisé');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (15,1,'Volkswagen Sharan II Préfecture de Police');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (16,3,'Volkswagen Passat B8 basse visibilité');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (17,3,'Volkswagen Passat B8 banalisée');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (18,3,'Volkswagen Passat B8 Break basse visibilité');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (19,3,'Volkswagen Passat B8 Break banalisée');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (20,1,'Volkswagen Passat B8 Break Préfecture de Police');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (21,1,'Volkswagen ID3 Préfecture de Police');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (22,6,'Volkswagen Amarok');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (23,3,'Skoda Octavia III basse visibilité');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (24,3,'Skoda Octavia III Combi basse visibilité');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (25,3,'Skoda Octavia III Combi banalisée');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (26,1,'Skoda Octavia III Combi');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (27,1,'Skoda Octavia III');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (28,3,'Seat Leon IV FR banalisée');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (29,2,'Renault Trafic III PMV CRS');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (30,2,'Renault Trafic III CRS');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (31,1,'Renault Trafic III');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (32,3,'Renault Talisman Estate basse visibilité');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (33,3,'Renault Talisman Estate banalisée');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (34,1,'Renault Senic IV');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (35,2,'Renault Master III PC CRS');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (36,2,'Renault Master III CRS');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (37,1,'Renault Master II');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (38,2,'Renault K380 lanceur d''eau');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (39,2,'Peugeot Rifter CRS');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (40,1,'Peugeot Rifter');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (41,2,'Peugeot Expert III PMV CRS');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (42,1,'Peugeot Expert III PMV');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (43,3,'Peugeot 508 II basse visibilité');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (44,3,'Peugeot 508 I basse visibilité');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (45,3,'Peugeot 508 I banalisée');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (46,3,'Peugeot 508 I SW basse visibilité');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (47,3,'Peugeot 5008 II banalisé');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (48,1,'Peugeot 5008 II');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (49,3,'Peugeot 308 III banalisée');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (50,3,'Peugeot 3008 II banalisé');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (51,1,'Peugeot 3008 II');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (52,6,'Panhard PVP');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (53,6,'Mercedes Sprinter III DOPC');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (54,3,'Megane IV banalisée');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (55,3,'Megane IV Estate banalisée');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (56,2,'Megane IV Estate CRS');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (57,1,'Megane IV Estate');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (58,1,'Megane III Estate');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (59,3,'Ford Mondeo IV Break basse visibilité');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (60,3,'Ford Galaxy basse visibilité');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (61,1,'Ford Focus III Break');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (62,2,'Fiat Ducato III CRS');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (63,1,'Dacia Duster (nouvelle sérigraphie)');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (64,1,'Dacia Duster (ancienne sérigraphie)');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (65,3,'DS7 banalisé');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (66,2,'BMW 1250 RT CRS 1');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (67,2,'BMW 1250 RT CRS');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (68,1,'BMW 1250 RT');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (69,3,'BMW 1250 RT');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (70,6,'Audi RS3');",
                @"INSERT INTO ""Vehicules"" (""Id"",""VehSpecialisation"",""Nom"") VALUES (71,6,'Audi A3 Berline');",
                @"INSERT INTO ""Utilisateurs"" (""Id"",""Username"",""Password"",""Role"") VALUES (1,'admin','8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918',1);"
            };

            foreach (string insertTable in insertTableQueries)
                ExecuteNonQuery(insertTable);
        }
        #endregion
    }
}
