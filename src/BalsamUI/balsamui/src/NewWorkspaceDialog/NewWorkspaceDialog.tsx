import {useState, Fragment, useEffect, useContext, useRef} from 'react';
import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import FormControl from '@mui/material/FormControl';
import { Box, InputLabel, MenuItem, Select, TextField } from '@mui/material';
import { useDispatch } from 'react-redux';
import { postSuccess, postError} from '../Alerts/alertsSlice';
import { CreateWorkspaceRequest, Project, Template } from '../services/BalsamAPIServices'
import AppContext, { AppContextState } from '../configuration/AppContext';


export interface NewWorkspaceDialogProperties
{
    project: Project,
    selectedBranchId: string,
    templates: Array<Template>,
    onClosing: () => void,
}

export default function NewProjectDialog(props: NewWorkspaceDialogProperties ) {
    const [open, setOpen] = useState(false);
    const [workspaceName, setWorkspaceName] = useState<string>("");
    const [workspaceNameError, setWorkspaceNameError] = useState(false);
    const [workspaceNameHelperText, setWorkspaceNameHelperText] = useState("")

    const [templateId, setTemplateId] = useState<string>();
    const [branchName, setBranchName] = useState<string>();

    const [okEnabled, setOkEnabled] = useState(false);
    const dispatch = useDispatch();

    const appContext = useContext(AppContext) as AppContextState;

    // const nameInputRef = useRef();
    
    // useEffect(() => {
    //     nameInputRef!.current.focus();
    // }, []);
    
    useEffect(() => {
        setTemplateId( props.templates[0].id );
    }, []);

    useEffect(() => {
        setBranchName(props.project.branches.find((b) => b.id === props.selectedBranchId)?.name)

    }, [props.selectedBranchId]);


    useEffect(() => {
        updateOkEnabled(workspaceName);

    }, [workspaceName]);

    const updateOkEnabled = (workspaceName: string) => 
    {
        let projectNameValid = validateWorkspaceName(workspaceName).length == 0;
        setOkEnabled(projectNameValid)

    }

    const showNewItemCreatedAlert = (message: string, workspaceUrl: string) => 
    {
        dispatch(postSuccess(message, {caption: "Öppna", href: workspaceUrl} )); //TODO: Language
    }

    const handleClickOpen = () => {
        setOpen(true);
    };

    const resetDialog = () => {
        setWorkspaceName("");
        setWorkspaceNameError(false);
        setWorkspaceNameHelperText("");
    };

    const handleCancel = () => {
        setOpen(false);
        props.onClosing();
        resetDialog();
    };

    const handleClose = () => {
        setOpen(false);
        props.onClosing();
        resetDialog();
    };

    const workspaceNameChanged = (name: string) => {
        name = name.trim();
        setWorkspaceName(name);
        let errors = validateWorkspaceName(name);

        if (errors.length > 0)
        {
            let errorMessage : string = "";
            errors.map((error) => {
                errorMessage = errorMessage + " " + error;
            })

            errorMessage = errorMessage.trim();

            setWorkspaceNameError(true);
            setWorkspaceNameHelperText(errorMessage);
        }
        else{
            setWorkspaceNameError(false);
            setWorkspaceNameHelperText("");
        }
    }

    const validateWorkspaceName = (name: string) : Array<string> => {
        let errors : Array<string> = [];

        if(name === "")
        {
            errors.push("Bearbetningsmiljön måste ha ett namn"); //TODO: Language
        }

        return errors;
    };

    const handleCreate = () => {

        let workspace : CreateWorkspaceRequest = {
            name: workspaceName,
            projectId: props.project.id,
            branchId: props.selectedBranchId,
            templateId: templateId!,
        }

        appContext.balsamApi.workspaceApi.createWorkspace(workspace).then((response => {
            
            setOpen(false);
            props.onClosing();
            resetDialog();

            //TODO: Set url to response url when it is generated on response object
            let workspaceUrl = "";

            showNewItemCreatedAlert(`Bearbetningsmiljön "${response.data.name}" är skapad`, workspaceUrl); //TODO: Language
        }), () => {
                
            dispatch(postError("Det gick inte att skapa bearbetningsmiljön " + workspaceName)); //TODO: Language

        });
    };

    return (
        <Fragment>
            {/* TODO: seperate button from dialog */}
            <Button variant="contained" onClick={handleClickOpen}>
                +
            </Button>
            
            <Dialog
                open={open}
                onClose={handleClose}
                fullWidth={true}
                disableRestoreFocus 
            >
                <DialogTitle>Skapa bearbetningsmiljö</DialogTitle>
                <DialogContent>
                    <DialogContentText>
                        Ange information om bearbetningsmiljön
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
                            <TextField id="project-name" disabled  variant='standard' label="Projekt" value={props.project.name} aria-describedby="Namn på projekt" />
                        </FormControl>
                        <FormControl sx={{ mt: 4}}>
                            <TextField id="branch-name" disabled  variant='standard' label="Branch" value={branchName} aria-describedby="Namn på branch" />
                        </FormControl>
                        <FormControl sx={{ mt: 4}}>
                            <TextField id="name-input" autoFocus inputRef={(input) => input && input.focus()} error={workspaceNameError} helperText={workspaceNameHelperText} label="Namn på bearbetningsmiljö" variant='standard' required value={workspaceName} onChange={e => workspaceNameChanged(e.target.value)} aria-describedby="Namn på bearbetningsmiljö" />
                        </FormControl>
                        <FormControl variant="standard" sx={{ mt: 4, minWidth: 120 }}>
                            <InputLabel id="template-select-label">Mall</InputLabel>
                            <Select
                                id="template-select"
                                labelId="template-select-label"
                                value={templateId}
                                variant='standard'
                                onChange={(e) => setTemplateId(e.target.value)}
                                label="Mall"
                                aria-describedby="Mall" 
                                >
                            
                            {props.templates.map(template => (
                                <MenuItem key={template.id} value={template.id}>{template.name}</MenuItem>
                            ))}
                            </Select>
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
