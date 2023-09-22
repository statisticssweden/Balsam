import React from 'react'
import ReactDOM from 'react-dom/client'
import App from './App.tsx'
import './index.css'
import KeyCloakService from './security/KeyCloakService.ts';
import HttpService from './services/HttpServices.ts';


const root = ReactDOM.createRoot(
    document.getElementById("root") as HTMLElement
);

const renderApp = () =>
    root.render(
        <React.StrictMode>
            <App />
        </React.StrictMode>
    );

KeyCloakService.CallLogin(renderApp);
HttpService.configure();

