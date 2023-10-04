import React, {useState, useEffect} from 'react';
import { useSelector, useDispatch } from 'react-redux';
import Snackbar, { SnackbarCloseReason } from '@mui/material/Snackbar';
import MuiAlert, { AlertColor, AlertProps } from '@mui/material/Alert';
import Button from '@mui/material/Button';
import IconButton from '@mui/material/IconButton';
import CloseIcon from '@mui/icons-material/Close';
import {selectAlerts, AlertItem, removeAlert } from '../Alerts/alertsSlice';
import { Link } from 'react-router-dom';
import "./Alerter.css"

export const Alert = React.forwardRef<HTMLDivElement, AlertProps>(function Alert(
        props,
        ref)
    {
        return <MuiAlert elevation={6} ref={ref} variant="filled" {...props} />;
    });

export default function Alerter() {
    const alerts = useSelector(selectAlerts);
    const [currentAlert, setCurrentAlert] = useState<AlertItem>();
    const dispatch = useDispatch();

    const handleSnackbarClose = (event: any, reason: SnackbarCloseReason) => {
        
        if (reason !== "clickaway")
        {
            if(currentAlert !== undefined)
            {
                dispatch(removeAlert(currentAlert.id));
            }
        }
    };

    const handleAlertClose = () => {
        
        if(currentAlert !== undefined)
        {
            dispatch(removeAlert(currentAlert.id));
        }
    };

    useEffect(() => {
        if(alerts.length > 0)
        {
            setCurrentAlert(alerts[0]);
            
        }
        else
        {
            setCurrentAlert(undefined);
        }
    }, [alerts])

    useEffect(() => {
    }, [currentAlert])

    const renderSnackbar = () =>
    {
        if (currentAlert !== undefined)
        {
            const link = currentAlert.link ? (<a className='alert-link-button' href={currentAlert.link.href}>
                                    {currentAlert.link.caption}
                                    </a>) : ("");
            // const link = currentAlert.link ? (<Button className='alert-link-button' component={Link as any} underline="hover" to={currentAlert.link.href} color='info' size="small" onClick={handleAlertClose}>
            //                         {currentAlert.link.caption}
            //                         </Button>) : ("");
            // // const link = currentAlert.link ? (<Button color="secondary" size="small" onClick={handleAlertClose}>
            // //                         {currentAlert.link}
            // //                         </Button>) : ("");

            // if (currentAlert.link !== undefined)
            // {
            //     const action = (
            //         <React.Fragment>
            //           <Button color="secondary" size="small" onClick={handleAlertClose}>
            //             UNDO
            //           </Button>
            //           <IconButton
            //             size="small"
            //             aria-label="close"
            //             color="inherit"
            //             onClick={handleAlertClose}
            //           >
            //             <CloseIcon fontSize="small" />
            //           </IconButton>
            //         </React.Fragment>
            //       );   
            //       return <Snackbar anchorOrigin={{vertical: "top", horizontal: "center"}} open={true} autoHideDuration={6000} onClose={handleSnackbarClose} action={action}/>

            // }
            // else
            // {
                return <Snackbar anchorOrigin={{vertical: "top", horizontal: "center"}} open={true} autoHideDuration={600000} onClose={handleSnackbarClose}>
                    <Alert onClose={handleAlertClose} severity={currentAlert.severity as AlertColor} sx={{ width: '100%' }}>
                    <div className='alert-content' ><div>{currentAlert.text}</div><div> {link}</div></div>
                    </Alert>
                </Snackbar>
            // }

        }
    }

    return renderSnackbar();
}


