namespace LogicLayer.Utilisateur
{
    public class Utilisateur
    {
        #region Attributes
        private int _id;
        private string _username;
        private string _password;
        private int _role;
        #endregion

        #region Properties
        public int Id
        {
            get => _id;
            set
            {
                _id = value;
            }
        }

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
            }
        }

        public int Role
        {
            get => _role;
            set
            {
                _role = value;
            }
        }
        #endregion

        public Utilisateur() { }

        public Utilisateur(string username, string password, int role)
        {
            Username = username;
            Password = password;
            Role = role;
        }

        public Utilisateur(int id, string username, string password, int role)
        {
            Id = id;
            Username = username;
            Password = password;
            Role = role;
        }
    }
}
