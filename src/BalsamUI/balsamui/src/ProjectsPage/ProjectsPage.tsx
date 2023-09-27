import BalsamApi from '../services/BalsamAPIServices';
import ProjectCard from '../ProjectCard/ProjectCard';
import { useState, useEffect } from 'react';

export default function ProjectsPage() {
    const [projects, setProjects] = useState<any[]>();
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


    function renderProjectsTable(projs: Array<any>) {
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
        : renderProjectsTable(projects as any[]);

    return ( 
        <div>
            <h2 id="tabelLabel">Projekt och Undersökningar</h2>
            {contents}
        </div>
        );

}