import { Button, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle } from "@mui/material";
import { PropsWithChildren } from 'react'

export interface ConfirmDialogProperties{
    onConfirm?: (key: any) => void,
    onAbort?: (key: any) => void,
    title: string,
    key?: any
    open: boolean
}

export default function ConfirmDialog(props: PropsWithChildren<ConfirmDialogProperties>){

    function handleAbort()
    {
        if(props.onAbort) {
            props.onAbort(props.key);
        }
    }

    function handleConfirm()
    {
        if(props.onConfirm) {
            props.onConfirm(props.key);
        }
    }

    function renderChildrenContent()
    {
        return (
            props.children ? 
                <DialogContent>
                    <DialogContentText>
                        {props.children}
                    </DialogContentText>
                </DialogContent>
                : ""
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
