import {Project, Branch, Workspace, Template, RepoFile} from '../services/BalsamAPIServices';
import MenuItem from '@mui/material/MenuItem';
import Select, { SelectChangeEvent } from '@mui/material/Select';
import FormControl from '@mui/material/FormControl';
import MarkdownViewer from '../MarkdownViewer/MarkdownViewer';
import { useDispatch } from 'react-redux';
import { useState, useEffect, useContext } from 'react';
import { useParams } from 'react-router-dom'
import { postError, postSuccess } from '../Alerts/alertsSlice';
import './ProjectPage.css'
import { Resource } from '../Model/Model';
import ResourcesSection from '../ResourceSection/ResourcesSection';
import AppContext, { AppContextState } from '../configuration/AppContext';
import WorkspacesSection from '../WorkspacesSection/WorkspacesSection';
import NewWorkspaceDialog from '../NewWorkspaceDialog/NewWorkspaceDialog';
import { Accordion, AccordionDetails, AccordionSummary, Box, Button, Divider, Tab, Tabs } from '@mui/material';
import { AxiosResponse } from 'axios';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';

import CustomTabPanel from '../CustomTabPanel/CustomTabPanel';
import FileTree, { convertToFileTreeNodes, getAllIds } from '../FileTree/FileTree';
import Resources from '../Resources/Resources';

export default function ProjectPage() {
    const [project, setProject] = useState<Project>();
    const [loading, setLoading] = useState(true);
    const { id } = useParams<string>();
    const [branches, setBranches] = useState<Array<Branch>>();
    const [selectedBranch, setSelectedBranch] = useState<string>();
    const [readmeMarkdown, setReadmeMarkdown] = useState<string>();
    const [resources, setResources] = useState<Array<Resource>>();
    const [workspaces, setWorkspaces] = useState<Array<Workspace>>();
    const [templates, setTemplates] = useState<Array<Template>>();
    const [newWorkspaceDialogOpen, setNewWorkspaceDialogOpen] = useState(false);
    const appContext = useContext(AppContext) as AppContextState;
    const [selectedTab, setSelectedTab] = useState(0);
    const [files, setFiles] = useState<Array<RepoFile>>();
    
    
    const dispatch = useDispatch();

    function loadReadmeContent(projectId: string, branchId: string, fileId: string)
    {
        appContext.balsamApi.projectApi.getFile(projectId, branchId, fileId)
        .then((response) => 
        {   
            setReadmeMarkdown(response.data);
        })
        .catch( () => {
            setReadmeMarkdown("Fel vid inläsning av README.md"); //TODO: Language
        });
    }

    const loadFiles = async (projectId: string, branchId: string) => {

        if (branchId === null || typeof branchId === 'undefined') {
            return;
        }

        appContext.balsamApi.projectApi.getFiles(projectId, branchId)
        .catch(() => {
            dispatch(postError("Det gick inte att ladda filer")); //TODO: Language
        })
        .then(async (response) => {
            
            //TODO: Response should be typed, change OpenApi spec
            let axResponse = response as AxiosResponse<any[], any>;

            let files = axResponse.data as Array<RepoFile>;

            setFiles(files);

            let resourceFiles = Resources.getResourceFiles(files);
            let readmeFile = files.find((file) => file.path.toLowerCase() === "readme.md" || file.path.toLowerCase() === "/readme.md");

            if (readmeFile && readmeFile.contentUrl)
            {
                loadReadmeContent(projectId, branchId, readmeFile.contentUrl!);
                resourceFiles.push(readmeFile);
            }

            let resourcesArray = await Resources.convertToResources(resourceFiles, projectId, branchId, async (fileId): Promise<string> => {
                let promise = appContext.balsamApi.projectApi.getFile(projectId, branchId, fileId);
                return (await promise).data;
            });

            setResources(resourcesArray);
        });

    };

    const loadTemplates = async () => {
        appContext.balsamApi.workspaceApi.listTemplates()
        .catch(() => {
            dispatch(postError("Det gick inte att ladda mallar")); //TODO: Language
        })
        .then((response) => {
            setTemplates(response?.data);
        })
    };


    const loadWorkspaces = async (projectId: string, branchId: string) => {
        if (branchId === null || typeof branchId === 'undefined') {
            return;
        }

        appContext.balsamApi.workspaceApi.getWorkspace(projectId, branchId, true)
            .catch(() => {
                dispatch(postError("Det gick inte att ladda bearbetningsmiljöer")); //TODO: Language
            })
            .then((response) => {
                setWorkspaces(response?.data);
            });
    };


    useEffect(() => {

        setLoading(true);
        const fetchData = async () => {
            appContext.balsamApi.projectApi.getProject(id as string)
            .catch(() => {
                
                dispatch(postError("Det gick inte att ladda projektet")); //TODO: Language
            })
            .then((response) => {
                setProject(response?.data);
                setBranches(response?.data.branches);
                setSelectedBranch(response?.data.branches.find((b) => b.isDefault)?.id); 
                setLoading(false);

            });
        };

        fetchData()
            .catch(console.error);

    }, [id]);

    useEffect(() => {
        if (selectedBranch !== undefined)
        {
            (async () =>  {
                await loadFiles(id!, selectedBranch!);
                await loadTemplates();
                await loadWorkspaces(id!, selectedBranch!)
            })();
        }

    }, [selectedBranch]);

    const onNewWorkspaceDialogClosing = () => {

        setNewWorkspaceDialogOpen(false);
        loadWorkspaces(id!, selectedBranch!);
    };

    const deleteWorkspace = (workspaceId: string) => 
    {
        let promise = appContext.balsamApi.workspaceApi.deleteWorkspace(workspaceId);
        promise.catch(() => {
            dispatch(postError("Det gick inte att ta bort bearbetningsmiljö")); //TODO: Language
        })
        promise.then(() => {
            dispatch(postSuccess("Bearbetningsmiljö borttagen.")); //TODO: Language
            loadWorkspaces(id!, selectedBranch!);
        })
    }

    function renderReadme() {
        let readmeElement = readmeMarkdown
            ? <div className="scroll"><MarkdownViewer markdown={readmeMarkdown} /></div>
            : <p><em>ingen readme</em></p>

        return (
            readmeElement
        );
    }

    const selectedBranchChanged = (event: SelectChangeEvent<string>) => {
        setSelectedBranch(event.target.value);
        //TODO: Reload project content
    };

    function renderBranchSelect() {
        var selectBranchesElement;

        if (branches) {
            selectBranchesElement = (<div className="branchSelect">
                <FormControl size="small" variant="standard">
                    <Select
                        id="branch-select"
                        value={selectedBranch}
                        onChange={selectedBranchChanged}
                    >
                        {branches.map(branch => (
                            <MenuItem key={branch.id} value={branch.id}>{branch.name}</MenuItem>
                        ))}

                    </Select>
                </FormControl>
            </div>)
        }

        return selectBranchesElement;
    }

    const handleClickOpen = () => {
        setNewWorkspaceDialogOpen(true);
    };

    function renderNewWorkspaceDialog()
    {
        if (project && templates && selectedBranch)
        {
            return (<NewWorkspaceDialog project={project!} open={newWorkspaceDialogOpen} selectedBranchId={selectedBranch!} templates={templates!} onClosing={onNewWorkspaceDialogClosing}></NewWorkspaceDialog>)
        }
    
        return "";
    }
    
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

    function renderProject(project: Project)
    {
        let readmeElement = renderReadme();
        let branchSelect = renderBranchSelect();
        let newWorkspaceDialog = renderNewWorkspaceDialog();
        let filesElement = renderFilesTree();

        function tabProps(index: number) {
            return {
                id: `simple-tab-${index}`,
                'aria-controls': `simple-tabpanel-${index}`,
            };
        }

        return (
            <div>
                <div className="project-header">
                    <div><h2>{project.name}</h2></div>
                    <div className="git-box">
                        <div className="git-box-content">
                            {branchSelect}
                            {/* <Button component={Link as any} target="_blank" underline="hover" to={project.url}>Git<OpenInNew fontSize="inherit" /></Button> */}
                        </div>
                    </div>
                </div>
                <Accordion defaultExpanded >
                    <Box sx={{ width: '100%' }}>
                        <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
                            <Tabs value={selectedTab} onChange={handleTabChange} aria-label="Tabbar för filer och resurser">
                                <Tab label="README.md" {...tabProps(0)} />
                                <Tab label="Resurser" {...tabProps(1)} />
                                <Tab label="Filer" {...tabProps(2)} />
                            </Tabs>
                        </Box>
                        <CustomTabPanel value={selectedTab} index={0}>
                            {readmeElement}
                        </CustomTabPanel>
                        <CustomTabPanel value={selectedTab} index={1}>
                            <ResourcesSection projectid={project.id} branch={selectedBranch!} resources={resources} />
                        </CustomTabPanel>
                        <CustomTabPanel value={selectedTab} index={2}>
                            {filesElement}
                        </CustomTabPanel>
                    </Box>
                </Accordion>  

                <Accordion defaultExpanded >
                    <AccordionSummary 
                        expandIcon={<ExpandMoreIcon />}
                        aria-controls="panel1a-content"
                        id="workspace-accordion-summary"
                        >
                        Bearbetningsmiljöer            
                    </AccordionSummary>
                    <Divider></Divider>
                    <AccordionDetails>
                        <div className='buttonrow'>
                            <Button variant="contained" onClick={handleClickOpen}>
                                +
                            </Button>
                        </div>
                        <WorkspacesSection projectid={project.id} branch={selectedBranch!} workspaces={workspaces} deleteWorkspaceCallback={deleteWorkspace} templates={templates} />
                        { newWorkspaceDialog }
                    </AccordionDetails>
                </Accordion>
            </div>
            );
    }

    let contents = loading
        ? <p><em>Laddar...</em></p>
        : renderProject(project as Project);

    return (
        <div>
            {contents}
        </div>
    ); 

}