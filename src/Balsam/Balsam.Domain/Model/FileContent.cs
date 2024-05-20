namespace Balsam.Model
{
    public class FileContent
    {

        public byte[]? Content { get; set; }
        public string Mediatype { get; set; } = string.Empty;

        public FileContent(byte[]? content, string mediatype)
        {
            Content = content;
            Mediatype = mediatype;
        }

        public FileContent()
        {
                
        }
    }
}
