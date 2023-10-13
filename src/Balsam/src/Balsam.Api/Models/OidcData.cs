namespace Balsam.Api.Models
{
    public class OidcData
    {
        public string GroupId { get; set; }
        public string GroupName { get; set; }

        public OidcData(string groupId, string groupName)
        {
            GroupId = groupId;
            GroupName = groupName;
        }
    }
}
