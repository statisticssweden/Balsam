using System.Runtime.Serialization;

namespace Balsam.Model
{ 
    //TODO: Rename to RepoFileTreeNode or something
    public class BalsamRepoFile
    {
        public string Id { get; set; }

        public string Path { get; set; }

        public string Name { get; set; }

        public enum TypeEnum
        {
            
            /// <summary>
            /// Enum FileEnum for File
            /// </summary>
            [EnumMember(Value = "File")]
            FileEnum = 1,
            
            /// <summary>
            /// Enum FolderEnum for Folder
            /// </summary>
            [EnumMember(Value = "Folder")]
            FolderEnum = 2
        }

        public TypeEnum Type { get; set; }

        public string? ContentUrl { get; set; }
    }
}
