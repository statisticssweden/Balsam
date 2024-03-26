import { SimpleTreeView } from "@mui/x-tree-view";
import { TreeItem } from "@mui/x-tree-view/TreeItem";
import FolderIcon from '@mui/icons-material/Folder';
import DescriptionIcon from '@mui/icons-material/Description';
import { PropsWithChildren, useEffect, useState } from "react";


import { FileTreeNode } from "../TreeHelper/TreeHelper";



export interface FileTreeProperties
{
    defaultExpandedItems?: string[],
    fileTree: FileTreeNode[],
    
}

export default function FileTree(props: PropsWithChildren<FileTreeProperties>)
{
    const [expandedItems, setExpandedItems] = useState<string[]>([]);


    useEffect(() => {
        setExpandedItems(props.defaultExpandedItems || []);
        
    },[props.defaultExpandedItems]);


    function renderFileTreeLabel(node: FileTreeNode)
    {
        let style = {position:"relative", top:"3px", marginRight:"8px" };
        return (
            <div>
                {node.isFolder 
                    ? <FolderIcon sx={style} fontSize='small'></FolderIcon> 
                    : <DescriptionIcon sx={style} fontSize='small'></DescriptionIcon>}
                {node.name}
            </div>);
    }

    function onExpandedItemsChange(_event: React.SyntheticEvent<Element, Event>, itemIds: string[])
    {
        setExpandedItems(itemIds);
    }

    const renderTree = (tree: FileTreeNode[]) => {
        
        return (tree.map((node) => {
            return (
                <TreeItem key={node.path} itemId={node.path} label={renderFileTreeLabel(node)}>   
                    {Array.isArray(node.children) 
                        ? renderTree(node.children) 
                        : null}
                </TreeItem>
              ) 

        }));
    };

    return (<SimpleTreeView
        aria-label="file navigator"
        expandedItems={expandedItems}
        onExpandedItemsChange={onExpandedItemsChange}
        sx={{ height: 240, flexGrow: 1, maxWidth: 400, overflowY: 'auto' }}
      >
        {renderTree(props.fileTree)}
        {props.children}
      </SimpleTreeView>)
}
