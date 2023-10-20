import HttpService from './HttpServices';
import { Configuration } from '../../BalsamAPI/configuration';
import { ProjectApi, WorkspaceApi } from '../../BalsamAPI/api'

export interface BalsamAPI {
   projectApi: ProjectApi,
   workspaceApi: WorkspaceApi
}

export const getBalsamAPI = (apiUrl: string): BalsamAPI => {
    const configuration = new Configuration({
        basePath: apiUrl, 
      });

    return {
        //TODO: Should they use the same axios client?
        projectApi:  new ProjectApi(configuration, apiUrl, HttpService.getAxiosClient()),
        workspaceApi:  new WorkspaceApi(configuration, apiUrl, HttpService.getAxiosClient())
    };
}

//Export the model for ease of use
export * from "../../BalsamAPI/BalsamAPI/Model";