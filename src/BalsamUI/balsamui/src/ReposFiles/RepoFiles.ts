import { RepoFileTypeEnum } from "../services/BalsamAPIServices";

export function toNumber(repoFileTypeEnum: RepoFileTypeEnum) : number {
    switch(repoFileTypeEnum)
    {
        case RepoFileTypeEnum.File:
            return 1;
        case RepoFileTypeEnum.Folder:
            return 2;
        default:
            throw(`${repoFileTypeEnum} not implemented in toNumber`);
    }
}


//When response comes from aspnet type is number instead of RepoFileTypeEnum
export function toRepoFileTypeEnum(type: RepoFileTypeEnum | number ) : RepoFileTypeEnum
{
    if (Number.isInteger(type))
    {
        switch(type)
        {
            case 1:
                return RepoFileTypeEnum.File
            case 2:
                return RepoFileTypeEnum.Folder
            default:
                throw(`${type} is not a part of enum RepoFileTypeEnum`);
        }
    }
    else 
    {
        return type as RepoFileTypeEnum;
    }
}