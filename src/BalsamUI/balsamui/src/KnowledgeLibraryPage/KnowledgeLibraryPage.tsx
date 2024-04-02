import { useContext, useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { useDispatch } from "react-redux";
import { postError } from "../Alerts/alertsSlice";
import { KnowledgeLibrary, RepoFile, RepoFileTypeEnum } from "../services/BalsamAPIServices";
import AppContext, { AppContextState } from "../configuration/AppContext";
import { Accordion, Box, Button, Tab, Tabs } from "@mui/material";
import MarkdownViewer from "../MarkdownViewer/MarkdownViewer";
import { AxiosResponse } from "axios";
import { KnowledgeLibraryResource } from "../Model/Resource";
import CustomTabPanel from "../CustomTabPanel/CustomTabPanel";
import { RepositoryTemplate } from "../Model/RepositoryTemplate";
import KnowledgeLibraryResourcesSection from "../KnowledgeLibraryResourcesSection/KnowledgeLibraryResourcesSection";
import RepositoryTemplatesSection from "../RepositoryTemplatesSection/RepositoryTemplatesSection";
import KnowLedgeLibraries from "../KnowledgeLibraries/KnowledgeLibraries";
import { Link } from 'react-router-dom';
import { OpenInNew } from "@mui/icons-material";
import KnowledgeLibraries from "../KnowledgeLibraries/KnowledgeLibraries";
import { RichTreeView } from '@mui/x-tree-view/RichTreeView';
import TreeHelper, { FileTreeNode } from "../TreeHelper/TreeHelper";
import { toRepoFileTypeEnum } from "../ReposFiles/RepoFiles";

export default function KnowledgeLibraryPage()
{
    const [library, setLibrary] = useState<KnowledgeLibrary>();
    const [loading, setLoading] = useState(true);
    const { id } = useParams<string>();
    const dispatch = useDispatch();
    const appContext = useContext(AppContext) as AppContextState;
    const [readmeMarkdown, setReadmeMarkdown] = useState<string>();
    const [resources, setResources] = useState<Array<KnowledgeLibraryResource>>();
    const [templates, setTemplates] = useState<Array<RepositoryTemplate>>();
    const [articleFiles, setArticleFiles] = useState<Array<RepoFile>>();
    const [articlesTree, setArticlesTree] = useState<Array<FileTreeNode>>();
    const [articleMarkdown, setArticleMarkdown] = useState("*Ingen artikel vald*");
    const [selectedTab, setSelectedTab] = useState(0);

    useEffect(() => {
        
        setLoading(true);
        
        if (id === undefined)
        {
            dispatch(postError("Inget kunskapsbibliotek är valt")); //TODO: Language
            return;
        }

        //TODO: API does not support GET by id yet...
        appContext.balsamApi.knowledgeLibraryApi.listKnowledgeLibaries()
        .then((response) => {
            let library = response.data.find(kb => kb.id == id);
            if (library)
            {
                setLibrary(library);
                loadFiles(library);
                setLoading(false);
            }
            else 
            {
                dispatch(postError("Det gick inte att ladda kunskapsbibliotek")); //TODO: Language
            }

        })
        .catch(() => {
            dispatch(postError("Det gick inte att ladda kunskapsbibliotek")); //TODO: Language
        });

    },[id])

    function loadReadmeContent(knowledgeLibraryId: string, fileId: string)
    {
        appContext.balsamApi.knowledgeLibraryApi.getKnowledgeLibraryFileContent(knowledgeLibraryId, fileId)
        .then((response) => 
        {   
            setReadmeMarkdown(response.data);
        })
        .catch( () => {
            setReadmeMarkdown("Fel vid inläsning av README.md"); //TODO: Language
        });
    }

    const loadFiles = (knowledgeLibrary: KnowledgeLibrary) => {


        appContext.balsamApi.knowledgeLibraryApi.listKnowledgeLibraryFiles(knowledgeLibrary.id)
        .then(async (response) => {
            
            let axResponse = response as AxiosResponse<RepoFile[], any>;

            let files = axResponse.data;

            let readmeFile = files.find((file) => file.path.toLowerCase() === "readme.md");

            if (readmeFile && readmeFile.id)
            {
                loadReadmeContent(knowledgeLibrary.id, readmeFile.id);
            }

            let knowledgeLibraryResources = await KnowledgeLibraries.getResources(appContext.balsamApi.knowledgeLibraryApi, knowledgeLibrary)
            setResources(knowledgeLibraryResources);

            let templatesArray = await KnowLedgeLibraries.getTemplatesFromFiles(appContext.balsamApi.knowledgeLibraryApi, files, knowledgeLibrary);
            setTemplates(templatesArray)

            let artFiles = KnowLedgeLibraries.getArticlesFromFiles(files);
            setArticleFiles(artFiles);

            let treeNodes = TreeHelper.convertToFileTreeNodes(artFiles);

           
        
            if (treeNodes.length > 0)
            {
                 //Remove top 'Articles' node
                let unparented = treeNodes[0].children;

                setArticlesTree(unparented);
            }
            else 
            {
                setArticlesTree([]);
            }

        })
        .catch(() => {
            dispatch(postError("Det gick inte att ladda filer")); //TODO: Language
        });

    };

    const handleTabChange = (_event: React.SyntheticEvent, newTab: number) => {
        setSelectedTab(newTab);
    };
    
    function renderReadme() {
        let readmeElement = readmeMarkdown
            ? <div className="scroll"><MarkdownViewer markdown={readmeMarkdown} /></div>
            : <p><em>ingen readme</em></p>

        return (
            readmeElement
        );
    }

    function handleItemSelectionToggle(_event: React.SyntheticEvent<Element, Event>, itemId: string, isSelected: boolean) {
        if (articlesTree === undefined || articleFiles === undefined || library === undefined)
        {
            return undefined;
        }
        
        if (isSelected)
        {
            let file = articleFiles.find(x=> x.path === itemId);

            if (file && toRepoFileTypeEnum(file.type) === RepoFileTypeEnum.File)
            {
                KnowLedgeLibraries.getArticleContent(appContext.balsamApi.knowledgeLibraryApi, library.id, file.id)
                .then(content => {
                    setArticleMarkdown(content);    
                });
            }
        }
    }

    function getItemLabel(item: FileTreeNode)
    {
        //Remove file extension
        let name = item.name.replace(/\.[^/.]+$/, "");
        return name;
    }

    function renderArticles()
    {

        if (articlesTree === undefined)
        {
            return undefined;
        }

        return  <Box 
                    sx={{
                        display:"flex"
                    }}> 
                    <RichTreeView
                        sx={{
                            minWidth:"200px"
                        }}
                        items={articlesTree || []}
                        getItemId={ (item) => item.path}
                        getItemLabel={item => getItemLabel(item)}
                        onItemSelectionToggle={handleItemSelectionToggle}
                    />
                    <Box 
                        sx={{
                            flex:1,
                            margin: "0px 12px"
                        }}>
                        <MarkdownViewer markdown={articleMarkdown} />
                    </Box>

                </Box>
    }

    function renderLibrary(library: KnowledgeLibrary)
    {
        let readmeElement = renderReadme();
        let artilces = renderArticles();

        function tabProps(index: number) {
            return {
                id: `project-tab-${index}`,
                'aria-controls': `project-tabpanel-${index}`,
            };
        }

        return (            
            <div>
                <div className="project-header">
                    <div><h2>{library.name}<Button component={Link as any} target="_blank" underline="hover" to={library.repositoryFriendlyUrl}>Git<OpenInNew fontSize="inherit" /></Button></h2></div>
                    <Accordion defaultExpanded >
                        <Box sx={{ width: '100%' }}>
                            <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
                                <Tabs value={selectedTab} onChange={handleTabChange} aria-label="Tabbar för filer och resurser">
                                    <Tab label="Artiklar" {...tabProps(0)} />
                                    <Tab label="Resurser" {...tabProps(1)} />
                                    <Tab label="Mallar och Exempel" {...tabProps(2)} />
                                    <Tab label="README.md" {...tabProps(3)} />
                                </Tabs>
                            </Box>
                            <CustomTabPanel value={selectedTab} index={0}>
                                {artilces}
                            </CustomTabPanel>
                            <CustomTabPanel className="cards-tab" value={selectedTab} index={1}>
                                <KnowledgeLibraryResourcesSection resources={resources} />
                            </CustomTabPanel>
                            <CustomTabPanel className="cards-tab" value={selectedTab} index={2}>
                                <RepositoryTemplatesSection knowledgeLibraryId={library.id} templates={templates} />
                            </CustomTabPanel>
                            <CustomTabPanel value={selectedTab} index={3}>
                                {readmeElement}
                            </CustomTabPanel>
                        </Box>
                    </Accordion>
                </div>
            </div>
            )
    }

    let contents = loading || library === undefined
        ? <p><em>Laddar...</em></p>
        : renderLibrary(library);

    return (
        <div>
            {contents}
        </div>
    ); 

}


