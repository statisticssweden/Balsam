import React, {useState, useEffect} from 'react';
import { useSelector, useDispatch } from 'react-redux';
import Snackbar, { SnackbarCloseReason } from '@mui/material/Snackbar';
import MuiAlert, { AlertProps } from '@mui/material/Alert';
import {selectAlerts, postAlert, AlertItem, removeAlert } from '../Alerts/alertsSlice';

// import ReduxHelper from '../ReduxHelper/ReduxHelper';

export const Alert = React.forwardRef<HTMLDivElement, AlertProps>(function Alert(
        props,
        ref)
    {
        return <MuiAlert elevation={6} ref={ref} variant="filled" {...props} />;
    });

export default function Alerter() {
    const alerts = useSelector(selectAlerts);
    const [currentAlert, setCurrentAlert] = useState<AlertItem>();
    //const [alertMessage, setAlertMessage] = useState("");
    // const [alertSeverity, setAlertSeverity] = useState("");
    // const [alertId, setAlertId] = useState("");
    //const [snackbarOpen, setSnackbarOpen] = useState(false);
    const dispatch = useDispatch();

    const handleSnackbarClose = (event, reason: SnackbarCloseReason) => {
        
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
            // setSnackbarOpen(true);
        }
        else
        {
            setCurrentAlert(undefined);
            //alert("då");
            // setSnackbarOpen(false);
        }
    }, [alerts])

    useEffect(() => {
        // if(alerts.length > 0)
        // {
        //     setCurrentAlert(alerts[0]);
        //     // setSnackbarOpen(true);
        // }
        // else
        // {
        //     setCurrentAlert(undefined);
        //     //alert("då");
        //     // setSnackbarOpen(false);
        // }
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
        else
        {
            //setSnackbarOpen(false);
        }
    }

    return renderSnackbar();
}

// export default function AppSnackBars(){
//     // const [alertMessage, setAlertMessage]  = useState("");
//     // const [snackbarOpen, setSnackbarOpen]  = useState(false);
//     const [snackbars, setSnackbars] = useState([])

//     const handleSnackbarClose = () => {
//         setSnackbarOpen(false);
//     }

//     const renderAlerts

//     return <Snackbar anchorOrigin={{vertical: "top", horizontal: "center"}} open={snackbarOpen} autoHideDuration={60000} onClose={handleSnackbarClose}>
//         <Alert onClose={handleSnackbarClose} severity="success" sx={{ width: '100%' }}>
//             {alertMessage}
//         </Alert>
//     </Snackbar>
// }


