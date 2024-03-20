import ProjectResourceCard from '../ProjectResourceCard/ProjectResourceCard';
import { ProjectResource } from "../Model/Resource";


export interface ProjectResourcesSectionProperties{
    resources?: Array<ProjectResource>,
}

export default function ProjectResourcesSection(props: ProjectResourcesSectionProperties) {
 
    function renderResources(resources: Array<ProjectResource>) {
        return (
            
            <div className='cards' aria-labelledby="tabelLabel">
                {resources.map((resource: ProjectResource) =>
                    <ProjectResourceCard resource={resource} key={resource.resource.filePath} />
                )}
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