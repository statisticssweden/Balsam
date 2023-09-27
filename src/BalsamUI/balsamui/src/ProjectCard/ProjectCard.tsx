import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardActionArea from '@mui/material/CardActionArea';
import CardContent from '@mui/material/CardContent';
import CardMedia from '@mui/material/CardMedia';
// import Button from '@mui/material/Button';
import Typography from '@mui/material/Typography';
import projectimage from '../assets/project.jpg'
import { OpenInNew } from '@mui/icons-material';
import { Link } from 'react-router-dom';

type ProjectCardProperties = {
    project: any
}

export default function ProjectCard(properties : ProjectCardProperties) {
    return (
        <Card sx={{ maxWidth: 300, minWidth: 300 }}>
            <CardActionArea component={Link} to={'project/' + properties.project.id}>
                <CardMedia
                    sx={{ height: 140 }}
                    image={projectimage}
                    title="green iguana"
                />
                <CardContent>
                    <Typography gutterBottom variant="h5" component="div">
                        {properties.project.name}
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                        {properties.project.description}
                    </Typography>
                </CardContent>
            </CardActionArea>
            <CardActions>
                <Link  
                    target="_blank" 
                    
                    to={'project/' + properties.project.id}>
                        Öppna
                        <OpenInNew fontSize='inherit' />
                    </Link>
                    {/* <Button component={Link} 
                    target="_blank" 
                    underline="hover" 
                    to={'project/' + project.id}>
                        Öppna<OpenInNew fontSize="small" />
                    </Button> */}
            </CardActions>
        </Card>
    );
}