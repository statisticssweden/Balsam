import { Template } from "../Model/Template";
import TemplateCard from "../TemplateCard/TemplateCard";

export interface TemplatesSectionProperties{
    knowledgeLibraryId: string,
    templates?: Array<Template>,
}

export default function ProjectResourcesSection(props: TemplatesSectionProperties) {
 
    function renderResources(resources: Array<Template>) {
        return (
            
            <div className='cards' aria-labelledby="tabelLabel">
                {resources.map((template: Template) =>
                    <TemplateCard knowledgeLibraryId={props.knowledgeLibraryId} template={template} key={template.fileId} />
                )}
            </div>
        );
    }

    let contents = props.templates === undefined
        ? <p><em>Laddar resurser...</em></p>
        : renderResources(props.templates!);

    return (
        <div>
            {contents}
        </div>
    );
}