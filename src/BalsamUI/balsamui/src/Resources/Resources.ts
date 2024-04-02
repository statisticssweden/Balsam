import { Resource, ResourceType, getResourceType } from "../Model/Resource";
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
    let allFiles = resourceFiles.concat(getFirstLevelResoruceFiles(files));
    return allFiles;
}

function getFiles(files: Array<RepoFile>, resourceFolderName: string) : Array<RepoFile>
{
    let filesInResourceFolder = files.filter((file) => file.path.toLowerCase().startsWith(`Resources/${resourceFolderName}`.toLowerCase()));
                
    //Include Resources-folder to render tree correctly
    let resourceFolder = files.find(f=> f.path === `Resources`);
    if (resourceFolder)
    {
        filesInResourceFolder.push(resourceFolder);
    }
    return filesInResourceFolder;
}

function getReadmeFile(files: Array<RepoFile>, resourceFolderName: string) : RepoFile | undefined
{
    let readmeFile = files.find((file) => file.path.toLowerCase() === `Resources/${resourceFolderName}/readme.md`.toLowerCase());
    return readmeFile;
}

export async function convertToResources(files: RepoFile[], getContentCallback: (fileId: string) => Promise<string>) : Promise<Array<Resource>>
{
    let resourcesArray = await Promise.all(files.map( async (file): Promise<Resource> => {
        let name = file.name;
        name = name.replace(/\.[^/.]+$/, ""); //Remove file ending
        
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
                if(file.id)
                {
                    let content = await getContentCallback(file.id);
                    let matches = content.match(/URL\s*=\s*(\S*)/)
                    linkUrl =  matches !== null && matches.length > 1 ? matches[1] : "";

                    description = `Länk till '${linkUrl}'`;
                }

                break;
            case ResourceType.Document:
                description = file.name;
                break;
            default:
                description = file.name;
        }


        return { 
            name: name,
            fileName: file.name,
            description: description,
            type: type,
            linkUrl: linkUrl,
            fileId: file.id,
            filePath: file.path
            }
    }));
    return resourcesArray;
}

const Resources = {
    getResourceFiles,
    convertToResources,
    getFiles,
    getReadmeFile
}

export default Resources;