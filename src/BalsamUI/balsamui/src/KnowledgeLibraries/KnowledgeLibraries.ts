import { KnowledgeLibraryApi } from "../../BalsamAPI/api";
import { Template } from "../Model/Template";
import Templates from "../Templates/templates";
import { KnowledgeLibrary, RepoFile } from "../services/BalsamAPIServices";


async function getTemplates(api: KnowledgeLibraryApi, knowledgeLibrary: KnowledgeLibrary) : Promise<Array<Template>>
{
    let promise = api.listKnowledgeLibraryFiles(knowledgeLibrary.id);
    let files = (await promise).data;
    
    return getTemplatesFromFiles(api, files, knowledgeLibrary);
}

async function getTemplatesFromFiles(api: KnowledgeLibraryApi, files: Array<RepoFile>, knowledgeLibrary: KnowledgeLibrary) : Promise<Array<Template>>
{
    
    let templateFiles = Templates.getTemplateFiles(files);
    let templatesArray = await Templates.convertToTemplates(templateFiles, async (fileId): Promise<Template> => {
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

async function getAllTemplates(api: KnowledgeLibraryApi) : Promise<Array<Template>>
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



const KnowLedgeLibraries = {
    getAllTemplates,
    getTemplates,
    getTemplatesFromFiles
}

export default KnowLedgeLibraries;