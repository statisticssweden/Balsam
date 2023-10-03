import React, {useState, useEffect} from 'react';
import { useSelector, useDispatch } from 'react-redux';
import Snackbar, { SnackbarCloseReason } from '@mui/material/Snackbar';
import MuiAlert, { AlertProps } from '@mui/material/Alert';
import {selectAlerts, AlertItem, removeAlert } from '../Alerts/alertsSlice';

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

            return <Snackbar anchorOrigin={{vertical: "top", horizontal: "center"}} open={true} autoHideDuration={6000} onClose={handleSnackbarClose}>
                <Alert onClose={handleAlertClose} severity={currentAlert.severity} sx={{ width: '100%' }}>
                {currentAlert.text}
                </Alert>
            </Snackbar>

        }
    }

    return renderSnackbar();
}


