
using Balsam.Model;

namespace Balsam.Interfaces
{
    public interface IWorkspaceService
    {
        Task<BalsamWorkspace?> CreateWorkspace(string projectId, string branchId, string name, string templateId, string userName, string userMail);
        Task DeleteWorkspace(string projectId, string branchId, string workspaceId, string userName);
        Task<BalsamWorkspace?> GetWorkspace(string projectId, string userId, string workspaceId);
        Task<List<BalsamWorkspace>> GetWorkspaces();
        Task<List<BalsamWorkspace>> GetWorkspacesByProject(string projectId);
        Task<List<BalsamWorkspace>> GetWorkspacesByProjectAndBranch(string projectId, string branchId);
        Task<List<BalsamWorkspace>> GetWorkspacesByProjectAndUser(string projectId, string userId);
        Task<List<BalsamWorkspace>> GetWorkspacesByProjectBranchAndUser(string projectId, string branchId, string userId);
        Task<List<BalsamWorkspace>> GetWorkspacesByUser(string userId, List<string> projectIds);
        Task<List<WorkspaceTemplate>> ListWorkspaceTemplates();
    }
}