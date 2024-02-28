import { Button, Dialog, DialogActions, DialogContent, DialogTitle, FormControl, TextField } from "@mui/material";
import { PropsWithChildren, useState } from 'react'

export interface ConfirmInputSettings{
    errorMessage: string,
    requieredValue: string,
    label: string,
}

export interface ConfirmDialogProperties{
    onConfirm?: (itemKey: any) => void,
    onAbort?: (itemKey: any) => void,
    title: string,
    itemKey?: any
    open: boolean,
    confirmInput?: ConfirmInputSettings
}

export default function ConfirmDialog(props: PropsWithChildren<ConfirmDialogProperties>){
    const [confirmInputValue, setConfirmInputValue] = useState("");
    const [confirmIntpuError, setConfirmIntpuError] = useState(false);
    const [confirmInputHelperText, setConfirmInputHelperText] = useState("");

    function handleAbort()
    {
        if(props.onAbort) {
            props.onAbort(props.itemKey);
        }
    }

    function handleConfirm()
    {
        if (props.confirmInput)
        {
            if(confirmInputValue.trim() !== props.confirmInput.requieredValue.trim())
            {
                setConfirmIntpuError(true);
                setConfirmInputHelperText(props.confirmInput.errorMessage);
                return;
            }
            else 
            {
                setConfirmInputHelperText("");
            }
        }

        if(props.onConfirm) {
            props.onConfirm(props.itemKey);
        }
    }

    function confrimInputChanged(inputValue: string)
    {
        setConfirmInputValue(inputValue);
        setConfirmIntpuError(false);
        setConfirmInputHelperText("");
    }

    function renderChildrenContent()
    {
        return (
            <DialogContent>
                
                {props.children ? 
                    (props.children)
                    : ("")}
                {props.confirmInput ? 
                    (<FormControl sx={{ mt: 4}}>
                        <TextField id="branch-input" 
                            error={confirmIntpuError} 
                            helperText={confirmInputHelperText} 
                            variant='standard' 
                            onChange={e => confrimInputChanged(e.target.value)} 
                            label={props.confirmInput.label} 
                            required 
                            value={confirmInputValue}/>
                    </FormControl>) : ("")
                }
            </DialogContent>
        );
    }

    const childrenContent = renderChildrenContent();

    return (
            <Dialog
                open={props.open }
                onClose={handleAbort}
                fullWidth={true}>
                <DialogTitle>{props.title}</DialogTitle>
                {childrenContent}
                <DialogActions>
                    <Button onClick={handleAbort}>Avbryt</Button>
                    <Button onClick={handleConfirm}>Ok</Button>
                </DialogActions>
            </Dialog>
    );
}
