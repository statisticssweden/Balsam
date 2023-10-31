import { useState, useEffect, useContext } from 'react';
import { useParams, useSearchParams } from 'react-router-dom'
import MarkdownViewer from '../MarkdownViewer/MarkdownViewer'
import HttpService from '../services/HttpServices';
import { Paper } from '@mui/material';
import AppContext, { AppContextState } from '../configuration/AppContext';


export default function ResoruceMarkdownPage() {
    const [markdown, setMarkdown] = useState<string>();
    const [loading, setLoading] = useState<boolean>();
    const [searchParams] = useSearchParams();
    const [resourceName, setResourceName] = useState<string>();
    const {projectId, branchId, fileId} = useParams<string>();
    const appContext = useContext(AppContext) as AppContextState;
    
    // const [fileId, setFileId] = useState<string>();

    useEffect(() => {
        setResourceName(searchParams.get("resourcename") || undefined );
        // setFileId(searchParams.get("fileid") || undefined );
    }, [searchParams]);

    useEffect(() => {

        setLoading(true);

        // declare the async data fetching function
        const fetchData = async () => {

            if (fileId !== undefined && projectId !== undefined && branchId !== undefined)
            {
                appContext.balsamApi.projectApi.getFile(projectId, branchId, fileId)
                    .then((response) => 
                    {   
                        setMarkdown(response.data);
                    })
                    .catch( () => {
                        setMarkdown("Fel vid inläsning av fil"); //TODO: Language
                    });
                setLoading(false);
            }
        }

        fetchData()
            .catch(console.error);

    }, [resourceName])

    function renderResource(markdown: string) {
        return (
            <div>
                <h2>{resourceName}</h2>
                <Paper sx={{padding:7}} elevation={3}>
                    <MarkdownViewer markdown={markdown} />
                </Paper>
            </div>
        );
    }

    let contents = loading
        ? <p><em>Laddar...</em></p>
        : renderResource(markdown || "");

    return (
        <div>
            {contents}
        </div>
    );
}