import { createSlice } from '@reduxjs/toolkit'

// type alertMessage = {
//   message: string
// }

export const alsertsSlice = createSlice({
  name: 'alerts',
  initialState: {
    message: "",
  },
  reducers: {
    postAlert: (state, action) => {
      state.message = action.payload
    },
    
  },
})

export const { postAlert } = alsertsSlice.actions

export const selectMessage = (state: any) => state.alerts.message;

export default alsertsSlice.reducer