import resourceImage from '../assets/text.png'
import markdownImage from '../assets/text.png'
import { ProjectResource, ResourceType } from '../Model/Resource';
import FolderIcon from '@mui/icons-material/Folder';
import ResourceCard from '../ResourceCard/ResourceCard';

export interface ProjectResourceCardProperties {
    resource: ProjectResource
}

export default function ProjectResourceCard({ resource } : ProjectResourceCardProperties) {

    function renderLinkCard(name: string, description: string, cardActionAreaTo: string, image?: string, mediaContent?: any)
    {
        //TODO: Refactor and make generic
        return (
            <ResourceCard name={name} description={description} cardActionAreaTo={cardActionAreaTo} image={image} mediaContent={mediaContent}   />
        );
    }
    
    let card: any;

    if(resource.resource.type === ResourceType.Folder)
    {
        let mediaContent = <div className='icon-container'> <FolderIcon sx={{fontSize:"134px", color:"lightgray"}}></FolderIcon></div>;
        let cardActionAreaTo = `/resourcefolder/${resource.projectId}/${resource.branchId}/?folder=${resource.resource.fileName}`;
        card = renderLinkCard(resource.resource.name, resource.resource.description, cardActionAreaTo, undefined, mediaContent);
    }
    else if (resource.resource.type === ResourceType.Url) {
        let cardActionAreaTo = resource.resource.linkUrl || "";
        card = renderLinkCard(resource.resource.name, resource.resource.description, cardActionAreaTo, resourceImage);
    }
    else if (resource.resource.type === ResourceType.Md) {
        let cardActionAreaTo = `/resorucemarkdown/${resource.projectId}/${resource.branchId}/${resource.resource.fileId}?resourcename=${resource.resource.name}`;
        card = renderLinkCard(resource.resource.name, resource.resource.description, cardActionAreaTo, markdownImage);
    }
    
    return card;
}