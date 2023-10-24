import { Card, CardContent, CardMedia, Fab, Skeleton, Typography } from "@mui/material";
import { Fragment, PropsWithChildren } from "react";
import './NewCard.css';

export interface NewCardProperties<KeyType>{
    buttonContent?: any;
    image: string;
    onClick?: (itemKey: any) => void;
    itemKey: KeyType;
}

export default function NewCard<KeyType>(props: PropsWithChildren<NewCardProperties<KeyType>>)
{
    function onClick(){
        if (props.onClick)
        {
            props.onClick(props.itemKey);
        }
    }


    function renderContent()
    {
        if(props.children )
        {
            return props.children
        }
        else {
            return <>
                        <Typography gutterBottom variant="h6" component="div">
                            <Skeleton sx={{ width: "30%" }} animation="wave"></Skeleton>
                        </Typography>
                        <Typography variant="body2" color="text.secondary">
                            <Skeleton sx={{ width: "50%" }} animation="wave"></Skeleton>
                        </Typography>
                    </>
        }
    }

    const content = renderContent();


    return (<Fragment>
        <div className="addCardContainer">
            <div className='background'>
                <Card sx={{ height:300, maxWidth: 300, minWidth: 300, display:"block" }}>
                    <CardMedia
                        className='inactive'
                        sx={{ height: 140 }}
                        image={props.image}
                        title="Bearbetningsmiljö"
                    />
                    <CardContent className="cardContent">
                        {content}
                    </CardContent>
                </Card>
            </div>
            <div className='foreground'>
                
                <Fab sx={{ display:"block", margin:"auto", position:"relative"}} onClick={onClick} variant="extended" className='workspaceAddButton' size="medium" color="primary" aria-label="add">
                    {props.buttonContent}
                </Fab>        
            </div>
        </div>
    </Fragment>);
}