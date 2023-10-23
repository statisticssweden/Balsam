import { useState, useEffect, useContext } from 'react';
import './MyPage.css';
import { useDispatch } from 'react-redux';
import { postError, postSuccess } from '../Alerts/alertsSlice';
import { Branch, Project, Template, Workspace} from '../services/BalsamAPIServices'
import AppContext, { AppContextState } from '../configuration/AppContext';
import { Accordion, AccordionDetails, AccordionSummary, Chip, Divider } from '@mui/material';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import WorkspacesSection, { NewWorkspaceCardKeyType } from '../WorkspacesSection/WorkspacesSection';
import NewWorkspaceDialog, { NewWorkspaceDialogProperties } from '../NewWorkspaceDialog/NewWorkspaceDialog';


export default function MyPage() {

    const [projects, setProjects] = useState<Array<Project>>();
    const [loading, setLoading] = useState(true);
    const dispatch = useDispatch();
    const appContext = useContext(AppContext) as AppContextState;
    const [workspaces, setWorkspaces] = useState<Array<Workspace>>();
    const [templates, setTemplates] = useState<Array<Template>>();
    const [newWorkspaceDialogProperties, setNewWorkspaceDialogProperties] = useState<NewWorkspaceDialogProperties>();
    
    const loadProjects = () =>
    {
        setLoading(true);

        const fetchData = async () => {
            let promise = appContext.balsamApi.projectApi.listProjects(false);
            promise.catch(() => {
                
                dispatch(postError("Det gick inte att ladda projekt")); //TODO: Language
            })

            let listProjectsResponse = await promise;
            
            setProjects(listProjectsResponse.data.projects);
            setLoading(false);
        }

        fetchData()
            .catch(console.error);
        
        

    }

    const loadWorkspaces = async () => {

        let promise = appContext.balsamApi.workspaceApi.getWorkspace(undefined, undefined, false);
        promise.catch(() => {
            dispatch(postError("Det gick inte att ladda bearbetningsmiljöer")); //TODO: Language
        })
        let response = await promise;
        setWorkspaces(response.data);
    };

    useEffect(() => {
        (async () =>  {
            loadProjects();
            await loadTemplates();
            loadWorkspaces();
        })();
    }, [])

   
    const deleteWorkspace = (workspaceId: string) => 
    {
        let promise = appContext.balsamApi.workspaceApi.deleteWorkspace(workspaceId);
        promise.catch(() => {
            dispatch(postError("Det gick inte att ta bort bearbetningsmiljö")); //TODO: Language
        })
        promise.then(() => {
            dispatch(postSuccess("Bearbetningsmiljö borttagen.")); //TODO: Language
            
            //Faster than reloading all
            if (workspaces){
                setWorkspaces(workspaces.filter( w => w.id !== workspaceId));
            }
        })
    }
    
    const loadTemplates = async () => {
        let promise = appContext.balsamApi.workspaceApi.listTemplates();
        promise.catch(() => {
            dispatch(postError("Det gick inte att ladda mallar")); //TODO: Language
        })
        let response = await promise;
        setTemplates(response.data);
    };

    const onNewWorkspaceDialogClosing = () => {
        
        if (newWorkspaceDialogProperties)
        {
            setNewWorkspaceDialogProperties( { 
                ...newWorkspaceDialogProperties, 
                open: false}
            )
        }

        loadWorkspaces();
    };

    function renderNewWorkspaceDialog()
    {
        if (newWorkspaceDialogProperties !== undefined)
        {
            return (<NewWorkspaceDialog 
                project={newWorkspaceDialogProperties.project} 
                selectedBranchId={newWorkspaceDialogProperties.selectedBranchId} 
                templates={newWorkspaceDialogProperties.templates} 
                onClosing={onNewWorkspaceDialogClosing} //Cannot use props.onClosing because then newWorkspaceDialogProperties will be undefined when onClosing is called because of closure
                open={newWorkspaceDialogProperties.open}></NewWorkspaceDialog>)
        }
    
        return "";
    }

    function onNewWorkspaceClick(itemKey: NewWorkspaceCardKeyType) {

        let project = projects?.find(p => p.id === itemKey.projectId);

        if (project && templates)
        {
            let props: NewWorkspaceDialogProperties = {
                project: project,
                selectedBranchId: itemKey.branchId,
                templates: templates,
                onClosing: onNewWorkspaceDialogClosing,
                open: true,
            }

            setNewWorkspaceDialogProperties(props);
        } 
    }

    function renderBranchContent(projectId: string, branch: Branch)
    {
        let branchWorkspaces: Array<Workspace> = [];

        if (workspaces)
        {
            branchWorkspaces = workspaces.filter((w) => w.branchId === branch.id && w.projectId === projectId)
        }

        return (
            <div className='branchWorkspaceContainer' key={projectId + "_" + branch.id }>
                <Divider sx={{mb:"10px"}}>
                    <Chip label={"Branch: " + branch.name} color='info' variant='outlined' />
                </Divider>

                <WorkspacesSection projectid={projectId} 
                    branch={branch.id} 
                    deleteWorkspaceCallback={deleteWorkspace} 
                    showNewCard={true} 
                    workspaces={branchWorkspaces} 
                    templates={templates}
                    onNewClick={onNewWorkspaceClick} ></WorkspacesSection>
                
            </div>
        );

    }

    function renderProjectsTable(projs: Array<Project>) {
        return (
            <div aria-labelledby="tabelLabel">
                {
                    projs.map((project) => {
                       return (
                        <Accordion defaultExpanded key={project.id}>
                            <AccordionSummary 
                                expandIcon={<ExpandMoreIcon />}
                                aria-controls="panel1a-content"
                                id="panel1a-header">
                                    
                                     {/* <Typography variant="h3" color="primary" >{project.name}</Typography> */}
                                     <h3 className='projectHeader'>{project.name}</h3>
                                     
                                     <div>
                                     <p className="description"><em>{project.description}</em></p>
                                     </div>
                                     
                            </AccordionSummary>
                            <AccordionDetails>
                                
                                {project.branches.map((branch) => {
                                    return renderBranchContent(project.id, branch)
                                })}
                            </AccordionDetails>
                        </Accordion>
                       )
                    })
                }


            </div>
        );
    }

    let newWorkspaceDialog = renderNewWorkspaceDialog();

    let contents = loading
        ? <p><em>Laddar...</em></p>
        : renderProjectsTable(projects as Array<Project>);

    return (
        <div>
            <h2 id="tabelLabel">Mina Projekt och Undersökningar</h2>
            {contents}
            {newWorkspaceDialog}
        </div>
        );

}