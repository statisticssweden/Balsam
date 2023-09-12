namespace GitLabProvider.Configuration
{
    public class ApiOptions
    {
        public const string SectionName = "Api";

        public string BaseUrl { get; set; }
        public string PAT { get; set; }
        public string GroupID { get; set; }
        public string TemplatePath { get; set; }



    }
}
