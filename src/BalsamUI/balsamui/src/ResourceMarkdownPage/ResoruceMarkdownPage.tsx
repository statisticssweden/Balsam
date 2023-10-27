import { useState, useEffect } from 'react';
import { useSearchParams } from 'react-router-dom'
import MarkdownViewer from '../MarkdownViewer/MarkdownViewer'
import HttpService from '../services/HttpServices';
import { Paper } from '@mui/material';

export default function ResoruceMarkdownPage() {
    const [markdown, setMarkdown] = useState<string>();
    const [loading, setLoading] = useState<boolean>();
    const [searchParams] = useSearchParams();
    const [resourceName, setResourceName] = useState<string>();
    const [contentUrl, setContentUrl] = useState<string>();

    useEffect(() => {
        setResourceName(searchParams.get("resourcename") || undefined );
        setContentUrl(searchParams.get("contenturl") || undefined );
    }, [searchParams]);

    useEffect(() => {

        setLoading(true);

        // declare the async data fetching function
        const fetchData = async () => {

            if (contentUrl !== undefined)
            {
                HttpService.getTextFromUrl(contentUrl).then(
                    (text) => setMarkdown(text)
                );
            
                setLoading(false);
            }
        }

        fetchData()
            .catch(console.error);

    }, [resourceName, contentUrl])

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