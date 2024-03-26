import KnowledgeLibraryResourceCard from '../KnowledgeLibraryResourceCard/KnowledgeLibraryResourceCard';
import { KnowledgeLibraryResource } from "../Model/Resource";


export interface KnowledgeLibraryResourcesSectionProperties{
    resources?: Array<KnowledgeLibraryResource>,
}

export default function KnowledgeLibraryResourcesSection(props: KnowledgeLibraryResourcesSectionProperties) {
 
    function renderResources(resources: Array<KnowledgeLibraryResource>) {
        return (
            
            <div className='cards' aria-labelledby="tabelLabel">
                {resources.map((resource: KnowledgeLibraryResource) =>
                    <KnowledgeLibraryResourceCard resource={resource} key={resource.resource.filePath} />
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