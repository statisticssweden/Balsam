import { createSlice } from '@reduxjs/toolkit'
import {v4 as uuidv4} from 'uuid';
import type { PayloadAction } from '@reduxjs/toolkit'

export interface AlertLink
{
  caption: string,
  href: string  
}

export interface AlertItem {
  id: string,
  text: string,
  //severity: 'success' | 'error' | 'info' | 'warning', //blir knas med prepare funktion
  severity: string
  link?: AlertLink
}


const initialState: Array<AlertItem> = [];


export const alertsSlice = createSlice({
  name: 'alerts',
  initialState: initialState,
  reducers: {
    postSuccess: { 
      reducer: (state, action: PayloadAction<AlertItem>) => {
        //let newItem: AlertItem = { id: uuiv4(), severity: "success", text: action.payload.text, link: action.payload.link};
        state.push(action.payload);
      },
      prepare: (text: string, link?: AlertLink) => {
        const id = uuidv4(); 
        return {
          payload: {
            id: id,
            text: text,
            severity: 'success',
            link: link,
            
          }  
        }
      },

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