import { useState, useEffect, useContext } from 'react';
import { useParams } from 'react-router-dom'
import MarkdownViewer from '../MarkdownViewer/MarkdownViewer'
import { Paper } from '@mui/material';
import AppContext, { AppContextState } from '../configuration/AppContext';
import { postError } from '../Alerts/alertsSlice';
import { useDispatch } from 'react-redux';
import { convertToResources } from '../Resources/Resources';
import { Resource, ResourceType } from '../Model/Resource';


export default function KnowledgeLibraryResorucePage() {
    const [content, setContent] = useState<string>();
    const [resource, setResource] = useState<Resource>();
    const [loading, setLoading] = useState<boolean>();
    const {knowledgeLibraryId, fileId} = useParams<string>();
    const appContext = useContext(AppContext) as AppContextState;

    const dispatch = useDispatch();

    useEffect(() => {

        setLoading(true);

        const fetchData = async () => {

            if (fileId !== undefined && knowledgeLibraryId)
            {
                
                //TODO: API doesen't have a method to get a single file-header
                appContext.balsamApi.knowledgeLibraryApi.listKnowledgeLibraryFiles(knowledgeLibraryId)
                    .then( async (response) => 
                    {   
                        let files = response.data;
                        let file = files.find(f => f.id === fileId);

                        if (file)
                        {
                            let resourceArray = await convertToResources([ file],  async (fileId): Promise<string> => {
                                let promise = appContext.balsamApi.knowledgeLibraryApi.getKnowledgeLibraryFileContent(knowledgeLibraryId, fileId);
                                return (await promise).data;
                            });

                            let fileResource = resourceArray[0]
                            setResource(fileResource);
                            
                            appContext.balsamApi.knowledgeLibraryApi.getKnowledgeLibraryFileContent(knowledgeLibraryId, fileId)
                            .then((contentResponse) => 
                            {   
                                setContent(contentResponse.data);
                                setLoading(false);
                            })
                            .catch( () => {
                                setContent("Fel vid inläsning av fil"); //TODO: Language
                            });
                        }
                        else 
                        {
                            dispatch(postError("Filen finns inte")); //TODO: Language
                        }
                    })
                    .catch( () => {
                        setContent("Fel vid inläsning av fil"); //TODO: Language
                    });
                
            }
        }

        fetchData()
            .catch(console.error);

    }, [])

    function renderContent()
    {
        if (resource === undefined || content === undefined)
        {
            return null;
        }

        switch(resource.type)
        {
            case ResourceType.Md: return <MarkdownViewer markdown={content} />;
            default: return null;
        }
    }


    function renderResource() {
        if (resource === undefined)
        {
            return;
        }

        let formattedContent = renderContent();
        return (
            <div>
                <h2>{resource.name}</h2>
                
                <Paper sx={{padding:7}} elevation={3}>
                    {formattedContent}
                </Paper>
            </div>
        );
    }

    let contents = loading
        ? <p><em>Laddar...</em></p>
        : renderResource();

    return (
        <div>
            {contents}
        </div>
    );
}