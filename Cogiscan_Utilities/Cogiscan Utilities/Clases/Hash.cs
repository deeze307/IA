using System;
using System.Security.Cryptography;
using System.Text;

namespace Cogiscan_Utilities.Clases
{
    class Hash
    {
        public static bool getHash(string passwordTyped, string passwordDB2)
        {
            using (MD5 md5hash = MD5.Create())
            {
                string convertedHash = getMD5Hash(md5hash, passwordTyped);
                if (convertedHash == passwordDB2)
                {
                    return true;
                }
                else return false;
            }
        }

        static string getMD5Hash(MD5 md5hash, string password)
        {
            //Convierto la contraseña tipeada a un array de byte y comparo el hash
            byte[] data = md5hash.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder sBuilder = new StringBuilder();
            //Hago un loop en cada byte y formateo cada uno a un string hexadecimal
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }
}
