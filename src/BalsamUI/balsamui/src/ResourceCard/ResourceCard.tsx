import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardActionArea from '@mui/material/CardActionArea';
import CardContent from '@mui/material/CardContent';
import CardMedia from '@mui/material/CardMedia';
import Button from '@mui/material/Button';
import Typography from '@mui/material/Typography';
import resourceImage from '../assets/text.jpg'
import markdownImage from '../assets/text.jpg'
import pdfImage from '../assets/PDF_icon.svg'
import { Link } from 'react-router-dom';
import { OpenInNew } from '@mui/icons-material';
import './ResourceCard.css';
import { Resource, ResourceType } from '../Model/Model';

export interface ResourceCardProperties {
    projectid: string,
    branch: string,
    resource: Resource
}

export default function ResourceCard({ projectid, branch, resource } : ResourceCardProperties) {

    let secondaryText = resource.description;
    let cardActionAreaTo = "";
    let image = resourceImage;
    let resourceTitle = resource.name

    if (resource.type === ResourceType.Url) {
        cardActionAreaTo = resource.linkUrl || "";
    }
    else if (resource.type === ResourceType.Md) {

        cardActionAreaTo = `/resorucemarkdown/?resourcename=${resource.name}&contenturl=${resource.contentUrl}`;

        // if (linkerProjectId) {
        //     cardActionAreaTo = `/resorucemarkdown/linked/${projectid}/${branch}/${linkerProjectId}/${linkerProjectBranch}/${resource.resourcePath}`;
        // }

        image = markdownImage;
    }
    // else if (resource.type === "pdf") {
    //     image = pdfImage;
    // }
    
    return (
        <Card sx={{ height:300, maxWidth: 300, minWidth: 300 }}>
            <CardActionArea component={Link} to={cardActionAreaTo}>
                <CardMedia
                    sx={{ height: 140 }}
                    image={image}
                    title={resource.name}
                />
                <CardContent className="cardContent">
                    <Typography gutterBottom variant="h6" component="div">
                        {resourceTitle}
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                        <label className="secondaryText">{secondaryText}</label>
                    </Typography>
                </CardContent>
            </CardActionArea>
            <CardActions>
                <Button component={Link as any} to={cardActionAreaTo} target="_blank" underline="hover">Öppna<OpenInNew fontSize="inherit" /></Button>
            </CardActions>
        </Card>
    );
}