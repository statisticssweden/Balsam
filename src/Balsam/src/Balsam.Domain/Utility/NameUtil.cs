using System.Text.RegularExpressions;

namespace Balsam.Utility
{
    public static class NameUtil
    {
        public static string SanitizeName(string name)
        {
            //TODO: Combine with GenerateWorkspaceId
            var crc32 = new System.IO.Hashing.Crc32();

            crc32.Append(System.Text.Encoding.ASCII.GetBytes(name));
            var hash = crc32.GetCurrentHash();
            var crcHash = string.Join("", hash.Select(b => b.ToString("x2").ToLower()).Reverse());

            name = name.ToLower(); //Only lower charachters allowed
            name = name.Replace(" ", "-"); //replaces spaches with hypen
            name = Regex.Replace(name, @"[^a-z0-9\-]", ""); // make sure that only a-z or digit or hypen removes all other characters
            name = name.Substring(0, Math.Min(50 - crcHash.Length, name.Length)) + "-" + crcHash; //Assures max size of 50 characters

            return name;

        }
    }
}
