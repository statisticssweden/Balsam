import HttpService from './HttpServices';
import { ProjectCreatedResponse, CreateProjectRequest } from '../Model/ApiModels';

const apiMockUrl = "http://balsam-api-mock.tanzu.scb.intra/api/v1";



const getProjects = async () : Promise<any[]> => {
    let promise = HttpService.getAxiosClient()
            .get(apiMockUrl + "/projects");


    promise.catch((reason: any) => 
        {
            alert(reason) ;
            throw new Error("Could not load projects");
        })

    let response = await promise;

    return response.data.projects;
};

const postProject = async (project: CreateProjectRequest) : Promise<ProjectCreatedResponse> => {
    let promise = HttpService.getAxiosClient()
    .post(apiMockUrl + "/projects", project);

    promise.catch((reason: any) => 
    {
        alert(reason) ;
        throw new Error("Could not load projects");
    })

    let response = await promise;

    return response.data as ProjectCreatedResponse;
}

//HttpService.getAxiosClient();
            // .get(apiMockUrl + "/projects")
            // .then(
            //     (p) => callback(p.data),
            //     (e) => alert(e.message) 
            // );

const BalsamApi = {
    getProjects,
    postProject
}

export default BalsamApi