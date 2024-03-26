import { useState } from "react";
import { RepositoryTemplate } from "../Model/RepositoryTemplate";
import RepositoryTemplateCard from "../RepositoryTemplateCard/RepositoryTemplateCard";
import NewProjectDialog from "../NewProjectDialog/NewProjectDialog";

export interface RepositoryTemplatesSectionProperties{
    knowledgeLibraryId: string,
    templates?: Array<RepositoryTemplate>,
}

export default function RepositoryTemplateSection(props: RepositoryTemplatesSectionProperties) {
    const [newDialogOpen, setNewDialogOpen] = useState(false);
    const [defaultTemplate, setDefaultTemplate] = useState<RepositoryTemplate>();

    const onNewProjectDialogClosing = () => {
        setNewDialogOpen(false);
    };

    const onNewProjectClick = (template: RepositoryTemplate) => {
        setDefaultTemplate(template);
        setNewDialogOpen(true);
        
    };

    function renderResources(resources: Array<RepositoryTemplate>) {
        return (
            
            <div className='cards'>
                {resources.map((template: RepositoryTemplate) =>
                        <RepositoryTemplateCard knowledgeLibraryId={props.knowledgeLibraryId} template={template} newProjectClick={onNewProjectClick} key={template.fileId} />
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