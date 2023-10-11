import React from 'react';
import { Route, Routes, Link } from 'react-router-dom';
import AppBar from '@mui/material/AppBar';
import Toolbar from '@mui/material/Toolbar';
import Button from '@mui/material/Button';
import CssBaseline from '@mui/material/CssBaseline';
import ProjectsPage from './ProjectsPage/ProjectsPage';
import ProjectPage from './ProjectPage/ProjectPage';

import './App.css'
import Alerter  from './Alerter/Alerter';
import ResoruceMarkdownPage from './ResourceMarkdownPage/ResoruceMarkdownPage';

function App() {
    return (
        
        <React.Fragment>
             <CssBaseline />

             <AppBar position="static">
                 <Toolbar>
                     <Button component={Link} color="inherit" to="/">Projekt</Button>
                     <Button component={Link} color="inherit" to="/library">Kunskapsbibliotek</Button>
                 </Toolbar>
             </AppBar>
             <div className="app">
                
                 <Routes>
                    <Route path="/" element={<ProjectsPage />} />
                    <Route path="project/:id" element={<ProjectPage />} />
                    <Route path="resorucemarkdown" element={<ResoruceMarkdownPage />} />
                 </Routes>
             </div>
             <Alerter />
        </React.Fragment>
        
    )
}

export default App
