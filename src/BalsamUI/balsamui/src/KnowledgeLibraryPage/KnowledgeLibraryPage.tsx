import { useEffect, useState } from "react";
import { Library, librariesStaticData } from "../Model/Library";
import { useParams } from "react-router-dom";
import { useDispatch } from "react-redux";
import { postError } from "../Alerts/alertsSlice";

export default function KnowledgeLibraryPage()
{
    const [library, setLibrary] = useState<Library>();
    const [loading, setLoading] = useState(true);
    const { id } = useParams<string>();
    const dispatch = useDispatch();
    
    useEffect(() => {
        
        setLoading(true);

        //TODO: Get from API when implemented
        let library = librariesStaticData.find(l => l.id === id);
        if (library)
        {
            setLibrary(library);
            setLoading(false);
        }
        else 
        {
            dispatch(postError("Det gick inte att ladda kunskapsbibliotek")); //TODO: Language
        }

    },[id])

    function renderLibrary(library: Library)
    {
        return (            
            <div>
                <div className="project-header">
                    <div><h2>{library.name}</h2></div>
                    
                </div>
            </div>
            )
    }

    let contents = loading || library === undefined
        ? <p><em>Laddar...</em></p>
        : renderLibrary(library);

    return (
        <div>
            {contents}
        </div>
    ); 

}
