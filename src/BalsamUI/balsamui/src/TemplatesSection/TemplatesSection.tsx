import { useState } from "react";
import { Template } from "../Model/Template";
import TemplateCard from "../TemplateCard/TemplateCard";
import NewProjectDialog from "../NewProjectDialog/NewProjectDialog";

export interface TemplatesSectionProperties{
    knowledgeLibraryId: string,
    templates?: Array<Template>,
}

export default function ProjectResourcesSection(props: TemplatesSectionProperties) {
    const [newDialogOpen, setNewDialogOpen] = useState(false);
    const [defaultTemplate, setDefaultTemplate] = useState<Template>();

    const onNewProjectDialogClosing = () => {
        setNewDialogOpen(false);
    };

    const onNewProjectClick = (template: Template) => {
        setDefaultTemplate(template);
        setNewDialogOpen(true);
        
    };

    function renderResources(resources: Array<Template>) {
        return (
            
            <div className='cards'>
                {resources.map((template: Template) =>
                        <TemplateCard knowledgeLibraryId={props.knowledgeLibraryId} template={template} newProjectClick={onNewProjectClick} key={template.fileId} />
                )}

                <NewProjectDialog open={newDialogOpen} defaultTemplate={defaultTemplate} onClosing={onNewProjectDialogClosing} ></NewProjectDialog>
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