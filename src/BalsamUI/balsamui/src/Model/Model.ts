import { ModelFile, ModelFileTypeEnum } from "../services/BalsamAPIServices";

export enum ResourceType {
    Document,
    Url,
    Md,
    Folder
}

export interface Resource
{
    name: string,
    description: string,
    type: ResourceType,
    linkUrl?: string,
    contentUrl: string,
    filePath: string,
}

export const getResourceType = (file: ModelFile): ResourceType => 
{
    if (file.type === ModelFileTypeEnum.Folder)
    {
        return ResourceType.Folder;
    }
    if(file.name.toLocaleUpperCase().endsWith(".url"))
    {
        return ResourceType.Url;
    }
    else if (file.name.toLowerCase().endsWith(".md"))
    {
        return ResourceType.Md;
    }
    

    return ResourceType.Document;
};