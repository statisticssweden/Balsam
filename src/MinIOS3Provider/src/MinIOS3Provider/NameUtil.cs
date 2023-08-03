using System.Text.RegularExpressions;

namespace MinIOS3Provider
{
    public class NameUtil
    {
        //See https://min.io/docs/minio/container/operations/checklists/thresholds.html for naming rules

        /// <summary>
        /// Sanitize bucket names
        /// </summary>
        /// <param name="name">name of the bucket that should be sanitized</param>
        /// <returns>A sanitized version of the name</returns>
        public static string SanitizeBucketName(string name)
        {
            name = name.ToLower(); //Only lower charachters allowed
            name = name.PadLeft(3, '0'); //At least 3 charachters long otherwise padd with zeros in the begining
            name = Regex.Replace(name, "[^a-z0-9]", "-"); // make sure that only a-z or digit or hypen repleces all other to hypen 
            name = name.StartsWith("-") || name.StartsWith("xn--") ? "x" + name : name; //make sure it starts with a character or a number and not xn--

            if (name.Length > 63) // Make sure that the name is not longer than 63 characters
            {
                System.IO.Hashing.Crc32 crc32 = new System.IO.Hashing.Crc32();

                crc32.Append(System.Text.Encoding.ASCII.GetBytes(name));
                var hash = crc32.GetCurrentHash();
                var crcHash = string.Join("", hash.Select(b => b.ToString("x2").ToLower()).Reverse());
                name = name.Substring(0, 63 - crcHash.Length) + crcHash;
            }
            return name;

        }

        /// <summary>
        /// Checks if a object name is valid
        /// </summary>
        /// <param name="name">the object name</param>
        /// <returns>true if the object name is a valid objet name oterwise false</returns>
        public static bool CheckObjectName(string name)
        {
            if (name.Length > 1024) return false;
            foreach (var token in name.Split("/", StringSplitOptions.RemoveEmptyEntries))
            {
                if (token.Length > 255) return false;
            }

            return true;

        }
    }
}
