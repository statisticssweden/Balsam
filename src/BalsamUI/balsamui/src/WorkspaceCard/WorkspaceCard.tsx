import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardActionArea from '@mui/material/CardActionArea';
import CardContent from '@mui/material/CardContent';
import CardMedia from '@mui/material/CardMedia';
import Button from '@mui/material/Button';
import Typography from '@mui/material/Typography';
import workspaceImage from '../assets/workspace.png'
import { Link } from 'react-router-dom';
import { OpenInNew } from '@mui/icons-material';
import './WorkspaceCard.css';
import { Workspace } from '../services/BalsamAPIServices';
import { Fragment, useState } from 'react';
import ConfirmDialog from '../ConfirmDialog/ConfirmDialog';

export interface WorkspaceCardProperties {
    workspace: Workspace,
    templateName: string,
    canOpen: boolean,
    canDelete: boolean,
    deleteWorkspaceCallback: (projectId: string, branchId: string, workspaceId: string) => void,
}

export default function WorkspaceCard(props : WorkspaceCardProperties) {

    const [showDeleteConfirmation, setShowDeleteConfirmation] = useState(false);

    let secondaryText = props.templateName;
    let image = workspaceImage;
    let workspaceTitle = props.workspace.name;

    function onDeleteConfirm()
    {
        props.deleteWorkspaceCallback(props.workspace.projectId, props.workspace.branchId, props.workspace.id);
        setShowDeleteConfirmation(false);
        
    }

    function onDeleteAbort()
    {
        setShowDeleteConfirmation(false);
    }

    function onDeleteClick()
    {
        setShowDeleteConfirmation(true);    

    }

    function renderDeleteDialog()
    {
        //TODO: Language
        return (
            <ConfirmDialog title='Ta bort bearbetningsmiljö' open={showDeleteConfirmation} key={props.workspace.id} onConfirm={onDeleteConfirm} onAbort={onDeleteAbort}> 
                Vill du ta bort bearbetningsmiljön {props.workspace.name}? <br/> 
                <strong>Om du vill att kod eller data inte ska gå förlorad, se till att du utfört följande steg innan du trycker OK </strong>
                <ul>
                    <li>Utför commit och push på din källkod så att den lagras i projektets repository</li>
                    <li>Lagra data utanför bearabetningsmiljön. T ex i S3 lagring</li>
                </ul>
            </ConfirmDialog>
        );
    }

    const deleteDialogContent = renderDeleteDialog();
    const cardNavigationUrl = props.canOpen ? props.workspace.url : "";
    
    
    return (
        <Fragment>
            {deleteDialogContent}
            <Card sx={{ height:300, maxWidth: 300, minWidth: 300 }}>
                <CardActionArea component={Link} to={cardNavigationUrl} disabled={!props.canOpen}>
                    <CardMedia
                        sx={{ height: 140 }}
                        image={image}
                        title={props.workspace.name}
                    />
                    <CardContent className="cardContent">
                        <Typography gutterBottom variant="h6" component="div">
                            {workspaceTitle}
                        </Typography>
                        <Typography variant="body2" color="text.secondary">
                            <label className="secondaryText">{secondaryText}</label>
                        </Typography>
                    </CardContent>
                </CardActionArea>
                <CardActions>
                    <div className='buttons'>
                        {props.canOpen && <Button component={Link as any} to={props.workspace.url} target="_blank" underline="hover">Öppna<OpenInNew fontSize="inherit" /></Button> }
                        {props.canDelete && <Button onClick={onDeleteClick} >Ta bort</Button>}                        
                    </div>
                </CardActions>
            </Card>
        </Fragment>
    );
}