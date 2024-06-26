import {useState, Fragment, useEffect, useContext} from 'react';
import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import FormControl from '@mui/material/FormControl';
import { Box, CircularProgress, TextField } from '@mui/material';
import { useDispatch } from 'react-redux';
import { postSuccess, postError} from '../Alerts/alertsSlice';
import { CreateProjectRequest, ProjectCreatedResponse } from '../services/BalsamAPIServices'
import AppContext, { AppContextState } from '../configuration/AppContext';
import { RepositoryTemplate } from '../Model/RepositoryTemplate';
import RepositoryTemplateAsyncAutocomplete from '../RepositoryTemplatesAsyncAutocomplete/RepositoryTemplatesAsyncAutocomplete';
import KnowledgeLibraries from '../KnowledgeLibraries/KnowledgeLibraries';
import './NewProjectDialog.css'

export interface NewProjectDialogProperties
{
    onClosing: () => void,
    open: boolean,
    defaultTemplate?: RepositoryTemplate
}

export default function NewProjectDialog(props: NewProjectDialogProperties ) {
    const appContext = useContext(AppContext) as AppContextState;

    const [projectName, setProjectName] = useState("");
    const [projectDescription, setProjectDescription] = useState("");
    const [branchName, setBranchName] = useState(appContext.config.defaultGitBranchName);
    const [projectNameError, setProjectNameError] = useState(false);
    const [projectNameHelperText, setProjectNameHelperText] = useState("")
    const [branchNameError, setBranchNameError] = useState(false);
    const [branchNameHelperText, setBranchNameHelperText] = useState("");
    const [template, setTemplate] = useState<RepositoryTemplate | null>(null);
    
    const [busy, setBusy] = useState(false);
    const [okEnabled, setOkEnabled] = useState(false);
    
    const dispatch = useDispatch();

    useEffect(() => {
        updateOkEnabled(projectName, branchName, busy);

    }, [branchName, projectName, busy]);

    useEffect(() => {
        setTemplate(props.defaultTemplate ?? null);
    }, [props.defaultTemplate])

    const updateOkEnabled = (projectName: string, branchName: string, busy: boolean) => 
    {
        let projectNameValid = validateProjectName(projectName).length == 0;
        let branchNameValid = validateBranchName(branchName).length == 0;

        setOkEnabled(projectNameValid && branchNameValid && !busy)
    }

    const showNewItemCreatedAlert = (message: string, id: string) => 
    {
        dispatch(postSuccess(message, {caption: "Öppna", href: `/project/${id}`} )); //TODO: Language
    }



    const resetDialog = () => {
        setProjectName("");
        setProjectNameError(false);
        setProjectNameHelperText("");
        setProjectDescription("");
        setBranchName(appContext.config.defaultGitBranchName);
        setBranchNameError(false);
        setBranchNameHelperText("");
        setBusy(false);
    };

    const handleCancel = () => {
        //setOpen(false);
        props.onClosing();
        resetDialog();
    };

    const handleClose = () => {
        //setOpen(false);
        props.onClosing();
        resetDialog();
    };

    const projectNameChanged = (name: string) => {
        name = name.trim();
        setProjectName(name);
        let errors = validateProjectName(name);

        if (errors.length > 0)
        {
            let errorMessage : string = "";
            errors.map((error) => {
                errorMessage = errorMessage + " " + error;
            })

            errorMessage = errorMessage.trim();

            setProjectNameError(true);
            setProjectNameHelperText(errorMessage);
        }
        else{
            setProjectNameError(false);
            setProjectNameHelperText("");
        }
    }

    const projectDescriptionChanged = (description: string) => {
        setProjectDescription(description);
    }

    const branchNameChanged = (name: string) => {
        name = name.trim();
        setBranchName(name);
        let errors = validateBranchName(name);

        if (errors.length > 0)
        {
            let errorMessage : string = "";
            errors.map((error) => {
                errorMessage = errorMessage + " " + error;
            })

            errorMessage = errorMessage.trim();

            setBranchNameError(true);
            setBranchNameHelperText(errorMessage);
        }
        else{
            setBranchNameError(false);
            setBranchNameHelperText("");
        }
    }

    const validateProjectName = (name: string) : Array<string> => {
        let errors : Array<string> = [];

        if(name === "")
        {
            errors.push("Projektet måste ha ett namn"); //TODO: Language
        }

        return errors;
    };

    const validateBranchName = (name: string) : Array<string> => {
        let errors : Array<string> = [];

        if(name === "")
        {
            errors.push("Defaultbranchen måste ha ett namn"); //TODO: Language
        }

        return errors;
    };

    function renderProgress()
    {
        if(busy)
        {
            return (<CircularProgress size="18px" sx={{marginLeft:"6px"}} />)
        }
        else 
        {
            return "";
        }
    }

    const onCreated = (response: ProjectCreatedResponse) => 
    {
        appContext.refreshToken(); //Update the users user groups
        //setOpen(false);
        props.onClosing();
        resetDialog();
        showNewItemCreatedAlert(`Projekt "${response.name}" är skapat.`, response.id); //TODO: Language
    }

    const handleCreate = () => {
        let sourceLocation = template !== null ? template.git : null;

        let project : CreateProjectRequest = {
            name: projectName,
            description: projectDescription,
            branchName: branchName,
            sourceLocation: sourceLocation
        }
        setBusy(true);
        updateOkEnabled(projectName, branchName, true);
       
        appContext.balsamApi.projectApi.createProject(project).then((response => {
            onCreated(response.data);
        }), () => {
                
            dispatch(postError("Det gick inte att skapa projektet")); //TODO: Language
            setBusy(false);
        });    
           
    };

    function onTemplateChanged(template: RepositoryTemplate | null)
    {
        setTemplate(template);
    }

    async function getTemplates() : Promise<Array<RepositoryTemplate>>
    {
        return KnowledgeLibraries.getAllTemplates(appContext.balsamApi.knowledgeLibraryApi);
    }

    function renderCopyProjectInformation()
    {
        if(template)
        {
            return <Box className='form-control-info-text'>
                Det kan ta någon minut innan filerna har kopierats till projektet när det har skapats. Uppdatera i så fall projektsidan efter att har navigerat tills de dyker upp.
            </Box>
        }
        else
        {
            return undefined;
        }

    }

    const progress = renderProgress();

    const copyProjectInformation = renderCopyProjectInformation();

    return (
        <Fragment>
            {/* TODO: seperate button from dialog */}
            
            
            <Dialog
                open={props.open}
                onClose={handleClose}
                fullWidth={true}
            >
                <DialogTitle>Skapa projekt
                    {progress}
                </DialogTitle>
                <DialogContent>
                
                    <DialogContentText>
                        Ange information om projektet
                    </DialogContentText>
                    <Box
                        noValidate
                        sx={{
                            display:'flex',
                            flexDirection: 'column',
                            m:'auto'
                        }}
                        component='form'>
                        {/* TODO: Language */}
                        <FormControl sx={{ mt: 4}}>
                            <TextField id="name-input" error={projectNameError} helperText={projectNameHelperText} label="Namn" variant='standard' required value={projectName} onChange={e => projectNameChanged(e.target.value)} aria-describedby="Projektets namn" />
                        </FormControl>
                        <FormControl sx={{ mt: 4}}>
                            <TextField id="description-input" variant='standard' label="Beskrivning" value={projectDescription} onChange={e => projectDescriptionChanged(e.target.value)} aria-describedby="Beskrivning av projekt" />
                        </FormControl>
                        <FormControl sx={{ mt: 4}}>
                            <TextField id="branch-input" error={branchNameError} helperText={branchNameHelperText} variant='standard' label="Branchnamn" required value={branchName} onChange={e => branchNameChanged(e.target.value)} aria-describedby="Namn på defaultbranch" />
                        </FormControl>
                        <FormControl sx={{ mt: 4}}>
                            <RepositoryTemplateAsyncAutocomplete 
                                getTemplates={getTemplates}
                                label="Skapa från Mall eller Exempel"
                                onChange={onTemplateChanged}
                                defaultTemplate={props.defaultTemplate}
                            />
                            {copyProjectInformation}
                        </FormControl>
                    </Box>
                    
                </DialogContent>
                
                <DialogActions>
                    
                    <Button onClick={handleCancel} >Avbryt</Button>
                    <Button onClick={handleCreate} disabled={!okEnabled}>Skapa</Button>
                    
                </DialogActions>      
                
            </Dialog>
        </Fragment>
    );
}
