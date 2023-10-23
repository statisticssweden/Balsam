import {Project, Branch, Workspace, Template} from '../services/BalsamAPIServices';
// import Button from '@mui/material/Button';
// import { Link } from 'react-router-dom';
// import { Description, OpenInNew } from '@mui/icons-material';
import MenuItem from '@mui/material/MenuItem';
import Select, { SelectChangeEvent } from '@mui/material/Select';
import FormControl from '@mui/material/FormControl';
import MarkdownViewer from '../MarkdownViewer/MarkdownViewer';
import { useDispatch } from 'react-redux';
import { useState, useEffect, useContext } from 'react';
import { useParams } from 'react-router-dom'
import { postError, postSuccess } from '../Alerts/alertsSlice';
import './ProjectPage.css'
import HttpService from '../services/HttpServices';
import { Resource, ResourceType, getResourceType } from '../Model/Model';
import ResourcesSection from '../ResourceSection/ResourcesSection';
import AppContext, { AppContextState } from '../configuration/AppContext';
import WorkspacesSection from '../WorkspacesSection/WorkspacesSection';
import NewWorkspaceDialog from '../NewWorkspaceDialog/NewWorkspaceDialog';
import { Button } from '@mui/material';


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
    
    // const { branch } = useSearchParams();

    const dispatch = useDispatch();

    const loadFiles = async (projectId: string, branch: string) => {

        if (branch === null || typeof branch === 'undefined') {
            return;
        }

        let promise = appContext.balsamApi.projectApi.getFiles(projectId, branch);
        promise.catch(() => {
            dispatch(postError("Det gick inte att ladda filer")); //TODO: Language
        })
        let response = await promise;
        let files = response.data;

        let readmeFile = files.find((file) => file.path.toLowerCase() === "/readme.md");

        HttpService.getTextFromUrl(readmeFile.contentUrl)
            .then((text) => 
            {   
                setReadmeMarkdown(text);
            })
            .catch( () => {
                setReadmeMarkdown("Fel vid inläsning av README.md"); //TODO: Language
            });

        let resourceFiles = files.filter((file) => {
            return file.path.startsWith('/Resources/') || file.path.toLowerCase() === "/readme.md"
        });

        let resourcesArray = await Promise.all(resourceFiles.map( async (file): Promise<Resource> => {
            let name = file.name;
            name = name.replace(/\.[^/.]+$/, "");
            
            let type = getResourceType(file.name);
            let linkUrl: string = "";
            
            let description = "";
            switch (type) {
                case ResourceType.Md:
                    description = "Markdownfil som går att läsa i gränssnittet"; //TODO: Language
                    break;
                case ResourceType.Url:
                    let content = await HttpService.getTextFromUrl(file.contentUrl);
                    let matches = content.match(/URL\s*=\s*(\S*)/)
                    linkUrl =  matches !== null && matches.length > 0 ? matches[0] : "";
                    description = file.contentUrl;

                    break;
                case ResourceType.Document:
                    description = file.name;
                    break;
                default:
                    description = file.name;
            }


            return { 
                name: name,
                description: description,
                type: type,
                linkUrl: linkUrl,
                contentUrl: file.contentUrl,
                filePath: file.path
                }
        }));

        setResources(resourcesArray);

    };

    const loadTemplates = async () => {
        let promise = appContext.balsamApi.workspaceApi.listTemplates();
        promise.catch(() => {
            dispatch(postError("Det gick inte att ladda mallar")); //TODO: Language
        })
        let response = await promise;
        setTemplates(response.data);
    };


    const loadWorkspaces = async (projectId: string, branchId: string) => {
        if (branchId === null || typeof branchId === 'undefined') {
            return;
        }

        let promise = appContext.balsamApi.workspaceApi.getWorkspace(projectId, branchId, true);
        promise.catch(() => {
            dispatch(postError("Det gick inte att ladda bearbetningsmiljöer")); //TODO: Language
        })
        let response = await promise;
        setWorkspaces(response.data);
    };


    useEffect(() => {

        setLoading(true);
        const fetchData = async () => {
            let promise = appContext.balsamApi.projectApi.getProject(id as string);

            promise.catch(() => {
                
                dispatch(postError("Det gick inte att ladda projektet")); //TODO: Language
            })

            let response = await promise;
            setProject(response.data);
            setBranches(response.data.branches);
            setSelectedBranch(response.data.branches.find((b) => b.isDefault)?.id); 
            setLoading(false);


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
        //TODO: Ladda om projekt
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


    function renderProject(project: Project)
    {
        let readmeElement = renderReadme();
        let branchSelect = renderBranchSelect();
        let newWorkspaceDialog = renderNewWorkspaceDialog();
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
                {readmeElement}
                <h3>Resurser</h3>
                <ResourcesSection projectid={project.id} branch={selectedBranch!} resources={resources} />
                <h3>Bearbetningsmiljöer</h3>
                <div className='buttonrow'>
                    <Button variant="contained" onClick={handleClickOpen}>
                        +
                    </Button>
                </div>
                <WorkspacesSection projectid={project.id} branch={selectedBranch!} workspaces={workspaces} deleteWorkspaceCallback={deleteWorkspace} templates={templates} />
                { newWorkspaceDialog }
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