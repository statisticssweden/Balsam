import { useState, useEffect, useContext } from 'react';
import { useParams, useSearchParams } from 'react-router-dom'
import AppContext, { AppContextState } from '../configuration/AppContext';
import { AxiosResponse } from 'axios';
import { RepoFile } from '../services/BalsamAPIServices';
import { useDispatch } from 'react-redux';
import { postError } from '../Alerts/alertsSlice';
import ResoruceFolder from '../ResourceFolder/ResourceFolder';
import Resources from '../Resources/Resources';

export default function ResoruceFolderPage() {
    const [markdown, setMarkdown] = useState<string>();
    const [searchParams] = useSearchParams();
    const [resourceFolderName, setResourceFolderName] = useState<string>("");
    const {projectId, branchId} = useParams<string>();
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

    function loadReadmeFileContent(projectId: string, branchId: string, readmeFile: RepoFile)
    {
        if (readmeFile.id)
        {
            appContext.balsamApi.projectApi.getFile(projectId, branchId, readmeFile.id)
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


        if (projectId !== undefined && branchId !== undefined && resourceFolderName !== undefined && resourceFolderName.length > 0)
        {
            appContext.balsamApi.projectApi.getFiles(projectId, branchId)
            .then((response) => 
            {   
                let axResponse = response as AxiosResponse<any[], any>;
                let files = axResponse.data as Array<RepoFile>;
                
                let filesInResourceFolder = Resources.getFiles(files, resourceFolderName);

                setFiles(filesInResourceFolder);

                let readmeFile = Resources.getReadmeFile(files, resourceFolderName);

                if (readmeFile)
                {
                    loadReadmeFileContent(projectId, branchId, readmeFile);
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