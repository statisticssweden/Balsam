import { useState } from 'react';
import MarkdownViewer from '../MarkdownViewer/MarkdownViewer'
import { Accordion, Box, Tab, Tabs } from '@mui/material';
import { RepoFile } from '../services/BalsamAPIServices';
import FileTree, { convertToFileTreeNodes, getAllIds } from '../FileTree/FileTree';
import CustomTabPanel from '../CustomTabPanel/CustomTabPanel';

export interface ResoruceFolderProperties
{
    readmeMarkdown?: string,
    resourceFolderName: string,
    files?: Array<RepoFile>
}

export default function ResoruceFolder(props: ResoruceFolderProperties) {
    const [selectedTab, setSelectedTab] = useState(0);
    

    const handleTabChange = (_event: React.SyntheticEvent, newTab: number) => {
        setSelectedTab(newTab);
    };

    function renderFilesTree()
    {
        if (props.files === undefined)
        {
            return;
        }

        let fileTree = convertToFileTreeNodes(props.files);
        let allIds = getAllIds(fileTree);

        return (<FileTree fileTree={fileTree} defaultExpanded={allIds}></FileTree>);
    }

    function renderResource(markdown: string) {
        return (
            <MarkdownViewer markdown={markdown} />
        );
    }

    let readmeContents = renderResource(props.readmeMarkdown || "");

    let fileTreeContent = renderFilesTree();

    function tabProps(index: number) {
        return {
            id: `folder-tab-${index}`,
            'aria-controls': `folder-tabpanel-${index}`,
        };
    }

    return (
        <div>
            <h2>{props.resourceFolderName || ""}</h2>
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