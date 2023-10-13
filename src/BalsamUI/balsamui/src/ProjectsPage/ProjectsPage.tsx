import ProjectCard from '../ProjectCard/ProjectCard';
import { useState, useEffect, useContext } from 'react';
import NewProjectDialog from '../NewProjectDialog/NewProjectDialog';
import './ProjectsPage.css';
import { useDispatch } from 'react-redux';
import { postError } from '../Alerts/alertsSlice';
import { Project} from '../services/BalsamAPIServices'
import AppContext, { AppContextState } from '../configuration/AppContext';


export default function ProjectsPage() {

    const [projects, setProjects] = useState<Array<Project>>();
    const [loading, setLoading] = useState(true);
    const dispatch = useDispatch();
    const appContext = useContext(AppContext) as AppContextState;

    const loadData = () =>
    {
        setLoading(true);

        const fetchData = async () => {
            let promise = appContext.balsamApi.projectApi.listProjects(true);
            promise.catch(() => {
                
                dispatch(postError("Det gick inte att ladda projekt"))
            })

            let listProjectsResponse = await promise;
            
            setProjects(listProjectsResponse.data.projects);
            setLoading(false);

        }

        fetchData()
            .catch(console.error);
    }

    useEffect(() => {
        loadData();
    }, [])

    const onNewProjectDialogClosing = () => {

        loadData();

    };

    function renderProjectsTable(projs: Array<Project>) {
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
        : renderProjectsTable(projects as Array<Project>);

    return (
        <div>
            <h2 id="tabelLabel">Projekt och Undersökningar</h2>
            <div className='buttonrow'>
                <NewProjectDialog onClosing={onNewProjectDialogClosing}></NewProjectDialog>
            </div>
            {contents}
        </div>
        );

}