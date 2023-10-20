import WorkspaceCard from "../WorkspaceCard/WorkspaceCard";
import { Template, Workspace } from "../services/BalsamAPIServices";

export interface WorkspaceSectionProperties
{
    projectid: string,
    branch: string,
    workspaces?: Array<Workspace>,
    templates?: Array<Template>,
    deleteWorkspaceCallback: (workspaceId: string) => void,
}

export default function WorkspacesSection(props: WorkspaceSectionProperties)
{

    const renderWorkspaces = (workspaces: Array<Workspace>) => {
        return (
            <div className='cards' aria-labelledby="tabelLabel">
                {workspaces.map((workspace: Workspace) => {
                    let templateName = workspace.templateId;

                    if(props.templates !== undefined)
                    {
                        templateName = props.templates.find(t => t.id === workspace.templateId)?.name || templateName;
                    }

                    return (<WorkspaceCard templateName={templateName} workspace={workspace} deleteWorkspaceCallback={props.deleteWorkspaceCallback} key={workspace.id} />);
                }
                )}
            </div>
        )

    }
    
    let contents = props.workspaces === undefined
        ? <p><em>Laddar resurser...</em></p>
        : renderWorkspaces(props.workspaces!);

    return (<div>
        { contents }
    </div>)
}