import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardActionArea from '@mui/material/CardActionArea';
import CardContent from '@mui/material/CardContent';
import CardMedia from '@mui/material/CardMedia';
import Button from '@mui/material/Button';
import Typography from '@mui/material/Typography';
import resourceImage from '../assets/text.png'
import markdownImage from '../assets/text.png'
import { Link } from 'react-router-dom';
import { OpenInNew } from '@mui/icons-material';
import './ResourceCard.css';
import { Resource, ResourceType } from '../Model/Resource';
import FolderIcon from '@mui/icons-material/Folder';

export interface ResourceCardProperties {
    resource: Resource
}

export default function ResourceCard({ resource } : ResourceCardProperties) {
    const height = 300;
    const maxWidth = 300;
    const minWidth = 300;

    //TODO: merge with renderResourceFolderCard
    function renderLinkCard(name: string, description: string, cardActionAreaTo: string, image?: string, mediaContent?: any)
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
    
    let card: any;

    if(resource.type === ResourceType.Folder)
    {
        let mediaContent = <div className='icon-container'> <FolderIcon sx={{fontSize:"134px", color:"lightgray"}}></FolderIcon></div>;
        let cardActionAreaTo = `/resourcefolder/${resource.projectId}/${resource.branchId}/?folder=${resource.fileName}`;
        card = renderLinkCard(resource.name, resource.description, cardActionAreaTo, undefined, mediaContent);
    }
    else if (resource.type === ResourceType.Url) {
        let cardActionAreaTo = resource.linkUrl || "";
        card = renderLinkCard(resource.name, resource.description, cardActionAreaTo, resourceImage);
    }
    else if (resource.type === ResourceType.Md) {
        let cardActionAreaTo = `/resorucemarkdown/${resource.projectId}/${resource.branchId}/${resource.fileId}?resourcename=${resource.name}`;
        card = renderLinkCard(resource.name, resource.description, cardActionAreaTo, markdownImage);
    }
    
    return card;
}