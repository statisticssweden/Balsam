import { useState, useEffect } from 'react';
import KnowledgeLibraryCard from "../KnowledgeLibraryCard/KnowledgeLibraryCard";
import { Library, librariesStaticData } from "../Model/Library";

export default function KnowledgeLibrariesPage()
{
    const [libraries, setLibraries] = useState<Array<Library>>();
    const [loading, setLoading] = useState(true);

    const loadData = () =>
    {
        setLoading(true);
        
        //TODO: Get from API when implemented
        const libraries = librariesStaticData;
        
        setLibraries(libraries);
        setLoading(false);
        
    }

    useEffect(() => {
        loadData();
    }, [])

    function renderKnowledgeLibraries(libraries: Array<Library>)
    {
        return (

            <div className='cards' aria-labelledby="tabelLabel">         
                {
                    libraries.map((library) => {
                       return <KnowledgeLibraryCard knowledgeLibrary={library} key={library.id} />
                    })
                }
            </div>
        );
    }

    let contents = loading
        ? <p><em>Laddar...</em></p>
        : renderKnowledgeLibraries(libraries as Array<Library>);

    return (
        <div>
            <h2 id="tabelLabel">Kunskapsbibliotek</h2>
            {contents}
        </div>
        );
}