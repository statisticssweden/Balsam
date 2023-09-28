import React, {useState, Fragment,forwardRef, useImperativeHandle, useRef} from 'react';
import Snackbar from '@mui/material/Snackbar';
import MuiAlert, { AlertProps } from '@mui/material/Alert';


export const Alert = React.forwardRef<HTMLDivElement, AlertProps>(function Alert(
        props,
        ref) 
    {
        return <MuiAlert elevation={6} ref={ref} variant="filled" {...props} />;
    });



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

