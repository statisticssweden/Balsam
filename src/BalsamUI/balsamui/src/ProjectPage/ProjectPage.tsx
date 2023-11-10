import {Project, Branch, Workspace, Template, RepoFile} from '../services/BalsamAPIServices';
import MenuItem from '@mui/material/MenuItem';
import Select, { SelectChangeEvent } from '@mui/material/Select';
import FormControl from '@mui/material/FormControl';
import MarkdownViewer from '../MarkdownViewer/MarkdownViewer';
import { useDispatch } from 'react-redux';
import { useState, useEffect, useContext, Fragment } from 'react';
import { useParams, useSearchParams } from 'react-router-dom'
import { postError, postSuccess } from '../Alerts/alertsSlice';
import './ProjectPage.css'
import { Resource } from '../Model/Model';
import ResourcesSection from '../ResourceSection/ResourcesSection';
import AppContext, { AppContextState } from '../configuration/AppContext';
import WorkspacesSection from '../WorkspacesSection/WorkspacesSection';
import NewWorkspaceDialog from '../NewWorkspaceDialog/NewWorkspaceDialog';
import { Accordion, AccordionDetails, AccordionSummary, Box, Button, Divider, IconButton, Tab, Tabs } from '@mui/material';
import { AxiosResponse } from 'axios';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import OpenInNew from '@mui/icons-material/OpenInNew';
import AddCircleIcon from '@mui/icons-material/AddCircle';

import CustomTabPanel from '../CustomTabPanel/CustomTabPanel';
import FileTree, { convertToFileTreeNodes, getAllIds } from '../FileTree/FileTree';
import Resources from '../Resources/Resources';
import { Link } from 'react-router-dom';
import NewBranchDialog from '../NewBranchDialog/NewBranchDialog';

export default function ProjectPage() {
    const [project, setProject] = useState<Project>();
    const [loading, setLoading] = useState(true);
    const { id } = useParams<string>();
    const [branches, setBranches] = useState<Array<Branch>>();
    const [selectedBranch, setSelectedBranch] = useState<string>();
    const [canCreateBranch, setCanCreateBranch] = useState<boolean>(false);
    const [canCreateWorkspace, setCanCreateWorkspace] = useState<boolean>(false);
    const [readmeMarkdown, setReadmeMarkdown] = useState<string>();
    const [resources, setResources] = useState<Array<Resource>>();
    const [workspaces, setWorkspaces] = useState<Array<Workspace>>();
    const [templates, setTemplates] = useState<Array<Template>>();
    const [newWorkspaceDialogOpen, setNewWorkspaceDialogOpen] = useState(false);
    const [newBranchDialogOpen, setNewBranchDialogOpen] = useState(false);
    const appContext = useContext(AppContext) as AppContextState;
    const [selectedTab, setSelectedTab] = useState(0);
    const [files, setFiles] = useState<Array<RepoFile>>();
    const [searchParams, setSearchParams] = useSearchParams();
    
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

    const loadFiles = (projectId: string, branchId: string) => {

        if (branchId === null || typeof branchId === 'undefined') {
            return;
        }

        appContext.balsamApi.projectApi.getFiles(projectId, branchId)
        .catch(() => {
            dispatch(postError("Det gick inte att ladda filer")); //TODO: Language
        })
        .then(async (response) => {
            
            let axResponse = response as AxiosResponse<RepoFile[], any>;

            let files = axResponse.data;

            setFiles(files);

            let resourceFiles = Resources.getResourceFiles(files);
            let readmeFile = files.find((file) => file.path.toLowerCase() === "readme.md");

            if (readmeFile && readmeFile.id)
            {
                loadReadmeContent(projectId, branchId, readmeFile.id);
            }

            let resourcesArray = await Resources.convertToResources(resourceFiles, projectId, branchId, async (fileId): Promise<string> => {
                let promise = appContext.balsamApi.projectApi.getFile(projectId, branchId, fileId);
                return (await promise).data;
            });

            setResources(resourcesArray);
        });

    };

    const loadTemplates = () => {
        appContext.balsamApi.workspaceApi.listTemplates()
        .catch(() => {
            dispatch(postError("Det gick inte att ladda mallar")); //TODO: Language
        })
        .then((response) => {
            setTemplates(response?.data);
        })
    };


    const loadWorkspaces = (projectId: string, branchId: string) => {
        if (branchId === null || typeof branchId === 'undefined') {
            return;
        }

        appContext.balsamApi.workspaceApi.listWorkspaces(projectId, branchId, true)
            .catch(() => {
                dispatch(postError("Det gick inte att ladda bearbetningsmiljöer")); //TODO: Language
            })
            .then((response) => {
                setWorkspaces(response?.data);
            });
    };


    useEffect(() => {

        setLoading(true);
        
        appContext.balsamApi.projectApi.getProject(id as string)
        .catch(() => {
            
            dispatch(postError("Det gick inte att ladda projektet")); //TODO: Language
        })
        .then((response) => {
            if (response && response.data)
            {
                let project = response.data;
                let isProjectGroupMember = appContext.getUserGroups().findIndex(g => g === project.authGroup) >= 0;
                
                setProject(project);
                setBranches(project.branches);

                let branchId = searchParams.get("branchId");
                //let branchId = null;
                
                if (project.branches.findIndex((b) => b.id === branchId) < 0 )
                {
                    branchId = null;
                }

                if(branchId === null || branchId.length === 0)
                {
                    let defaultBranch = project.branches.find((b) => b.isDefault)?.id;
                    if(defaultBranch)
                    {
                        branchId = defaultBranch
                    }
                    else 
                    {
                        branchId = project.branches[0].id
                    }
                }

                setSelectedBranch(branchId);
                setCanCreateBranch(isProjectGroupMember);
                setCanCreateWorkspace(isProjectGroupMember);
                setLoading(false);
            }

        });

    }, [id]);

    function updateBranchIdSearchParam(selectedBranch: string | undefined)
    {
        if(selectedBranch !== undefined)
        {
            let currentSearchParamBranchid = searchParams.get("branchId");
            if (currentSearchParamBranchid !== selectedBranch)
            {
                searchParams.set("branchId", selectedBranch );
                setSearchParams(searchParams);
            }
        }
    }

    useEffect(() => {
        
        updateBranchIdSearchParam(selectedBranch);
        
        if (project !== undefined && selectedBranch !== undefined)
        {
            loadFiles(id!, selectedBranch!);
            loadTemplates();
            loadWorkspaces(id!, selectedBranch!)
        }

    }, [selectedBranch]);

    const onNewWorkspaceDialogClosing = () => {

        setNewWorkspaceDialogOpen(false);
        loadWorkspaces(id!, selectedBranch!);
    };

    const onNewBranchDialogClosing = () => {

        setNewBranchDialogOpen(false);
        
    };

    const deleteWorkspace = (projectId: string, branchId: string, workspaceId: string) => 
    {
        let promise = appContext.balsamApi.workspaceApi.deleteWorkspace(projectId, branchId, workspaceId);
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
    };

    function handleNewBranchClick()
    {
        setNewBranchDialogOpen(true);
    }

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

    function handleBranchCreated(branchId: string): void {
        appContext.balsamApi.projectApi.getProject(id as string)
        .catch(() => {
            
            dispatch(postError("Det gick inte att uppdatera brancher")); //TODO: Language
        })
        .then((response) => {
            if (response && response.data)
            {
                let project = response.data;
                
                setBranches(project.branches);
                if (project.branches.findIndex((b) => b.id === branchId) >= 0 )
                {
                    setSelectedBranch(branchId);
                }
                else
                {
                    dispatch(postError("Det gick inte välja nyskapad branch")); //TODO: Language
                }
            }
        });
    }

    const handleNewWorkspaceClick = () => {
        setNewWorkspaceDialogOpen(true);
    };

    function renderNewBranchDialog()
    {
        if (project && canCreateBranch)
        {
            

            return (
            <Fragment>
                <NewBranchDialog project={project!} open={newBranchDialogOpen} onBranchCreated={handleBranchCreated} onClosing={onNewBranchDialogClosing}></NewBranchDialog>
                <IconButton aria-label="Lägg till branch" sx={{padding:"4px"}} color="primary" onClick={handleNewBranchClick}>
                    <AddCircleIcon />
                </IconButton>
            </Fragment>);
        }
    
        return "";
    }

    function renderNewWorkspaceDialog()
    {
        if (project && templates && selectedBranch && canCreateWorkspace)
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

    function renderNewWorkspaceButton()
    {
        if (canCreateWorkspace)
        {
            return (<div className='buttonrow'>
                    <Button variant="contained" onClick={handleNewWorkspaceClick}>
                        +
                    </Button>
                </div>)
        }
    }
    
    function renderProject(project: Project)
    {
        let readmeElement = renderReadme();
        let branchSelect = renderBranchSelect();
        let newWorkspaceDialog = renderNewWorkspaceDialog();
        let newBranchDialog = renderNewBranchDialog();
        let filesElement = renderFilesTree();
        let newWorkspaceButton = renderNewWorkspaceButton();

        function tabProps(index: number) {
            return {
                id: `project-tab-${index}`,
                'aria-controls': `project-tabpanel-${index}`,
            };
        }

        return (
            <div>
                <div className="project-header">
                    <div><h2>{project.name}</h2></div>
                    <div className="git-box">
                        <div className="git-box-content">
                            {branchSelect}
                            {newBranchDialog}
                            <Button component={Link as any} target="_blank" underline="hover" to={project.gitUrl}>Git<OpenInNew fontSize="inherit" /></Button>
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
                            <ResourcesSection projectid={project.id} branchId={selectedBranch!} resources={resources} />
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
                        {newWorkspaceButton}
                        <WorkspacesSection projectid={project.id} userName={appContext.getUserName()} branch={selectedBranch!} workspaces={workspaces} deleteWorkspaceCallback={deleteWorkspace} templates={templates} />
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