import React, {useState, useEffect} from 'react';
import { useSelector, useDispatch } from 'react-redux';
import Snackbar from '@mui/material/Snackbar';
import MuiAlert, { AlertProps } from '@mui/material/Alert';
import {selectMessage, postAlert } from '../Alerts/alertsSlice';

// import ReduxHelper from '../ReduxHelper/ReduxHelper';

export const Alert = React.forwardRef<HTMLDivElement, AlertProps>(function Alert(
        props,
        ref) 
    {
        return <MuiAlert elevation={6} ref={ref} variant="filled" {...props} />;
    });

export default function Alerter() {
    const message = useSelector(selectMessage);
    const [alertMessage, setAlertMessage] =useState("");
    const [snackbarOpen, setSnackbarOpen] = useState(false);
    const dispatch = useDispatch();

    const handleSnackbarClose = () => {
        setSnackbarOpen(false);
        //ReduxHelper.postAlert("");
        dispatch(postAlert(""));
        
    };

    useEffect(() => {
        if(message !== "")
        {
            setAlertMessage(message);
            setSnackbarOpen(true);

        }
        else 
        {
            setSnackbarOpen(false);
        }
    }, [message])

    return <Snackbar anchorOrigin={{vertical: "top", horizontal: "center"}} open={snackbarOpen} autoHideDuration={6000} onClose={handleSnackbarClose}>
        <Alert onClose={handleSnackbarClose} severity="success" sx={{ width: '100%' }}>
            {alertMessage}
        </Alert>
    </Snackbar>

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

