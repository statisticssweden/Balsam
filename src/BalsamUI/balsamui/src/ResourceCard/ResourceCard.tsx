import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardActionArea from '@mui/material/CardActionArea';
import CardContent from '@mui/material/CardContent';
import CardMedia from '@mui/material/CardMedia';
import Button from '@mui/material/Button';
import Typography from '@mui/material/Typography';
import { Link } from 'react-router-dom';
import { OpenInNew } from '@mui/icons-material';
import './ResourceCard.css';

export interface ResourceCardProperties {
    name: string, 
    description: string, 
    cardActionAreaTo: string, 
    image?: string, 
    mediaContent?: any
}

export default function ResourceCard( props : ResourceCardProperties) {
    const height = 300;
    const maxWidth = 300;
    const minWidth = 300;

    return (
        <Card sx={{ height:height, maxWidth: maxWidth, minWidth: minWidth }}>
            <CardActionArea component={Link} to={props.cardActionAreaTo}>
                <CardMedia
                    sx={{ height: 140 }}
                    image={props.image}
                    title={props.name}
                >
                    {props.mediaContent}
                </CardMedia>
                <CardContent className="cardContent">
                    <Typography gutterBottom variant="h6" component="div">
                        {props.name}
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                        <label className="secondaryText">{props.description}</label>
                    </Typography>
                </CardContent>
            </CardActionArea>
            <CardActions>
                {props.cardActionAreaTo.length > 0 
                    ? <Button component={Link as any} to={props.cardActionAreaTo} target="_blank" underline="hover">Öppna<OpenInNew fontSize="inherit" /></Button> 
                    : ""
                }
            </CardActions>
        </Card>
    );

}