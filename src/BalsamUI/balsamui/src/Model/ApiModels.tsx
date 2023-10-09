export type ProjectCreatedResponse = 
{
    id: string, 
    name: string
} 

export type CreateProjectRequest = 
{
    name: string,
    description: string,
    branchName: string
} 