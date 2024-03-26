import { Button, Paper } from '@mui/material';
import { RepositoryTemplate } from '../Model/RepositoryTemplate';
import { Link } from 'react-router-dom';
import { OpenInNew } from '@mui/icons-material';

export interface RepositoryTemplateViewProperties
{
    knowledgeLibraryId: string,
    template: RepositoryTemplate,
}

export default function RepositoryTemplateView( props: RepositoryTemplateViewProperties) {
    return (
        <div>
            <div className="project-header">
                <div><h2>{props.template.name}</h2></div>
                <div className="git-box">
                    <div className="git-box-content">
                        <Button component={Link as any} target="_blank" underline="hover" to={props.template.html}>Git<OpenInNew fontSize="inherit" /></Button>
                    </div>
                </div>
            </div>
            <Paper sx={{padding:7}} >
                {props.template.description}
            </Paper>
        </div>
    );
}