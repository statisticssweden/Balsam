import { RepositoryTemplate } from "../Model/RepositoryTemplate";
import { toRepoFileTypeEnum } from "../ReposFiles/RepoFiles";
import { RepoFile, RepoFileTypeEnum } from "../services/BalsamAPIServices";

function getTemplateFiles(files: RepoFile[])
    {
        let filteredFiles = files.filter((file) => {
            return toRepoFileTypeEnum(file.type) === RepoFileTypeEnum.File 
                && file.path.startsWith("Templates/")
                && file.path.toLowerCase().endsWith(".json")
                && file.path.split("/").length === 2
        });
        return filteredFiles;
    }

export async function convertToTemplates(files: RepoFile[], getContentCallback: (fileId: string) => Promise<RepositoryTemplate>) : Promise<Array<RepositoryTemplate>>
{
    let resourcesArray = await Promise.all(files.map( async (file): Promise<RepositoryTemplate> => {
        try{
            let template = await getContentCallback(file.id);
            
            template.fileId = file.id
            return template;
        }
        catch (_error)
        {
            return {
                fileId: file.id,
                description: `<Gitt inte att läsa fil ${file.path}`,
                git:"",
                html:"",
                name: "",
                tags: []

            }
        }
        
    }));
    return resourcesArray;
}

const RepositoryTemplates = {
    getTemplateFiles,
    convertToTemplates
}

export default RepositoryTemplates;