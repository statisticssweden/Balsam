
namespace Balsam.Model
{
    public class HubPaths
    {
        public string BasePath { get; }

        public HubPaths(string basePath)
        {
            BasePath = basePath;
        }

        public string GetTemplatesPath()
        {
            return Path.Combine(BasePath, "templates");
        }

        public string GetHubPath()
        {
            return Path.Combine(BasePath, "hub");
        }

        public string GetProjectPath(string projectId)
        {
            return Path.Combine(GetHubPath(), projectId);
        }

        public string GetBranchPath(string projectId, string branchId)
        {
            return Path.Combine(GetProjectPath(projectId), branchId);
        }
        public string GetUserPath(string projectId, string branchId, string userName)
        {
            return Path.Combine(GetBranchPath(projectId, branchId), userName);
        }

        public string GetWorkspacePath(string projectId, string branchId, string userName, string workspaceId)
        {
            return Path.Combine(GetUserPath(projectId, branchId, userName), workspaceId);
        }

        public string GetWorkspaceTemplatesPath(string templateId)
        {
            return Path.Combine(GetWorkspacesTemplatesPath(), templateId);
        }

        public string GetProjectsTemplatesPath()
        {
            return Path.Combine(GetWorkspacesTemplatesPath(), "projects");
        }

        public string GetWorkspacesTemplatesPath()
        {
            return Path.Combine(GetTemplatesPath(), "workspaces");
        }

        public string GetKnowledgeLibrariesPath()
        {
            return Path.Combine(GetHubPath(), "kb");
        }

        public string GetKnowledgeLibraryFilePath(string knowledgeLibraryFileId)
        {
            return Path.Combine(GetKnowledgeLibrariesPath(), $"{knowledgeLibraryFileId}.json");
        }
    }
}
