import React from 'react'
import ReactDOM from 'react-dom/client'
import App from './App.tsx'
import './index.css'
import KeyCloakService from './security/KeyCloakService.ts';
import HttpService from './services/HttpServices.ts';

import { Provider } from 'react-redux';
import { BrowserRouter } from 'react-router-dom'
import { createTheme, ThemeProvider } from '@mui/material/styles';
import { svSE } from '@mui/material/locale';
import store from './App/store.ts';

const theme = createTheme(
  {
    palette: {
      background: {
        default: "#F0F0F0"
      },
    },
  },
  svSE,
);

const root = ReactDOM.createRoot(
    document.getElementById("root") as HTMLElement
);

const renderApp = () =>
    root.render(
      <Provider store={store}>
        
        <React.StrictMode>
            <ThemeProvider theme={theme}>
                <BrowserRouter>
                    <App />
                </BrowserRouter>
            </ThemeProvider>
        </React.StrictMode>
        
      </Provider>
    );

KeyCloakService.CallLogin(renderApp);
HttpService.configure();

