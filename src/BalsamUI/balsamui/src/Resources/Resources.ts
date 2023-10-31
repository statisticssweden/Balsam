import { Resource, ResourceType, getResourceType } from "../Model/Model";
import { toRepoFileTypeEnum } from "../ReposFiles/RepoFiles";
import { RepoFile, RepoFileTypeEnum } from "../services/BalsamAPIServices";

function getResourceFolders(files: RepoFile[])
    {
        let filteredFiles = files.filter((file) => {
            return toRepoFileTypeEnum(file.type) === RepoFileTypeEnum.Folder 
            && file.path.startsWith("Resources/")
            && file.path.split("/").length === 2
        });
        return filteredFiles;
    }

function getFirstLevelResoruceFiles(files: RepoFile[])
{
    let filteredFiles = files.filter((file) => {
        return toRepoFileTypeEnum(file.type) === RepoFileTypeEnum.File 
        && file.path.startsWith("Resources/")
        && file.path.split("/").length === 2
    });
    return filteredFiles;
}

export function getResourceFiles(files: RepoFile[])
{
    let resourceFiles = getResourceFolders(files);
    resourceFiles.concat(getFirstLevelResoruceFiles(files));
    return resourceFiles;
}

export async function convertToResources(files: RepoFile[], projectId: string, branchId: string, getContentCallback: (fileId: string) => Promise<string>) : Promise<Array<Resource>>
{
    let resourcesArray = await Promise.all(files.map( async (file): Promise<Resource> => {
        let name = file.name;
        name = name.replace(/\.[^/.]+$/, "");
        
        let type = getResourceType(file);
        let linkUrl: string = "";
        
        let description = "";
        switch (type) {
            case ResourceType.Folder:
                description = "En resurs som består av en eller flera filer"; //TODO: Language
                break;
            case ResourceType.Md:
                description = "Markdownfil som går att läsa i gränssnittet"; //TODO: Language
                break;
            case ResourceType.Url:
                if(file.contentUrl)
                {
                    let content = await getContentCallback(file.contentUrl);
                    let matches = content.match(/URL\s*=\s*(\S*)/)
                    linkUrl =  matches !== null && matches.length > 0 ? matches[0] : "";

                    description = linkUrl;
                }

                break;
            case ResourceType.Document:
                description = file.name;
                break;
            default:
                description = file.name;
        }


        return { 
            projectId: projectId,
            branchId: branchId,
            name: name,
            fileName: file.name,
            description: description,
            type: type,
            linkUrl: linkUrl,
            fileId: file.contentUrl,
            filePath: file.path
            }
    }));
    return resourcesArray;
}

const Resources = {
    getResourceFiles,
    convertToResources
}

export default Resources;