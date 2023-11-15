import { useDispatch } from "react-redux";
import { CreateBranchRequest, Project } from "../services/BalsamAPIServices";
import { Fragment, useContext, useEffect, useState } from "react";
import { Box, Button, CircularProgress, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle, FormControl, InputLabel, MenuItem, Select, TextField } from "@mui/material";
import AppContext, { AppContextState } from "../configuration/AppContext";
import { postError, postSuccess } from "../Alerts/alertsSlice";


export interface NewBranchDialogProperties
{
    project: Project,
    onClosing: () => void,
    onBranchCreated?: (branchId: string) => void,
    open: boolean,
}

export default function NewBranchDialog(props: NewBranchDialogProperties)
{
    const [branchName, setBranchName] = useState<string>("");
    const [branchNameError, setBranchNameError] = useState(false);
    const [branchNameHelperText, setBranchNameHelperText] = useState("")
    const [branchDescription, setBranchDescription] = useState<string>("");
    const [branchDescriptionError, setBranchDescriptionError] = useState(false);
    const [branchDescriptionHelperText, setBranchDescriptionHelperText] = useState("")
    const [busy, setBusy] = useState(false);
    const [fromBranchId, setFromBranchId] = useState<string>("");
    const [okEnabled, setOkEnabled] = useState(false);
    const dispatch = useDispatch();

    const appContext = useContext(AppContext) as AppContextState;

   

    useEffect(() => {
        updateOkEnabled(branchName, fromBranchId, busy);

    }, [branchName, fromBranchId, busy]);


    const updateOkEnabled = (branchName: string, fromBranchId: string, busy: boolean) => 
    {
        let projectNameValid = validateBranchName(branchName).length === 0;
        let fromBranchIdValid = validateFromBranchId(fromBranchId).length === 0;
        setOkEnabled(projectNameValid && fromBranchIdValid && !busy)
    }

    const handleCancel = () => {
        props.onClosing();
        resetDialog();
    };

    const handleClose = () => {
        props.onClosing();
        resetDialog();
    };

    const resetDialog = () => {
        setBranchName("");
        setBranchNameError(false);
        setBranchNameHelperText("");

        setBranchDescription("");
        setBranchDescriptionError(false);
        setBranchDescriptionHelperText("");

        setFromBranchId("");
        setBusy(false);
    };

    const showNewItemCreatedAlert = (message: string) => 
    {
        dispatch(postSuccess(message)); //TODO: Language
    }

    const validateBranchName = (name: string) : Array<string> => {
        let errors : Array<string> = [];

        if(name === "")
        {
            errors.push("Branchen måste ha ett namn"); //TODO: Language
        }
        else if(props.project.branches.findIndex(b => b.name.toLowerCase() === name.trim().toLowerCase()) >= 0)
        {
            errors.push(`Det finns redan en branch med det namnet`); //TODO: Language
        }

        return errors;
    };

    const validateFromBranchId = (fromBranchId: string) : Array<string> => {
        let errors : Array<string> = [];

        if(props.project.branches.findIndex(b=> b.id === fromBranchId) < 0)
        {
            errors.push("Branch från vilken den nya branchen ska utgå ifrån måste väljas"); //TODO: Language
        }

        return errors;
    };


    const branchNameChanged = (name: string) => {
        name = name.trim();
        setBranchName(name);
        let errors = validateBranchName(name);

        if (errors.length > 0)
        {
            let errorMessage : string = "";
            errors.map((error: string) => {
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

    const branchDescriptionChanged = (description: string) => {
        setBranchDescription(description);
    }

    const handleCreate = () => {

        let branch : CreateBranchRequest = {
            name: branchName,
            description: branchDescription,
            fromBranch: fromBranchId
        }

        setBusy(true);
        updateOkEnabled(branchName, fromBranchId, true);
        appContext.balsamApi.projectApi.createBranch(props.project.id, branch).then((response => {
            props.onBranchCreated?.(response.data.id);
            props.onClosing();
            resetDialog();

            showNewItemCreatedAlert(`Branchen "${response.data.name}" är skapad.`); //TODO: Language
        }), () => {
            dispatch(postError("Det gick inte att skapa branch " + branchName)); //TODO: Language
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
                    <DialogTitle>Skapa branch
                        {progress}
                    </DialogTitle>
                    <DialogContent>
                        <DialogContentText>
                            Ange information om branchen
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
                            <FormControl variant="standard" sx={{ mt: 4, minWidth: 120 }}>
                                <InputLabel id="branch-select-label">Branch som den nya branchen ska utgå ifrån</InputLabel>
                                <Select
                                    id="template-select"
                                    labelId="template-select-label"
                                    value={fromBranchId}
                                    variant='standard'
                                    onChange={(e) => setFromBranchId(e.target.value)}
                                    label="Branch"
                                    required
                                    >
                                
                                    {props.project.branches.map(branch => (
                                        <MenuItem key={branch.id} value={branch.id}>{branch.name}</MenuItem>
                                    ))}
                                </Select>
                            </FormControl>
                            <FormControl sx={{ mt: 4}}>
                                <TextField id="branch-name-input" autoFocus error={branchNameError} helperText={branchNameHelperText} label="Namn på branch" variant='standard' required value={branchName} onChange={e => branchNameChanged(e.target.value)} aria-describedby="Namn på branch" />
                            </FormControl>
                            <FormControl sx={{ mt: 4}}>
                                <TextField id="branch-description-input" error={branchDescriptionError} helperText={branchDescriptionHelperText} label="Beskrivning" variant='standard' value={branchDescription} onChange={e => branchDescriptionChanged(e.target.value)} aria-describedby="Beskrivning av branch" />
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

    return renderDialog();
}