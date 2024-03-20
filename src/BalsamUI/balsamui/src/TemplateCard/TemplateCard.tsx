import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardActionArea from '@mui/material/CardActionArea';
import CardContent from '@mui/material/CardContent';
import CardMedia from '@mui/material/CardMedia';
import Button from '@mui/material/Button';
import Typography from '@mui/material/Typography';
import { Link } from 'react-router-dom';
import { OpenInNew } from '@mui/icons-material';
import './TemplateCard.css';
import { Template } from '../Model/Template';


export interface TemplateCardProperties {
    knowledgeLibraryId: string,
    template: Template
}

export default function TemplateCard( props : TemplateCardProperties) {
    const height = 300;
    const maxWidth = 300;
    const minWidth = 300;


    function renderCard(name: string, description: string, cardActionAreaTo: string, image?: string, mediaContent?: any)
    {
        
        return (
            <Card sx={{ height:height, maxWidth: maxWidth, minWidth: minWidth }}>
                <CardActionArea component={Link} to={cardActionAreaTo}>
                    <CardMedia
                        sx={{ height: 140 }}
                        image={image}
                        title={name}
                    >
                        {mediaContent}
                    </CardMedia>
                    <CardContent className="cardContent">
                        <Typography gutterBottom variant="h6" component="div">
                            {name}
                        </Typography>
                        <Typography variant="body2" color="text.secondary">
                            <label className="secondaryText">{description}</label>
                        </Typography>
                    </CardContent>
                </CardActionArea>
                <CardActions>
                    {cardActionAreaTo.length > 0 
                        ? <Button component={Link as any} to={cardActionAreaTo} target="_blank" underline="hover">Öppna<OpenInNew fontSize="inherit" /></Button> 
                        : ""
                    }
                </CardActions>
            </Card>
        );
    }
    
    let navigateToUrl = `/knowledgeLibrary/${props.knowledgeLibraryId}/template/${props.template.fileId}`;

    let card = renderCard(props.template.name, props.template.description, navigateToUrl)
    
    return card;
}