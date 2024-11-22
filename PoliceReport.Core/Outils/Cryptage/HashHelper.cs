using System.Security.Cryptography;
using System.Text;

namespace PoliceReport.Core.Outils.Cryptage
{
    public static class HashHelper
    {
        public static string CalculateSHA256(string input)
        {
            using SHA256 sha256 = SHA256.Create();
            // Convertir le mot de passe en tableau de bytes
            byte[] bytes = Encoding.UTF8.GetBytes(input);

            // Calculer le hash SHA256
            byte[] hashBytes = sha256.ComputeHash(bytes);

            // Convertir le hash en une chaîne hexadécimale
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                builder.Append(hashBytes[i].ToString("x2"));
            }

            return builder.ToString();
        }
    }
}
