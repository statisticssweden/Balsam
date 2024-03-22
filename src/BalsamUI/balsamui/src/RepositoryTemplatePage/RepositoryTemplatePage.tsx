import { useState, useEffect, useContext } from 'react';
import { useParams } from 'react-router-dom'
import AppContext, { AppContextState } from '../configuration/AppContext';

import { useDispatch } from 'react-redux';
import { postError } from '../Alerts/alertsSlice';
import RepositoryTemplateView from '../RepositoryTemplateView/RepositoryTemplateView';
import { RepositoryTemplate } from '../Model/RepositoryTemplate';

export default function RepositoryTemplatePage() {
    const {knowledgeLibraryId, fileId } = useParams<string>();
    const appContext = useContext(AppContext) as AppContextState;
    const [template, setTemplate] = useState<RepositoryTemplate>()
    const dispatch = useDispatch();

    useEffect(() => {
        if (knowledgeLibraryId && fileId)
        {
            appContext.balsamApi.knowledgeLibraryApi.getKnowledgeLibraryFileContent(knowledgeLibraryId, fileId)
            .then((response) => 
            {   
                if (response.data !== null)
                {
                    
                    if (typeof response.data === "string")
                    {
                        let data = JSON.parse(response.data);
                        setTemplate(data)
                    }
                    else{
                        setTemplate(response.data)
                    }              
                }
            })
            .catch( () => {
                dispatch(postError("Det gick inte att ladda templatefil")); //TODO: Language
            });      
        }
       

    }, [knowledgeLibraryId, fileId])


    let content = template ?  <RepositoryTemplateView knowledgeLibraryId={knowledgeLibraryId!} template={template} /> : "Laddar..."

    return (
        content
    );
}