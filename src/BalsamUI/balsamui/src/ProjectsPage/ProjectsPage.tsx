import ProjectCard from '../ProjectCard/ProjectCard';
import { useState, useEffect, useContext } from 'react';
import NewProjectDialog from '../NewProjectDialog/NewProjectDialog';
import './ProjectsPage.css';
import { useDispatch } from 'react-redux';
import { postError } from '../Alerts/alertsSlice';
import { Project} from '../services/BalsamAPIServices'
import AppContext, { AppContextState } from '../configuration/AppContext';
import { Button } from '@mui/material';

export default function ProjectsPage() {

    const [projects, setProjects] = useState<Array<Project>>();
    const [loading, setLoading] = useState(true);
    const [newDialogOpen, setNewDialogOpen] = useState(false);
    
    const dispatch = useDispatch();
    const appContext = useContext(AppContext) as AppContextState;

    const loadData = () =>
    {
        setLoading(true);
        
        appContext.balsamApi.projectApi.listProjects(true)
        .then((response) => {
            setProjects(response?.data.projects);
            setLoading(false);
        })
        .catch(() => {
            dispatch(postError("Det gick inte att ladda projekt")); //TODO: Language
        });
    }

    useEffect(() => {
        loadData();
    }, [])

    const onNewProjectDialogClosing = () => {
        setNewDialogOpen(false);
        loadData();
    };

    const handleClickOpen = () => {
        setNewDialogOpen(true);
    };

    function renderProjects(projs: Array<Project>) {
        return (
            <div className='cards' aria-labelledby="tabelLabel">         
                {
                    projs.map((project) => {
                    return <ProjectCard project={project} key={project.id} />
                    })
                }
            </div>
        );
    }

    let contents = loading
        ? <p><em>Laddar...</em></p>
        : renderProjects(projects as Array<Project>);

    return (
        <div>
            <h2 id="tabelLabel">Projekt och Undersökningar</h2>
            <div className='buttonrow'>
                <Button variant="contained" onClick={handleClickOpen}>
                    +
                </Button>
                <NewProjectDialog open={newDialogOpen} onClosing={onNewProjectDialogClosing} ></NewProjectDialog>
            </div>
            {contents}
        </div>
        );

}