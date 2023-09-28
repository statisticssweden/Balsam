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


// The function below is called a selector and allows us to select a value from
// the state. Selectors can also be defined inline where they're used instead of
// in the slice file. For example: `useSelector((state) => state.counter.value)`
export const selectMessage = (state: any) => state.alerts.message;
//export const selectMessage = (state: alertMessage) => state.alerts.message
//export const selectCount = (state) => state.counter.value

export default alsertsSlice.reducer