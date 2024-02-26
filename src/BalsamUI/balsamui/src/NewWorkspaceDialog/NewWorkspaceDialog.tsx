import {useState, Fragment, useEffect, useContext} from 'react';
import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import FormControl from '@mui/material/FormControl';
import { Box, CircularProgress, InputLabel, MenuItem, Select, TextField } from '@mui/material';
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
    onWorkspaceCreated?: (workspaceId: string) => void,
    open: boolean,
}

export default function NewWorkspaceDialog(props: NewWorkspaceDialogProperties ) {
    const [workspaceName, setWorkspaceName] = useState<string>("");
    const [workspaceNameError, setWorkspaceNameError] = useState(false);
    const [workspaceNameHelperText, setWorkspaceNameHelperText] = useState("")
    const [templateId, setTemplateId] = useState<string>();
    const [branchName, setBranchName] = useState<string>();
    const [busy, setBusy] = useState(false);
    const [okEnabled, setOkEnabled] = useState(false);
    const dispatch = useDispatch();

    const appContext = useContext(AppContext) as AppContextState;

    useEffect(() => {
        setTemplateId( props.templates[0].id );
    }, [props.templates]);

    useEffect(() => {
        setBranchName(props.project.branches.find((b) => b.id === props.selectedBranchId)?.name)

    }, [props.selectedBranchId]);


    useEffect(() => {
        updateOkEnabled(workspaceName, busy);

    }, [workspaceName, busy]);

    const updateOkEnabled = (workspaceName: string, busy: boolean) => 
    {
        let projectNameValid = validateWorkspaceName(workspaceName).length === 0;
        setOkEnabled(projectNameValid && !busy)
    }

    const showNewItemCreatedAlert = (message: string, workspaceUrl: string) => 
    {
        dispatch(postSuccess(message, {caption: "Öppna", href: workspaceUrl, target: "_blank"} )); //TODO: Language
    }

    const resetDialog = () => {
        setWorkspaceName("");
        setWorkspaceNameError(false);
        setWorkspaceNameHelperText("");
        setBusy(false);
    };

    const handleCancel = () => {
        props.onClosing();
        resetDialog();
    };

    const handleClose = () => {
        props.onClosing();
        resetDialog();
    };

    const workspaceNameChanged = (name: string) => {
        //name = name.trim();
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
            name: workspaceName.trim(),
            projectId: props.project.id,
            branchId: props.selectedBranchId,
            templateId: templateId!,
        }
        setBusy(true);
        updateOkEnabled(workspaceName, true);
        appContext.balsamApi.workspaceApi.createWorkspace(workspace).then((response => {
            let workspaceUrl = response.data.url;
            
            props.onWorkspaceCreated?.(response.data.id);
            props.onClosing();
            resetDialog();

            showNewItemCreatedAlert(`Bearbetningsmiljön "${response.data.name}" är skapad. Det kan ta en liten stund innan den är redo att öppnas.`, workspaceUrl); //TODO: Language
        }), () => {
            dispatch(postError("Det gick inte att skapa bearbetningsmiljön " + workspaceName)); //TODO: Language
            setBusy(false);
        });
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

    const progress = renderProgress();

    function renderDialog()
    {
        return (
            <Fragment>
                <Dialog
                    open={props.open}
                    onClose={handleClose}
                    fullWidth={true}
                    disableRestoreFocus 
                >
                    <DialogTitle>Skapa bearbetningsmiljö
                        {progress}
                    </DialogTitle>
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

    return templateId ? renderDialog() : "";
}
