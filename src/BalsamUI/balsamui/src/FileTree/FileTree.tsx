import { TreeItem, TreeView } from "@mui/x-tree-view";
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import ChevronRightIcon from '@mui/icons-material/ChevronRight';
import FolderIcon from '@mui/icons-material/Folder';
import DescriptionIcon from '@mui/icons-material/Description';
import { PropsWithChildren } from "react";
import { RepoFile, RepoFileTypeEnum } from "../services/BalsamAPIServices";
import pathListToTree, { TreeNode } from 'path-list-to-tree';
import { toRepoFileTypeEnum } from "../ReposFiles/RepoFiles";
// import { toNumber } from "../ReposFiles/RepoFiles";

export interface FileTreeNode 
    {
        path: string,
        name: string,
        isFolder: boolean,
        children: FileTreeNode[]
    }

function toFileTree(tree: TreeNode[], files: Array<RepoFile>, currentPath: string) : FileTreeNode[]
{
    //TODO: This is a fix because pathListToTree returns empty node
    let cleanTree = tree.filter(f => f.name !== undefined);

    return cleanTree.map((node) => {
        let path = currentPath.length > 0 ?  currentPath + "/" + node.name : node.name;
        let type = files.find((f) => {return f.path === path} )?.type ?? RepoFileTypeEnum.File;
        let fileNode: FileTreeNode = 
        {
            path: path,
            name: node.name,
            children: toFileTree(node.children, files, path),
            isFolder: toRepoFileTypeEnum(type) === RepoFileTypeEnum.Folder
        }

        return fileNode;

    });
}
export function convertToFileTreeNodes(files: RepoFile[])
{
    let filePaths = files.map((f) => f.path);
    let tree = pathListToTree(filePaths);
    let fileTree = toFileTree(tree, files, "");
    return fileTree;
}

export interface FileTreeProperties
{
    defaultExpanded?: string[],
    fileTree: FileTreeNode[],
    
}

export function getAllIds(tree: FileTreeNode[]) : string[]
{
    return tree.flatMap((node) => {
        let array : string[] = [node.path]
        return array.concat(getAllIds(node.children))

    });
}

export default function FileTree(props: PropsWithChildren<FileTreeProperties>)
{
    function renderFileTreeLabel(node: FileTreeNode)
    {
        let style = {position:"relative", top:"3px", marginRight:"8px" };
        return (
            <div>
                {node.isFolder ? <FolderIcon sx={style} fontSize='small'></FolderIcon> : 
                     <DescriptionIcon sx={style} fontSize='small'></DescriptionIcon>}
                {node.name}
            </div>);
    }


    const renderTree = (tree: FileTreeNode[]) => {
        
        return (tree.map((node) => {
            return (<TreeItem key={node.path} nodeId={node.path} label={renderFileTreeLabel(node)}>
                
                {Array.isArray(node.children) ? renderTree(node.children) : null}
              </TreeItem>) 

        }));
        
    };

    return (<TreeView
        aria-label="file system navigator"
        defaultCollapseIcon={<ExpandMoreIcon />}
        defaultExpandIcon={<ChevronRightIcon />}
        defaultExpanded={props.defaultExpanded}
        sx={{ height: 240, flexGrow: 1, maxWidth: 400, overflowY: 'auto' }}
      >
        {renderTree(props.fileTree)}
        {props.children}
      </TreeView>)

}
