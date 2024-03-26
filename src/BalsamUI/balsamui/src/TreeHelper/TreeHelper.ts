import pathListToTree, { TreeNode } from 'path-list-to-tree';
import { RepoFile, RepoFileTypeEnum } from '../services/BalsamAPIServices';
import { toRepoFileTypeEnum } from '../ReposFiles/RepoFiles';

export interface FileTreeNode 
{
    path: string,
    name: string,
    fileId: string,
    isFolder: boolean,
    children: FileTreeNode[]
}

function toFileTree(tree: Array<TreeNode>, files: Array<RepoFile>, currentPath: string) : FileTreeNode[]
{
    //TODO: This is a fix because pathListToTree returns empty node
    let cleanTree = tree.filter(f => f.name !== undefined);

    return cleanTree.map((node) => {
        let path = currentPath.length > 0 ?  currentPath + "/" + node.name : node.name;
        let file = files.find((f) => {return f.path === path} );
        let type = file?.type ?? RepoFileTypeEnum.Folder;
        let fileNode: FileTreeNode = 
        {
            path: path,
            name: node.name,
            fileId: file?.path || "", //TODO: make safer
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

export function getAllIds(tree: FileTreeNode[]) : string[]
{
    return tree.flatMap((node) => {
        let array : string[] = [node.path]
        return array.concat(getAllIds(node.children))
    });
}


const TreeHelper =
{
    convertToFileTreeNodes,
    getAllIds
}

export default TreeHelper