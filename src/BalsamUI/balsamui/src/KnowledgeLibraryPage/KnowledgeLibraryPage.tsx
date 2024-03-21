import { useContext, useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { useDispatch } from "react-redux";
import { postError } from "../Alerts/alertsSlice";
import { KnowledgeLibrary, RepoFile } from "../services/BalsamAPIServices";
import AppContext, { AppContextState } from "../configuration/AppContext";
import { Accordion, Box, Tab, Tabs } from "@mui/material";
import MarkdownViewer from "../MarkdownViewer/MarkdownViewer";
import Resources from '../Resources/Resources';
import { AxiosResponse } from "axios";
import { KnowledgeLibraryResource } from "../Model/Resource";
import CustomTabPanel from "../CustomTabPanel/CustomTabPanel";
import Templates from "../Templates/templates";
import { Template } from "../Model/Template";
import KnowledgeLibraryResourcesSection from "../KnowledgeLibraryResourcesSection/KnowledgeLibraryResourcesSection";
import TemplatesSection from "../TemplatesSection/TemplatesSection";
import KnowLedgeLibraries from "../KnowledgeLibraries/KnowledgeLibraries";

export default function KnowledgeLibraryPage()
{
    const [library, setLibrary] = useState<KnowledgeLibrary>();
    const [loading, setLoading] = useState(true);
    const { id } = useParams<string>();
    const dispatch = useDispatch();
    const appContext = useContext(AppContext) as AppContextState;
    const [readmeMarkdown, setReadmeMarkdown] = useState<string>();
    const [resources, setResources] = useState<Array<KnowledgeLibraryResource>>();
    const [templates, setTemplates] = useState<Array<Template>>();
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

            let resourceFiles = Resources.getResourceFiles(files);
            let readmeFile = files.find((file) => file.path.toLowerCase() === "readme.md");

            if (readmeFile && readmeFile.id)
            {
                loadReadmeContent(knowledgeLibrary.id, readmeFile.id);
            }

            let resourcesArray = await Resources.convertToResources(resourceFiles, async (fileId): Promise<string> => {
                let promise = appContext.balsamApi.knowledgeLibraryApi.getKnowledgeLibraryFileContent(knowledgeLibrary.id, fileId);
                return (await promise).data;
            });

            let knowledgeLibraryResources = resourcesArray.map( r => { 
                return { knowledgeLibraryId : knowledgeLibrary.id,
                         resource: r
                        } as KnowledgeLibraryResource;
                    })

            setResources(knowledgeLibraryResources);

            let templatesArray = await KnowLedgeLibraries.getTemplatesFromFiles(appContext.balsamApi.knowledgeLibraryApi, files, knowledgeLibrary);
            
            setTemplates(templatesArray)

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

    function renderLibrary(library: KnowledgeLibrary)
    {
        let readmeElement = renderReadme();

        function tabProps(index: number) {
            return {
                id: `project-tab-${index}`,
                'aria-controls': `project-tabpanel-${index}`,
            };
        }

        return (            
            <div>
                <div className="project-header">
                    <div><h2>{library.name}</h2></div>
                    <Accordion defaultExpanded >
                        
                        <Box sx={{ width: '100%' }}>
                        <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
                            <Tabs value={selectedTab} onChange={handleTabChange} aria-label="Tabbar för filer och resurser">
                                <Tab label="README.md" {...tabProps(0)} />
                                <Tab label="Resurser" {...tabProps(1)} />
                                <Tab label="Mallar och Exempel" {...tabProps(2)} />
                            </Tabs>
                        </Box>
                        <CustomTabPanel value={selectedTab} index={0}>
                            {readmeElement}
                        </CustomTabPanel>
                        <CustomTabPanel className="cards-tab" value={selectedTab} index={1}>
                            <KnowledgeLibraryResourcesSection resources={resources} />
                        </CustomTabPanel>
                        <CustomTabPanel className="cards-tab" value={selectedTab} index={2}>
                            <TemplatesSection knowledgeLibraryId={library.id} templates={templates} />
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
