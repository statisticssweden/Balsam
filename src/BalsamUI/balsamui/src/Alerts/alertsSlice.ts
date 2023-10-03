import { createSlice } from '@reduxjs/toolkit'
import {v4 as uuidv4} from 'uuid';

export interface AlertItem {
  id: string,
  text: string,
  severity: 'success' | 'error' | 'info' | 'warning',
}


const initialState: Array<AlertItem> = [];


export const alertsSlice = createSlice({
  name: 'alerts',
  initialState: initialState,
  reducers: {
    postSuccess: (state, action) => {
      let newItem: AlertItem = { id: uuidv4(), severity: "success", text: action.payload};
      state.push(newItem);
    },
    postError: (state, action) => {
      let newItem: AlertItem = { id: uuidv4(), severity: "error", text: action.payload};
      state.push(newItem);
    },
    postInfo: (state, action) => {
      let newItem: AlertItem = { id: uuidv4(), severity: "info", text: action.payload};
      state.push(newItem);
    },
    postWarning: (state, action) => {
      let newItem: AlertItem = { id: uuidv4(), severity: "warning", text: action.payload};
      state.push(newItem);
    },
    removeAlert: (state, action) => {
      let id = action.payload;
      const index = state.findIndex((item) => item.id === id);
      state.splice(index, 1);
    }
  },
})

export const { postSuccess, postError, postInfo, postWarning, removeAlert } = alertsSlice.actions

export const selectAlerts = (state: any) => state.alerts;

export default alertsSlice.reducer