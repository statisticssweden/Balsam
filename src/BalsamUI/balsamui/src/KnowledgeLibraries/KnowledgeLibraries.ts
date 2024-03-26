import { KnowledgeLibraryApi } from "../../BalsamAPI/api";
import { RepositoryTemplate } from "../Model/RepositoryTemplate";
import { KnowledgeLibraryResource } from "../Model/Resource";
import { toRepoFileTypeEnum } from "../ReposFiles/RepoFiles";
import RepositoryTemplates from "../RepositoryTemplates/repositoryTemplates";
import Resources from "../Resources/Resources";
import { KnowledgeLibrary, RepoFile, RepoFileTypeEnum } from "../services/BalsamAPIServices"; 

async function getTemplates(api: KnowledgeLibraryApi, knowledgeLibrary: KnowledgeLibrary) : Promise<Array<RepositoryTemplate>>
{
    let promise = api.listKnowledgeLibraryFiles(knowledgeLibrary.id);
    let files = (await promise).data;
    
    return getTemplatesFromFiles(api, files, knowledgeLibrary);
}

async function getTemplatesFromFiles(api: KnowledgeLibraryApi, files: Array<RepoFile>, knowledgeLibrary: KnowledgeLibrary) : Promise<Array<RepositoryTemplate>>
{
    
    let templateFiles = RepositoryTemplates.getTemplateFiles(files);
    let templatesArray = await RepositoryTemplates.convertToTemplates(templateFiles, async (fileId): Promise<RepositoryTemplate> => {
        let promise = api.getKnowledgeLibraryFileContent(knowledgeLibrary.id, fileId);
        let data = (await promise).data;

        if (typeof data === "string")
        {
            return JSON.parse(data);
        }

        return data;
    });

    return templatesArray;
}

async function getAllTemplates(api: KnowledgeLibraryApi) : Promise<Array<RepositoryTemplate>>
{
    let promise = api.listKnowledgeLibaries();
    let knowledgeLibraries = (await promise).data;

    let promises = knowledgeLibraries.map(knowledgeLibrary  => 
        {
            return getTemplates(api, knowledgeLibrary);
        });

    let reduced = Promise.all(promises);

    let templates = (await reduced).flatMap(arr => arr);
    
    return templates;
}

async function getResources(api: KnowledgeLibraryApi, knowledgeLibrary: KnowledgeLibrary) : Promise<Array<KnowledgeLibraryResource>>
{
    let promise = api.listKnowledgeLibraryFiles(knowledgeLibrary.id);
    let files = (await promise).data;
    
    return getResourcesFromFiles(api, files, knowledgeLibrary);
}

async function getResourcesFromFiles(api: KnowledgeLibraryApi, files: Array<RepoFile>, knowledgeLibrary: KnowledgeLibrary) : Promise<Array<KnowledgeLibraryResource>>
{
    
    let resourceFiles = Resources.getResourceFiles(files);
    let resourcesArray = await Resources.convertToResources(resourceFiles, async (fileId): Promise<string> => {
        let promise = api.getKnowledgeLibraryFileContent(knowledgeLibrary.id, fileId);
        return (await promise).data;
    });

    let knowledgeLibraryResources = resourcesArray.map( r => { 
        return { knowledgeLibrary : knowledgeLibrary,
                 resource: r
                } as KnowledgeLibraryResource;
            })

    return knowledgeLibraryResources;
}

async function getAllResources(api: KnowledgeLibraryApi) : Promise<Array<KnowledgeLibraryResource>>
{
    let promise = api.listKnowledgeLibaries();
    let knowledgeLibraries = (await promise).data;

    let promises = knowledgeLibraries.map(knowledgeLibrary  => 
        {
            return getResources(api, knowledgeLibrary);
        });

    let reduced = Promise.all(promises);

    let resources = (await reduced).flatMap(arr => arr);
    
    return resources;
}

function getArticlesFromFiles(files: Array<RepoFile>) : Array<RepoFile>
{
    
    let articleFiles = files.filter((file) => {
        return file.path.startsWith("Articles/")
    });

    return articleFiles;
}

async function getArticleContent(api: KnowledgeLibraryApi, libraryId: string, fileId: string): Promise<string>
{
    let promise = api.getKnowledgeLibraryFileContent(libraryId, fileId);
    return (await promise).data;
}

const KnowLedgeLibraries = {
    getAllTemplates,
    getTemplates,
    getTemplatesFromFiles,
    getAllResources,
    getResources,
    getResourcesFromFiles,
    getArticlesFromFiles,
    getArticleContent

}

export default KnowLedgeLibraries;