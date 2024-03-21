import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardContent from '@mui/material/CardContent';
import CardMedia from '@mui/material/CardMedia';
import Button from '@mui/material/Button';
import Typography from '@mui/material/Typography';
import { Link } from 'react-router-dom';
import { OpenInNew } from '@mui/icons-material';
import './TemplateCard.css';
import { Template } from '../Model/Template';
import { Box, ButtonGroup, Chip, Divider, Stack } from '@mui/material';


export interface TemplateCardProperties {
    knowledgeLibraryId: string,
    template: Template
}

export default function TemplateCard( props : TemplateCardProperties) {


    function renderTags( tags: Array<string>)
    {
        return tags.map( tag => {
            return <Chip label={tag} size="small" variant="filled" sx={{width:"auto"}} /> 
        })
    }

    function renderCard(name: string, description: string, gitUrl: string, cardActionAreaTo: string, tags: Array<string>)
    {
        let tagChips = renderTags(tags);

        return (
            <Card sx={{ display: 'flex', flexDirection:"row", borderLeft:"3px solid", minWidth:"400px", maxWidth:"800px", borderColor:'primary.main', minHeight:"150px", maxHeight:"150px" }}>
                <CardMedia
                    component="img"
                    sx={{ flex:"0 1 0", objectFit: "contain", height:"100%" }}
                    image="/src/assets/template.png"
                    alt="Template" />  
                <Box sx={{ flex: '1 1 auto', position:"relative"}}>
                    <CardContent sx={{padding:"8px"}}  >
                        <Typography sx={{flex:"0 0"}} component="div" variant="h5">
                            {name}
                            <Stack direction="row" sx={{marginLeft:"10px" }} display="inline-block" spacing="3px" >
                                {tagChips}
                            </Stack>
                        </Typography>
                        <Typography sx={{overflow: "hidden"}} variant="subtitle1" color="text.secondary" component="div">
                            {description}
                        </Typography>
                    </CardContent>
                    <CardActions sx={{position:"absolute", bgcolor: 'background.paper', bottom:0, width:"100%"}} >
                        <div className='buttons'>
                        <Button component={Link as any} target="_blank" underline="hover" to={gitUrl}>Git<OpenInNew fontSize="inherit" /></Button>
                            <Button size="small">Skapa projekt</Button>
                        </div>
                    </CardActions>
                </Box>
                
            </Card>
        );
    }
    
    let navigateToUrl = `/knowledgeLibrary/${props.knowledgeLibraryId}/template/${props.template.fileId}`;

    let card = renderCard(props.template.name, props.template.description, props.template.html, navigateToUrl, props.template.tags)
    
    return card;
}