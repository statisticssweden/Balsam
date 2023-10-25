namespace Balsam.Api.Models
{
    public class WorkspaceContext : BalsamContext
    {
        public BalsamProject Project { get; set; }
        public BalsamBranch Branch { get; set; }
        public BalsamWorkspace Workspace { get; set; }
        public UserInfo User { get; set; }

        public WorkspaceContext(BalsamProject project, BalsamBranch branch, BalsamWorkspace workspace, UserInfo user) 
        { 
            Project = project;
            Branch = branch;
            Workspace = workspace;
            User = user;
        }
    }
}
