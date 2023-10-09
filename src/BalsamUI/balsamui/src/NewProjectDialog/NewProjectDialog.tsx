import {useState, Fragment, useEffect} from 'react';
import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import FormControl from '@mui/material/FormControl';
import BalsamApiOld from '../services/BalsamAPIServices';
//import { CreateProjectRequest } from '../Model/ApiModels';
import { Box, TextField } from '@mui/material';
import { useDispatch } from 'react-redux';
import { postSuccess, postError} from '../Alerts/alertsSlice';
import BalsamAPI, {CreateProjectRequest} from '../services/BalsamAPIServices'


export interface NewProjectDialogProperties
{
    onClosing: () => void
}

export default function NewProjectDialog(props: NewProjectDialogProperties ) {
    const [open, setOpen] = useState(false);
    const [projectName, setProjectName] = useState("");
    const [projectDescription, setProjectDescription] = useState("");
    const [branchName, setBranchName] = useState("main");
    const [projectNameError, setProjectNameError] = useState(false);
    const [projectNameHelperText, setProjectNameHelperText] = useState("")
    const [branchNameError, setBranchNameError] = useState(false);
    const [branchNameHelperText, setBranchNameHelperText] = useState("")
    const [okEnabled, setOkEnabled] = useState(false);
    const dispatch = useDispatch();

    useEffect(() => {
        updateOkEnabled(projectName, branchName);

    }, [branchName, projectName]);

    const updateOkEnabled = (projectName: string, branchName: string) => 
    {
        let projectNameValid = validateProjectName(projectName).length == 0;
        let branchNameValid = validateBranchName(branchName).length == 0;
        setOkEnabled(projectNameValid && branchNameValid)

    }

    const showNewItemCreatedAlert = (message: string, id: string) => 
    {
        dispatch(postSuccess(message, {caption: "Öppna", href: `/projects/${id}`} ));
    }

    const handleClickOpen = () => {
        setOpen(true);
        
    };

    const handleCancel = () => {
        setOpen(false);
        props.onClosing();
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
            setProjectNameHelperText("")
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
            setBranchNameHelperText("")
        }
    }

    const validateProjectName = (name: string) : Array<string> => {
        let errors : Array<string> = [];

        if(name === "")
        {
            errors.push("Projektet måste ha ett namn");
        }

        return errors;
    };

    const validateBranchName = (name: string) : Array<string> => {
        let errors : Array<string> = [];

        if(name === "")
        {
            errors.push("Defaultbranchen måste ha ett namn");
        }

        return errors;
    };


    const handleCreate = () => {

        let project : CreateProjectRequest = {
            name: projectName,
            description: projectDescription,
            branchName: branchName
        }

        BalsamAPI.projectApi.createProject(project).then((response => {
            
            setOpen(false);
            props.onClosing();
            showNewItemCreatedAlert(`Projekt "${response.data.name}" är skapat`, response.data.id);
            
        }), () => {
                
            dispatch(postError("Det gick inte att skapa projektet"))

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
                onClose={handleCancel}
                fullWidth={true}
            >
                <DialogTitle>Skapa projekt</DialogTitle>
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
                            
                        <FormControl sx={{ mt: 4}}>
                            <TextField id="name-input" error={projectNameError} helperText={projectNameHelperText} label="Namn" variant='standard' required value={projectName} onChange={e => projectNameChanged(e.target.value)} aria-describedby="Projektets namn" />
                        </FormControl>
                        <FormControl sx={{ mt: 4}}>
                            <TextField id="description-input" variant='standard' label="Beskrivning" value={projectDescription} onChange={e => projectDescriptionChanged(e.target.value)} aria-describedby="Beskrivning av projekt" />
                        </FormControl>
                        <FormControl sx={{ mt: 4}}>
                            <TextField id="branch-input" error={branchNameError} helperText={branchNameHelperText} variant='standard' label="Branchnamn" required value={branchName} onChange={e => branchNameChanged(e.target.value)} aria-describedby="Namn på defaultbranch" />
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
