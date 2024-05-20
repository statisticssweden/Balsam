namespace Balsam.Model
{
    public class UserInfo
    {
        public string UserName { get; set; }
        public string Mail { get; set; }
        public string GitPAT { get; set; }

        public S3Token S3 { get; set; }

        public UserInfo(string userName, string mail, string gitPAT)
        {
            UserName = userName;
            Mail = mail;
            GitPAT = gitPAT;
        }
    }
}
