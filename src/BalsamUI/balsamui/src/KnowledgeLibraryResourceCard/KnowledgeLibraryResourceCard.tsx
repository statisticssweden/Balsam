import resourceImage from '../assets/text.png'
import markdownImage from '../assets/text.png'
import './KnowledgeLibraryResourceCard.css';
import { KnowledgeLibraryResource, ResourceType } from '../Model/Resource';
import FolderIcon from '@mui/icons-material/Folder';
import ResourceCard from '../ResourceCard/ResourceCard';

export interface KnowledgeLibraryResourceCardProperties {
    resource: KnowledgeLibraryResource
}

export default function KnowledgeLibraryResourceCard({ resource } : KnowledgeLibraryResourceCardProperties) {

    function renderLinkCard(name: string, description: string, cardActionAreaTo: string, image?: string, mediaContent?: any)
    {
        return (
            <ResourceCard name={name} description={description} cardActionAreaTo={cardActionAreaTo} image={image} mediaContent={mediaContent}   />
        );
    }
    
    let card: any;

    if(resource.resource.type === ResourceType.Folder)
    {
        let mediaContent = <div className='icon-container'> <FolderIcon sx={{fontSize:"134px", color:"lightgray"}}></FolderIcon></div>;
        let cardActionAreaTo = `/knowledgeLibrary/${resource.knowledgeLibraryId}/resourceFolder?folder=${resource.resource.fileName}`;
        card = renderLinkCard(resource.resource.name, resource.resource.description, cardActionAreaTo, undefined, mediaContent);
    }
    else if (resource.resource.type === ResourceType.Url) {
        let cardActionAreaTo = resource.resource.linkUrl || "";
        card = renderLinkCard(resource.resource.name, resource.resource.description, cardActionAreaTo, resourceImage);
    }
    else if (resource.resource.type === ResourceType.Md) {
        let cardActionAreaTo = `/knowledgelibrary/${resource.knowledgeLibraryId}/file/${resource.resource.fileId}`;
        card = renderLinkCard(resource.resource.name, resource.resource.description, cardActionAreaTo, markdownImage);
    }
    
    return card;
}