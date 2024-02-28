import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardActionArea from '@mui/material/CardActionArea';
import CardContent from '@mui/material/CardContent';
import CardMedia from '@mui/material/CardMedia';
import Button from '@mui/material/Button';
import Typography from '@mui/material/Typography';
import projectimage from '../assets/project.png'
import { OpenInNew } from '@mui/icons-material';
import { Link } from 'react-router-dom';
import { Project } from '../services/BalsamAPIServices';
import ButtonMenu from '../ButtonMenu/ButtonMenu';
import './ProjectCard.css'
import ConfirmDialog, { ConfirmInputSettings } from '../ConfirmDialog/ConfirmDialog';
import { useContext, useState } from 'react';
import AppContext, { AppContextState } from '../configuration/AppContext';
import { postError, postSuccess } from '../Alerts/alertsSlice';
import { useDispatch } from 'react-redux';


type ProjectCardProperties = {
    project: Project,
    onProjectDeleted?: (projectId: string) => void;
}

export default function ProjectCard(props : ProjectCardProperties) {
    const toActionUrl = '/project/' + props.project.id;
    const [showDeleteProjectConfirmation, setShowDeleteProjectConfirmation] = useState(false);
    const [deleted, setDeleted] = useState(false);
    const dispatch = useDispatch();
    const appContext = useContext(AppContext) as AppContextState;

    
    function onDeleteProjectConfirm()
    {
        appContext.balsamApi.projectApi.deleteProject(props.project.id)
            .then(() => {
                dispatch(postSuccess(`Projektet ${props.project.name} är borttaget.`)); //TODO: Language
                setDeleted(true);
            })
            .catch(() => {
                dispatch(postError(`Det gick inte att ta bort projektet ${props.project.name}.`)); //TODO: Language
            });
        
        setShowDeleteProjectConfirmation(false);
    }

    function onDeleteProjectAbort()
    {
        setShowDeleteProjectConfirmation(false);
    }

    function handleDeleteProjectClick(): void {
        setShowDeleteProjectConfirmation(true);
    }

    function renderDeleteProjectDialog()
    {
        //TODO: Language
        //TODO: Optimize, a confirmation dialog is created per project card.
        let confirmInput: ConfirmInputSettings = {
            label: "Projektnamn",
            errorMessage: "Fel namn angivet",
            requieredValue: props.project.name,
        };

        return (
            <ConfirmDialog title='Ta bort projekt' open={showDeleteProjectConfirmation} key={props.project.id} onConfirm={onDeleteProjectConfirm} onAbort={onDeleteProjectAbort} confirmInput={confirmInput} > 
                Vill du ta bort projektet <i>{props.project.name}</i>?<br/> 
                <strong>VARNING! All källkod i git-repositoryt och all data i projektets S3-bucket kommer att tas bort!</strong>
                <ul>
                    <li>Om du inte vill att koden som ligger i git-repositoryt ska försvinna flytta den till annan plats.</li>
                    <li>Om du inte vill att data som ligger S3-bucketen skall försvinna så flytta denna till en annan plats.</li>
                </ul>
                Ange namnet på projektet i fältet nedan för att bekräfta borttagande av projekt.
            </ConfirmDialog>
        );
    }

    function renderCard()
    {
        return (
            <Card sx={{ maxWidth: 300, minWidth: 300 }}>
                <CardActionArea component={Link} to={toActionUrl}>
                    <CardMedia
                        sx={{ height: 140 }}
                        image={projectimage}
                    />
                    <CardContent>
                        <Typography gutterBottom variant="h5" component="div">
                            {props.project.name}
                        </Typography>
                        <Typography variant="body2" color="text.secondary">
                            {props.project.description}
                        </Typography>
                    </CardContent>
                </CardActionArea>
                <CardActions>
                    <div className='buttons-container'>
                        <div>
                            <Button component={Link as any} 
                            target="_blank" 
                            underline="hover" 
                            to={toActionUrl}
                        >
                            Öppna<OpenInNew fontSize="inherit" />
                        </Button>
                        </div>
                        <div>
                        <ButtonMenu options={[ { onClick: handleDeleteProjectClick, buttonContent:"Ta bort projekt", itemKey:"delete"  }  ]}   ></ButtonMenu>
                        </div>
                    </div>
                </CardActions>
            </Card>
        )
    }

    let card = renderCard();
    let deleteProjectDialog = renderDeleteProjectDialog();

    return (<>
            {showDeleteProjectConfirmation ? (deleteProjectDialog) : ("")}
            {!deleted ? (card) : ("")}
        </>);
}
