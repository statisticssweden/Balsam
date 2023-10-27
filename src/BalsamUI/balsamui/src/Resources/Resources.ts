import { Resource, ResourceType, getResourceType } from "../Model/Model";
import { ModelFile, ModelFileTypeEnum } from "../services/BalsamAPIServices";

function getResourceFolders(files: ModelFile[])
    {
        let filteredFiles = files.filter((file) => {
            return file.type === ModelFileTypeEnum.Folder 
            && file.path.startsWith("Resources/")
            && file.path.split("/").length === 2
        });
        return filteredFiles;
    }

function getFirstLevelResoruceFiles(files: ModelFile[])
{
    let filteredFiles = files.filter((file) => {
        return file.type === ModelFileTypeEnum.File 
        && file.path.startsWith("Resources/")
        && file.path.split("/").length === 2
    });
    return filteredFiles;
}

export function getResourceFiles(files: ModelFile[])
{
    let resourceFiles = getResourceFolders(files);
    resourceFiles.concat(getFirstLevelResoruceFiles(files));
    return resourceFiles;
}

export async function convertToResources(files: ModelFile[], getContentCallback: (url: string) => Promise<string>)
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
                let content = await getContentCallback(file.contentUrl);
                let matches = content.match(/URL\s*=\s*(\S*)/)
                linkUrl =  matches !== null && matches.length > 0 ? matches[0] : "";
                description = file.contentUrl;

                break;
            case ResourceType.Document:
                description = file.name;
                break;
            default:
                description = file.name;
        }


        return { 
            name: name,
            description: description,
            type: type,
            linkUrl: linkUrl,
            contentUrl: file.contentUrl,
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