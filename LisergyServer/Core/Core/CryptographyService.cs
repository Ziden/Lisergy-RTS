using Game.Engine.DataTypes;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace BaseServer.Core
{
    /// <summary>
    /// Simple cryptography to validate server-to-server communication and user authenticity
    /// TODO: Reuse byte buffers for performance increase
    /// </summary>
    public class CryptographyService : IDisposable
    {
        private Aes Encryptor;
        public TimeSpan TokenDuration { get; private set; } = TimeSpan.FromMinutes(1);

        public CryptographyService(string key)
        {
            Encryptor = Aes.Create();
            var pdb = new Rfc2898DeriveBytes(key, new byte[]
                {
                0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
                });

            Encryptor.Key = pdb.GetBytes(32);
            Encryptor.IV = pdb.GetBytes(16);
        }

        /// <summary>
        /// Generates a token for the user that's valid for a short time
        /// This token can be used on other servers to validate the user is authenticated
        /// so he can stablish a connection
        /// </summary>
        public string GenerateToken(GameId playerId)
        {
            var expires = (DateTime.UtcNow + TokenDuration).Ticks;
            var originalToken = $"{playerId}:{expires}";
            return Encrypt(originalToken);
        }

        /// <summary>
        /// Checks if a given token is valid
        /// </summary>
        public bool IsTokenValid(GameId player, string token)
        {
            var originalToken = Decrypt(token).Split(":");
            if (originalToken.Length != 2) return false;
            if (originalToken[0] != player.ToString()) return false;
            var ticks = Int64.Parse(originalToken[1]);
            var expires = new DateTime(ticks);
            if (DateTime.UtcNow > expires) return false;
            return true;
        }

        /// <summary>
        /// Encrypts the given string
        /// </summary>
        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            var clearBytes = Encoding.Unicode.GetBytes(plainText);
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, Encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(clearBytes, 0, clearBytes.Length);
                }
                plainText = Convert.ToBase64String(ms.ToArray());
            }
            return plainText;
        }

        /// <summary>
        /// Decrypts the given string
        /// </summary>
        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            cipherText = cipherText.Replace(" ", "+");
            var cipherBytes = Convert.FromBase64String(cipherText);
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, Encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(cipherBytes, 0, cipherBytes.Length);
                }
                cipherText = Encoding.Unicode.GetString(ms.ToArray());
            }
            return cipherText;
        }

        public void Dispose()
        {
            Encryptor.Dispose();
        }
    }

}
