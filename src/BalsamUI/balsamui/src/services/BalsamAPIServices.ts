import HttpService from './HttpServices';
import { Configuration } from '../../BalsamAPI/configuration';
import { ProjectApi } from '../../BalsamAPI/api'

export interface BalsamAPI {
   projectApi: ProjectApi
}

export const getBalsamAPI = (apiUrl: string): BalsamAPI => {
    const configuration = new Configuration({
        basePath: apiUrl, 
      });

    return {
        projectApi:  new ProjectApi(configuration, apiUrl, HttpService.getAxiosClient())
    };
}

//Export the model for ease of use
export * from "../../BalsamAPI/BalsamAPI/Model";