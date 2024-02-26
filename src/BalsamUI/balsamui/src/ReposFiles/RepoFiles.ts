import { RepoFileTypeEnum } from "../services/BalsamAPIServices";

//When response comes from aspnet api type is a number instead of RepoFileTypeEnum
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