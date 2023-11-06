import { useState, useEffect, useContext } from 'react';
import { useParams, useSearchParams } from 'react-router-dom'
import MarkdownViewer from '../MarkdownViewer/MarkdownViewer'
import { Accordion, Box, Tab, Tabs } from '@mui/material';
import AppContext, { AppContextState } from '../configuration/AppContext';
import { AxiosResponse } from 'axios';
import { RepoFile } from '../services/BalsamAPIServices';
import { useDispatch } from 'react-redux';
import { postError } from '../Alerts/alertsSlice';
import FileTree, { convertToFileTreeNodes, getAllIds } from '../FileTree/FileTree';
import CustomTabPanel from '../CustomTabPanel/CustomTabPanel';


export default function ResoruceFolderPage() {
    const [markdown, setMarkdown] = useState<string>();
    const [loading, setLoading] = useState<boolean>(true);
    const [searchParams] = useSearchParams();
    const [resourceFolderName, setResourceFolderName] = useState<string>();
    const {projectId, branchId} = useParams<string>();
    const appContext = useContext(AppContext) as AppContextState;
    const [files, setFiles] = useState<Array<RepoFile>>();
    const [selectedTab, setSelectedTab] = useState(0);
    
    const dispatch = useDispatch();

    useEffect(() => {
        setResourceFolderName(searchParams.get("folder") || undefined );
    }, [searchParams]);

    function loadReadmeFileContent(projectId: string, branchId: string, readmeFile: RepoFile)
    {
        if (readmeFile.id) //TODO: id is going to change name to blobId
        {
            appContext.balsamApi.projectApi.getFile(projectId, branchId, readmeFile.id)
            .then((response) => 
            {   
                setMarkdown(response.data);

                setLoading(false);
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

        setLoading(true);

        if (projectId !== undefined && branchId !== undefined && resourceFolderName !== undefined)
        {
            appContext.balsamApi.projectApi.getFiles(projectId, branchId)
            .then((response) => 
            {   
                let axResponse = response as AxiosResponse<any[], any>;
                let files = axResponse.data as Array<RepoFile>;
                
                let filesInResourceFolder = files.filter((file) => file.path.toLowerCase().startsWith(`Resources/${resourceFolderName}`.toLowerCase()));
                
                //Include Resources-folder to render tree correctly
                let resourceFolder = files.find(f=> f.path === `Resources`);
                if (resourceFolder)
                {
                    filesInResourceFolder.push(resourceFolder);
                }

                setFiles(filesInResourceFolder);

                let readmeFile = files.find((file) => file.path.toLowerCase() === `Resources/${resourceFolderName}/readme.md`.toLowerCase());

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

    const handleTabChange = (_event: React.SyntheticEvent, newTab: number) => {
        setSelectedTab(newTab);
    };

    function renderFilesTree()
    {
        if (files === undefined)
        {
            return;
        }

        let fileTree = convertToFileTreeNodes(files);
        let allIds = getAllIds(fileTree);

        return (<FileTree fileTree={fileTree} defaultExpanded={allIds}></FileTree>);
    }

    function renderResource(markdown: string) {
        return (
            <MarkdownViewer markdown={markdown} />
        );
    }

    let readmeContents = loading
        ? <p><em>Laddar...</em></p>
        : renderResource(markdown || "");

    let fileTreeContent = renderFilesTree();

    function tabProps(index: number) {
        return {
            id: `folder-tab-${index}`,
            'aria-controls': `folder-tabpanel-${index}`,
        };
    }

    return (
        <div>
            <h2>{resourceFolderName}</h2>
            <Accordion defaultExpanded >
                <Box sx={{ width: '100%' }}>
                    <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
                        <Tabs value={selectedTab} onChange={handleTabChange} aria-label="Tabbar för readme och filer">
                            <Tab label="README.md" {...tabProps(0)} />
                            <Tab label="Filer" {...tabProps(1)} />
                        </Tabs>
                    </Box>
                    <CustomTabPanel value={selectedTab} index={0}>
                        {readmeContents}
                    </CustomTabPanel>
                    <CustomTabPanel value={selectedTab} index={1}>
                        {fileTreeContent}
                    </CustomTabPanel>
                </Box>
            </Accordion>  
        </div>
    );
}