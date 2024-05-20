
using Balsam.Model;

namespace Balsam.Repositories
{
    public interface IWorkspaceRepository
    {
        Task<BalsamWorkspace?> CreateWorkspace(BalsamWorkspace workspace);
        Task DeleteWorkspace(string projectId, string branchId, string workspaceId, string userName);
        string GenerateWorkspaceId(string name);
        Task<BalsamWorkspace?> GetWorkspace(string projectId, string userId, string workspaceId);
        Task<List<BalsamWorkspace>> GetWorkspaces();
        Task<List<BalsamWorkspace>> GetWorkspacesByProject(string projectId);
        Task<List<BalsamWorkspace>> GetWorkspacesByProjectAndBranch(string projectId, string branchId);
        Task<List<BalsamWorkspace>> GetWorkspacesByProjectAndUser(string projectId, string userId);
        Task<List<BalsamWorkspace>> GetWorkspacesByProjectBranchAndUser(string projectId, string branchId, string userId);
        Task<List<BalsamWorkspace>> GetWorkspacesByUser(string userId, List<string> projectIds);
        Task<WorkspaceTemplate?> GetWorkspaceTemplate(string templateId);
        Task<List<WorkspaceTemplate>> ListWorkspaceTemplates();
    }
}