import ProjectResourceCard from '../ProjectResourceCard/ProjectResourceCard';
import { ProjectResource } from "../Model/Resource";
import NewCard from '../NewCard/NewCard';
import workspaceImage from '../assets/resource.png'

export interface AddResourceCardKeyType
{
    projectId: string,
    branchId: string
}

export interface ProjectResourcesSectionProperties{
    projectid: string,
    branch: string,
    resources?: Array<ProjectResource>,
    showNewCard?: boolean,
    onNewClick?: (itemKey: any) => void,
}

export default function ProjectResourcesSection(props: ProjectResourcesSectionProperties) {
 
    const newCardContent = props.showNewCard 
                ? <NewCard<AddResourceCardKeyType> itemKey={{projectId: props.projectid, branchId: props.branch }} buttonContent={"Lägg till resurs"} image={workspaceImage} onClick={props.onNewClick} />
                : ""
                
    function renderResources(resources: Array<ProjectResource>) {
        return (
            
            <div className='cards' aria-labelledby="tabelLabel">
                {resources.map((resource: ProjectResource) =>
                    <ProjectResourceCard resource={resource} key={resource.resource.filePath} />
                )}
                {newCardContent}
            </div>
        );
    }

    let contents = props.resources === undefined
        ? <p><em>Laddar resurser...</em></p>
        : renderResources(props.resources!);

    return (
        <div>
            {contents}
        </div>
    );
}