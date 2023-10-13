// import { createSlice } from '@reduxjs/toolkit'
// import { AppConfiguration } from './configuration'
// import type { PayloadAction } from '@reduxjs/toolkit'

// const initialState: AppConfiguration = { 
//         apiurl: ""
//     };

// export const configSlice = createSlice({
//     name: 'config',
//     initialState: initialState,
//     reducers: {
//         setConfig:(state:AppConfiguration, action: PayloadAction<AppConfiguration>) => {
//                 state = action.payload;
//             }
//     }
// });

// export const { setConfig } = configSlice.actions
// export const selectConfig = (state: any) => state.config;
// export default configSlice.reducer