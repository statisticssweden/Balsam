export enum ResourceType {
    Document,
    Url,
    Md,
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

export const getResourceType = (fileName: string): ResourceType => 
{
    if(fileName.toLocaleUpperCase().endsWith(".url"))
    {
        return ResourceType.Url;
    }
    else if (fileName.toLowerCase().endsWith(".md"))
    {
        return ResourceType.Md;
    }

    return ResourceType.Document;
};