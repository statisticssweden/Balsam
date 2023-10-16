import { configureStore } from '@reduxjs/toolkit';
import alertsReducer from '../Alerts/alertsSlice';
//import configReducer from '../configuration/configSlice';

export default configureStore({
  reducer: {
    alerts: alertsReducer,
    //config: configReducer
  },
});