import React from 'react';
import { Route, Routes, Link } from 'react-router-dom';
import AppBar from '@mui/material/AppBar';
import Toolbar from '@mui/material/Toolbar';
import Button from '@mui/material/Button';
import CssBaseline from '@mui/material/CssBaseline';
import ProjectsPage from './ProjectsPage/ProjectsPage';

//import { useState } from 'react'
import './App.css'




function App() {
    //const [count, setCount] = useState(0)

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
                 </Routes>
             </div>
        </React.Fragment>
        
    )
}

export default App
