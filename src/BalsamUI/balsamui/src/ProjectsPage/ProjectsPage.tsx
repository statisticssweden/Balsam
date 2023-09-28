import BalsamApi from '../services/BalsamAPIServices';
import ProjectCard from '../ProjectCard/ProjectCard';
import { useState, useEffect } from 'react';
import NewProjectDialog from '../NewProjectDialog/NewProjectDialog';

export default function ProjectsPage() {
    const [projects, setProjects] = useState<Array<any>>();
    const [loading, setLoading] = useState(true);

    const loadData = () => 
    {
        setLoading(true);

        const fetchData = async () => {

            let projects = await BalsamApi.getProjects();
            setProjects(projects);
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

    function renderProjectsTable(projs: Array<any>) {
        return (
            
            <div className='cards' aria-labelledby="tabelLabel">
                <NewProjectDialog onClosing={onNewProjectDialogClosing}></NewProjectDialog>
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
        : renderProjectsTable(projects as Array<any>);

    return ( 
        <div>
            <h2 id="tabelLabel">Projekt och Undersökningar</h2>
            {contents}
        </div>
        );

}