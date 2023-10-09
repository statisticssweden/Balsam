import HttpService from './HttpServices';
import { Configuration } from '../../BalsamAPI/configuration';
import { ProjectApi } from '../../BalsamAPI/api'

const apiMockUrl = "http://balsam-api-mock.tanzu.scb.intra/api/v1";

const configuration = new Configuration({
    basePath: apiMockUrl, 
  });

const projectApi = new ProjectApi(configuration, apiMockUrl, HttpService.getAxiosClient() );

const BalsamAPI = {
    projectApi
}

export default BalsamAPI

//Export the model for ease of use
export * from "../../BalsamAPI/BalsamAPI/Model";