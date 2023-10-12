import KeyCloakService from "../security/KeyCloakService";
import axios from "axios";
const HttpMethods = {
    GET: "GET",
    POST: "POST",
    DELETE: "DELETE",
};

const _axios = axios.create();

const configure = () => {
    _axios.interceptors.request.use((config: any) => {
        if (KeyCloakService.IsLoggedIn()) {
            const cb = () => {
                config.headers.Authorization = `Bearer ${KeyCloakService.GetToken()}`;
                return Promise.resolve(config);
            };
            return KeyCloakService.UpdateToken(cb);
        }
    });
};

const getAxiosClient = () => _axios;


const getTextFromUrl = async (url: string): Promise<string> => {
    let response = await fetch(url);
    let text = await response.text();
    return text;

    // let textContent: string | undefined = undefined
    
    // fetch(url)
    // .then((response) => {
    //     let textPromise = response.text();
        
    //     textPromise.then((text) => {
    //         textContent = text;
    //     })
    //     .catch( () => {
    //         //Silent
    //     });
    // })
    // .catch(() => {
    //     //Silent
    // });

    // return textContent;
}

const HttpService = {
    HttpMethods,
    configure,
    getAxiosClient,
    getTextFromUrl
};

export default HttpService;