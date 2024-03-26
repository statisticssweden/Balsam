import { useState, useEffect, useContext } from 'react';
import KnowledgeLibraryCard from "../KnowledgeLibraryCard/KnowledgeLibraryCard";
import AppContext, { AppContextState } from '../configuration/AppContext';
import { KnowledgeLibrary } from '../services/BalsamAPIServices';
import { useDispatch } from 'react-redux';
import { postError } from '../Alerts/alertsSlice';

export default function KnowledgeLibrariesPage()
{
    const [libraries, setLibraries] = useState<Array<KnowledgeLibrary>>();
    const [loading, setLoading] = useState(true);
    const appContext = useContext(AppContext) as AppContextState;
    const dispatch = useDispatch();

    const loadData = () =>
    {
        setLoading(true);
        
        appContext.balsamApi.knowledgeLibraryApi.listKnowledgeLibaries()
        .then((response) => {
            setLibraries(response?.data);
            setLoading(false);
        })
        .catch(() => {
            dispatch(postError("Det gick inte att ladda kunskapsbibliotek")); //TODO: Language
        });
        
    }

    useEffect(() => {
        loadData();
    }, [])

    function renderKnowledgeLibraries(libraries: Array<KnowledgeLibrary>)
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
        : renderKnowledgeLibraries(libraries as Array<KnowledgeLibrary>);

    return (
        <div>
            <h2 id="tabelLabel">Kunskapsbibliotek</h2>
            {contents}
        </div>
        );
}