using System.Text.RegularExpressions;

namespace RocketChatChatProvider;

public static class NameUtil
{
    //Channel naming has restraints following the regex filter[0 - 9a - zA - Z - _.]+ by default.
    //This can be modified in the Admin > General > UTF8.Channel names should not allow for any whitespaces.
    // https://developer.rocket.chat/reference/api/rest-api/endpoints/rooms/channels-endpoints/create

    public static string SanitizeName(string name)
    {
        name = name.ToLower();
        name = name.PadLeft(3, '0');
        name = Regex.Replace(name, "[^a-z0-9]", "_");
        return name;
    }
}