import { useState, useEffect, useContext } from 'react';
import { useParams, useSearchParams } from 'react-router-dom'
import AppContext, { AppContextState } from '../configuration/AppContext';
import { AxiosResponse } from 'axios';
import { RepoFile } from '../services/BalsamAPIServices';
import { useDispatch } from 'react-redux';
import { postError } from '../Alerts/alertsSlice';
import ResoruceFolder from '../ResourceFolder/ResourceFolder';
import Resources from '../Resources/Resources';

export default function KnowLedgeLibraryResoruceFolderPage() {
    const [markdown, setMarkdown] = useState<string>();
    const [searchParams] = useSearchParams();
    const [resourceFolderName, setResourceFolderName] = useState<string>("");
    const {knowledgeLibraryId} = useParams<string>();
    const appContext = useContext(AppContext) as AppContextState;
    const [files, setFiles] = useState<Array<RepoFile>>();
    
    const dispatch = useDispatch();

    useEffect(() => {
        let folder = searchParams.get("folder")
        if (folder !== null && folder.length > 0)
        {
            setResourceFolderName(folder);
        }
    }, [searchParams]);

    function loadReadmeFileContent(knowledgeLibraryId: string, readmeFile: RepoFile)
    {
        if (readmeFile.id)
        {
            appContext.balsamApi.knowledgeLibraryApi.getKnowledgeLibraryFileContent(knowledgeLibraryId, readmeFile.id)
            .then((response) => 
            {   
                setMarkdown(response.data);

            })
            .catch( () => {
                setMarkdown(`README.md i resursen ${resourceFolderName} gick inte att läsa`); //TODO: Language
            });
        } 
        else 
        {
            setMarkdown(`README.md saknar innehåll`); //TODO: Language   
        }  
    }



    useEffect(() => {


        if (knowledgeLibraryId && resourceFolderName !== undefined && resourceFolderName.length > 0)
        {
            appContext.balsamApi.knowledgeLibraryApi.listKnowledgeLibraryFiles(knowledgeLibraryId)
            .then((response) => 
            {   
                let axResponse = response as AxiosResponse<any[], any>;
                let files = axResponse.data as Array<RepoFile>;
                
                let filesInResourceFolder = Resources.getFiles(files, resourceFolderName);

                setFiles(filesInResourceFolder);

                let readmeFile = Resources.getReadmeFile(files, resourceFolderName);

                if (readmeFile)
                {
                    loadReadmeFileContent(knowledgeLibraryId, readmeFile);
                }
                else {
                    setMarkdown(`README.md saknas i mappen 'Resoruces/${resourceFolderName}'`); //TODO: Language   
                }
            })
            .catch( () => {
                dispatch(postError(`Det gick inte att läsa filer för resursen ${resourceFolderName}`)); //TODO: Language
            });
            
        }
       

    }, [resourceFolderName])

    useEffect(() => {
        
    }, [files]);

    return (
            <ResoruceFolder readmeMarkdown={markdown} resourceFolderName={resourceFolderName} files={files} />
    );
}