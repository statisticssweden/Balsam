import { createSlice } from '@reduxjs/toolkit'
import {v4 as uuidv4} from 'uuid';

export interface AlertItem {
  id: string,
  text: string,
  severity: 'success' | 'error' | 'info' | 'warning',
}

// export interface MessagesState {
//   messages: Array<MessageItemState>   
// }

const initialState: Array<AlertItem> = [];


export const alertsSlice = createSlice({
  name: 'alerts',
  initialState: initialState,
  reducers: {
    postAlert: (state, action) => {
      let newItem: AlertItem = { id: uuidv4(), severity: "success", text: action.payload};
      state.push(newItem);
    },
    removeAlert: (state, action) => {
      let id = action.payload;
      const index = state.findIndex((item) => item.id === id);
      state.splice(index, 1);
    }
  },
})

export const { postAlert, removeAlert } = alertsSlice.actions

export const selectAlerts = (state: any) => state.alerts;

export default alertsSlice.reducer