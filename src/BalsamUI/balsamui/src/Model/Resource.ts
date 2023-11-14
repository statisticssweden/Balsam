import { toRepoFileTypeEnum } from "../ReposFiles/RepoFiles";
import { RepoFile, RepoFileTypeEnum } from "../services/BalsamAPIServices";

export enum ResourceType {
    Document,
    Url,
    Md,
    Folder
}

export interface Resource
{
    projectId: string,
    branchId: string,
    name: string,
    description: string,
    type: ResourceType,
    linkUrl?: string,
    fileId?: string,
    fileName?: string,
    filePath: string,
}

export const getResourceType = (file: RepoFile): ResourceType => 
{
    if (toRepoFileTypeEnum(file.type) === RepoFileTypeEnum.Folder)
    {
        return ResourceType.Folder;
    }
    else if(file.name.toLowerCase().endsWith(".url"))
    {
        return ResourceType.Url;
    }
    else if (file.name.toLowerCase().endsWith(".md"))
    {
        return ResourceType.Md;
    }
    
    return ResourceType.Document;
};