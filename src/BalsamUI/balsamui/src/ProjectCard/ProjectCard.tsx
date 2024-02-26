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

type ProjectCardProperties = {
    project: Project
}

export default function ProjectCard(props : ProjectCardProperties) {
    const toActionUrl = '/project/' + props.project.id;
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

                <Button component={Link as any} 
                    target="_blank" 
                    underline="hover" 
                    to={toActionUrl}
                >
                    Öppna<OpenInNew fontSize="inherit" />
                </Button>
            </CardActions>
        </Card>
    );
}