import React, { useEffect, useState } from 'react';
import { Route, Routes, Link } from 'react-router-dom';
import AppBar from '@mui/material/AppBar';
import Toolbar from '@mui/material/Toolbar';
import Button from '@mui/material/Button';
import CssBaseline from '@mui/material/CssBaseline';
import ProjectsPage from './ProjectsPage/ProjectsPage';
import ProjectPage from './ProjectPage/ProjectPage';
import { useDispatch } from 'react-redux';
import './App.css'
import Alerter  from './Alerter/Alerter';
import ResoruceMarkdownPage from './ResourceMarkdownPage/ResoruceMarkdownPage';
import AppContext, { AppContextState } from './configuration/AppContext';
import { getConfig } from './configuration/configuration';
import { getBalsamAPI } from './services/BalsamAPIServices';
import { postError } from './Alerts/alertsSlice';
import MyPage from './MyPage/MyPage';
import ResoruceFolderPage from './ResourceFolderPage/ResourceFolderPage';
import KeyCloakService from './security/KeyCloakService.ts';
import KnowledgeLibrariesPage from './KnowledgeLibrariesPage/KnowledgeLibrariesPage.tsx';
import KnowledgeLibraryPage from './KnowledgeLibraryPage/KnowledgeLibraryPage.tsx';


function App() {
    const [appContextState, setAppContextState] = useState<AppContextState>();
    const [loading, setLoading] = useState(true);
    
    const dispatch = useDispatch();

    useEffect(() => {
        setLoading(true);

        getConfig().then((config) => {
            let state: AppContextState = {
                    config: config,
                    balsamApi: getBalsamAPI(config.apiurl),
                    getUserName: KeyCloakService.GetUserName,
                    getUserGroups: KeyCloakService.GetUserGroups,
                    refreshToken: KeyCloakService.RefreshToken,
                };
                setAppContextState(state)
                setLoading(false);
            })
            .catch(() => {
                dispatch(postError("Det gick inte att ladda konfigurationsfil")); //TODO: Language
            })

    }, []);

    const renderApp =  () =>
    {
        return (
            <React.Fragment>
                <AppContext.Provider value={appContextState} >
                <AppBar position="static">
                    <Toolbar>
                        <Button component={Link} color="inherit" to="/">Min sida</Button>
                        <Button component={Link} color="inherit" to="/projects">Projekt</Button>
                        <Button component={Link} color="inherit" to="/knowledgelibraries">Kunskapsbibliotek</Button>
                    </Toolbar>
                </AppBar>
                <div className="app">
                    <Routes>
                        <Route path="/" element={<MyPage />} />
                        <Route path="/projects/" element={<ProjectsPage />} />
                        <Route path="/project/:id" element={<ProjectPage />} />
                        <Route path="/knowledgelibraries/" element={<KnowledgeLibrariesPage />} />
                        <Route path="/knowledgelibrary/:id" element={<KnowledgeLibraryPage />} />
                        <Route path="resorucemarkdown/:projectId/:branchId/:fileId" element={<ResoruceMarkdownPage />} />
                        <Route path="resourcefolder/:projectId/:branchId/" element={<ResoruceFolderPage />} />
                    </Routes>
                </div>
                </AppContext.Provider>
            </React.Fragment>
        )
    }

    let contents = loading
        ? <p><em>Laddar...</em></p>
        :  renderApp();

    return ( <React.Fragment>
        <CssBaseline />

        { contents }
        <Alerter />
    </React.Fragment> )

}

export default App
