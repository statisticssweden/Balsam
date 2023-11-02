import { configureStore } from '@reduxjs/toolkit';
import alertsReducer from '../Alerts/alertsSlice';

export default configureStore({
  reducer: {
    alerts: alertsReducer,
  },
});