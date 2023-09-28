import React, {useState, Fragment} from 'react';
import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import FormControl from '@mui/material/FormControl';
//import slugify from 'slugify'
import { Form } from 'react-router-dom';
// import { SxProps } from '@material-ui/system';
import BalsamApi from '../services/BalsamAPIServices';
import { CreateProjectRequest } from '../Model/ApiModels';
import { Box, TextField } from '@mui/material';


export interface NewProjectDialogProperties
{
    onClosing: () => void
}

export default function NewProjectDialog(props: NewProjectDialogProperties ) {
    const [open, setOpen] = useState(false);
    const [projectName, setProjectName] = useState("");
    const [projectDescription, setProjectDescription] = useState("");
    const [branchName, setBranchName] = useState("main");
    const [nameError, setNameError] = useState(false);
    const [nameHelperText, setNameHelperText] = useState("")
    const [branchNameError, setBranchNameError] = useState(false);
    const [branchNameHelperText, setBranchNameHelperText] = useState("")
    


    const showAlert = (message: string) => 
    {
        alert(message);
        // setAlertMessage(message);
        // setSnackbarOpen(true);
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
        if(name === "")
        {
            setNameError(true);
            setNameHelperText("Projektet måste ha ett namn")
            return;
        }
        setNameError(false);
        setNameHelperText("");
        //var safeName = slugify(name);
        //repoNameChanged(safeName);
    }

    const projectDescriptionChanged = (description: string) => {
        setProjectDescription(description);
        //var safeName = slugify(name);
        //repoNameChanged(safeName);
    }

    const branchNameChanged = (name: string) => {
        setBranchName(name);
        if(name === "")
        {
            setBranchNameError(true);
            setBranchNameHelperText("Defaultbranchen måste ha ett namn")
            return;
        }

        setBranchNameError(false);
        setBranchNameHelperText("")
        //var safeName = slugify(name);
        //repoNameChanged(safeName);
    }

    // const repoNameChanged = (name) => {
    //     name = name.toLowerCase();
    //     setRepoName(name);
    //     var safeName = slugify(name);
    //     setUrlSafeName(safeName);
    // };

    const handleCreate = () => {

        let project : CreateProjectRequest = {
            name: projectName,
            description: projectDescription,
            branchName: branchName
        }

        BalsamApi.postProject(project).then((response => {
            
            setOpen(false);
            props.onClosing();
            showAlert(`Projekt "${response.name}" är skapat`);
        }));
    
    };

    return (
        <Fragment>
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
                            <TextField id="name-input" error={nameError} helperText={nameHelperText} label="Namn" variant='standard' required value={projectName} onChange={e => projectNameChanged(e.target.value)} aria-describedby="Projektets namn" />
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
                    <Button onClick={handleCancel}>Avbryt</Button>
                    <Button onClick={handleCreate}>Skapa</Button>
                </DialogActions>
            </Dialog>
        </Fragment>
    );
}
