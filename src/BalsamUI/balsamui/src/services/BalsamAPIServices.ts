import HttpService from './HttpServices';
import { Configuration } from '../../BalsamAPI/configuration';
import { ProjectApi } from '../../BalsamAPI/api'
import config from '../configuration/configuration';

const configuration = new Configuration({
    basePath: config.apiurl, 
  });

const projectApi = new ProjectApi(configuration, config.apiurl, HttpService.getAxiosClient() );

const BalsamAPI = {
    projectApi
}

export default BalsamAPI

//Export the model for ease of use
export * from "../../BalsamAPI/BalsamAPI/Model";