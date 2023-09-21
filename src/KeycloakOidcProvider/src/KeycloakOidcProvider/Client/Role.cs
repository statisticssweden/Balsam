using System.Text.RegularExpressions;

namespace Keycloak.OicdProvider.Client;

public class Role
{
    private const string NamePrefix = "balsam-role_";

    public Role(string preferredName)
    {
        PreferredName = preferredName;
        Name = SanitizeRoleName(preferredName);
    }

    private static string SanitizeRoleName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            name = string.Empty;
        }
        name = name.ToLower();
        name = name.PadLeft(3, '0');
        name = Regex.Replace(name, "[^a-z0-9]", "-"); // make sure that only a-z or digit or hyphen replaces all other to hyphen 
        //name = name.StartsWith('-') || name.StartsWith("xn--") ? "x" + name : name; //make sure it starts with a character or a number and not xn--

        //if (name.Length <= 63) return name; // Make sure that the name is not longer than 63 characters
        
        //System.IO.Hashing.Crc32 crc32 = new System.IO.Hashing.Crc32();

        //crc32.Append(System.Text.Encoding.ASCII.GetBytes(name));
        //var hash = crc32.GetCurrentHash();
        //var crcHash = string.Join("", hash.Select(b => b.ToString("x2").ToLower()).Reverse());
        //name = name.Substring(0, 63 - crcHash.Length) + crcHash;

        return $"{NamePrefix}{name}";
    }

    public string PreferredName { get; }

    public string Name { get; }

}

