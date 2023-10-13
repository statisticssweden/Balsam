import ResourceCard from '../ResourceCard/ResourceCard';
import { Resource } from "../Model/Model";


export interface ResourceSectionProperties{
    projectid: string,
    branch: string,
    resources?: Array<Resource>,
}

export default function ResourcesSection(props: ResourceSectionProperties) {
 
    function renderResources(resources: Array<Resource>) {
        return (
            
            <div className='cards' aria-labelledby="tabelLabel">
                {resources.map((resource: Resource) =>
                    <ResourceCard resource={resource} key={resource.filePath} />
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