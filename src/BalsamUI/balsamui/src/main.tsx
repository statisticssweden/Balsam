import React from 'react'
import ReactDOM from 'react-dom/client'
import App from './App.tsx'
import './index.css'
import KeyCloakService from './security/KeyCloakService.ts';
import HttpService from './services/HttpServices.ts';
import { BrowserRouter } from 'react-router-dom'
import { createTheme, ThemeProvider } from '@mui/material/styles';
import { svSE, enUS } from '@mui/material/locale';

const theme = createTheme(
  {
    palette: {
      primary: { main: '#1976d2' },
    },
  },
  svSE,
);

const root = ReactDOM.createRoot(
    document.getElementById("root") as HTMLElement
);

const renderApp = () =>
    root.render(
        <React.StrictMode>
            <ThemeProvider theme={theme}>
                <BrowserRouter>
                    <App />
                </BrowserRouter>
            </ThemeProvider>
        </React.StrictMode>
    );

KeyCloakService.CallLogin(renderApp);
HttpService.configure();

