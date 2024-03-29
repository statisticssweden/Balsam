/* tslint:disable */
/* eslint-disable */
/**
 * BalsamApi
 * This is the API for createing Baslam artifcats.
 *
 * The version of the OpenAPI document: 2.0
 * 
 *
 * NOTE: This class is auto generated by OpenAPI Generator (https://openapi-generator.tech).
 * https://openapi-generator.tech
 * Do not edit the class manually.
 */


import type { Configuration } from '../../configuration';
import type { AxiosPromise, AxiosInstance, AxiosRequestConfig } from 'axios';
import globalAxios from 'axios';
// Some imports not used depending on template conditions
// @ts-ignore
import { DUMMY_BASE_URL, assertParamExists, setApiKeyToObject, setBasicAuthToObject, setBearerAuthToObject, setOAuthToObject, setSearchParams, serializeDataIfNeeded, toPathString, createRequestFunction } from '../../common';
// @ts-ignore
import { BASE_PATH, COLLECTION_FORMATS, RequestArgs, BaseAPI, RequiredError } from '../../base';
// @ts-ignore
import { CreateWorkspaceRequest } from '../../BalsamAPI/Model';
// @ts-ignore
import { Problem } from '../../BalsamAPI/Model';
// @ts-ignore
import { Template } from '../../BalsamAPI/Model';
// @ts-ignore
import { Workspace } from '../../BalsamAPI/Model';
// @ts-ignore
import { WorkspaceCreatedResponse } from '../../BalsamAPI/Model';
/**
 * WorkspaceApi - axios parameter creator
 * @export
 */
export const WorkspaceApiAxiosParamCreator = function (configuration?: Configuration) {
    return {
        /**
         * Create a new workspace
         * @param {CreateWorkspaceRequest} [createWorkspaceRequest] Definition of a new workspace
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        createWorkspace: async (createWorkspaceRequest?: CreateWorkspaceRequest, options: AxiosRequestConfig = {}): Promise<RequestArgs> => {
            const localVarPath = `/workspaces`;
            // use dummy base URL string because the URL constructor only accepts absolute URLs.
            const localVarUrlObj = new URL(localVarPath, DUMMY_BASE_URL);
            let baseOptions;
            if (configuration) {
                baseOptions = configuration.baseOptions;
            }

            const localVarRequestOptions = { method: 'POST', ...baseOptions, ...options};
            const localVarHeaderParameter = {} as any;
            const localVarQueryParameter = {} as any;


    
            localVarHeaderParameter['Content-Type'] = 'application/json';

            setSearchParams(localVarUrlObj, localVarQueryParameter);
            let headersFromBaseOptions = baseOptions && baseOptions.headers ? baseOptions.headers : {};
            localVarRequestOptions.headers = {...localVarHeaderParameter, ...headersFromBaseOptions, ...options.headers};
            localVarRequestOptions.data = serializeDataIfNeeded(createWorkspaceRequest, localVarRequestOptions, configuration)

            return {
                url: toPathString(localVarUrlObj),
                options: localVarRequestOptions,
            };
        },
        /**
         * Delete workspace
         * @param {string} projectId id for the project
         * @param {string} branchId id for the branch
         * @param {string} workspaceId id id for the workspace
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        deleteWorkspace: async (projectId: string, branchId: string, workspaceId: string, options: AxiosRequestConfig = {}): Promise<RequestArgs> => {
            // verify required parameter 'projectId' is not null or undefined
            assertParamExists('deleteWorkspace', 'projectId', projectId)
            // verify required parameter 'branchId' is not null or undefined
            assertParamExists('deleteWorkspace', 'branchId', branchId)
            // verify required parameter 'workspaceId' is not null or undefined
            assertParamExists('deleteWorkspace', 'workspaceId', workspaceId)
            const localVarPath = `/workspaces/{workspaceId}`
                .replace(`{${"workspaceId"}}`, encodeURIComponent(String(workspaceId)));
            // use dummy base URL string because the URL constructor only accepts absolute URLs.
            const localVarUrlObj = new URL(localVarPath, DUMMY_BASE_URL);
            let baseOptions;
            if (configuration) {
                baseOptions = configuration.baseOptions;
            }

            const localVarRequestOptions = { method: 'DELETE', ...baseOptions, ...options};
            const localVarHeaderParameter = {} as any;
            const localVarQueryParameter = {} as any;

            if (projectId !== undefined) {
                localVarQueryParameter['projectId'] = projectId;
            }

            if (branchId !== undefined) {
                localVarQueryParameter['branchId'] = branchId;
            }


    
            setSearchParams(localVarUrlObj, localVarQueryParameter);
            let headersFromBaseOptions = baseOptions && baseOptions.headers ? baseOptions.headers : {};
            localVarRequestOptions.headers = {...localVarHeaderParameter, ...headersFromBaseOptions, ...options.headers};

            return {
                url: toPathString(localVarUrlObj),
                options: localVarRequestOptions,
            };
        },
        /**
         * List available workspace templates
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        listTemplates: async (options: AxiosRequestConfig = {}): Promise<RequestArgs> => {
            const localVarPath = `/templates`;
            // use dummy base URL string because the URL constructor only accepts absolute URLs.
            const localVarUrlObj = new URL(localVarPath, DUMMY_BASE_URL);
            let baseOptions;
            if (configuration) {
                baseOptions = configuration.baseOptions;
            }

            const localVarRequestOptions = { method: 'GET', ...baseOptions, ...options};
            const localVarHeaderParameter = {} as any;
            const localVarQueryParameter = {} as any;


    
            setSearchParams(localVarUrlObj, localVarQueryParameter);
            let headersFromBaseOptions = baseOptions && baseOptions.headers ? baseOptions.headers : {};
            localVarRequestOptions.headers = {...localVarHeaderParameter, ...headersFromBaseOptions, ...options.headers};

            return {
                url: toPathString(localVarUrlObj),
                options: localVarRequestOptions,
            };
        },
        /**
         * Get workspaces
         * @param {string} [projectId] id for the project
         * @param {string} [branchId] id for the branch
         * @param {boolean} [all] If all workspaces should be returened or only workspaces that the user has assess rights too
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        listWorkspaces: async (projectId?: string, branchId?: string, all?: boolean, options: AxiosRequestConfig = {}): Promise<RequestArgs> => {
            const localVarPath = `/workspaces`;
            // use dummy base URL string because the URL constructor only accepts absolute URLs.
            const localVarUrlObj = new URL(localVarPath, DUMMY_BASE_URL);
            let baseOptions;
            if (configuration) {
                baseOptions = configuration.baseOptions;
            }

            const localVarRequestOptions = { method: 'GET', ...baseOptions, ...options};
            const localVarHeaderParameter = {} as any;
            const localVarQueryParameter = {} as any;

            if (projectId !== undefined) {
                localVarQueryParameter['projectId'] = projectId;
            }

            if (branchId !== undefined) {
                localVarQueryParameter['branchId'] = branchId;
            }

            if (all !== undefined) {
                localVarQueryParameter['all'] = all;
            }


    
            setSearchParams(localVarUrlObj, localVarQueryParameter);
            let headersFromBaseOptions = baseOptions && baseOptions.headers ? baseOptions.headers : {};
            localVarRequestOptions.headers = {...localVarHeaderParameter, ...headersFromBaseOptions, ...options.headers};

            return {
                url: toPathString(localVarUrlObj),
                options: localVarRequestOptions,
            };
        },
    }
};

/**
 * WorkspaceApi - functional programming interface
 * @export
 */
export const WorkspaceApiFp = function(configuration?: Configuration) {
    const localVarAxiosParamCreator = WorkspaceApiAxiosParamCreator(configuration)
    return {
        /**
         * Create a new workspace
         * @param {CreateWorkspaceRequest} [createWorkspaceRequest] Definition of a new workspace
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        async createWorkspace(createWorkspaceRequest?: CreateWorkspaceRequest, options?: AxiosRequestConfig): Promise<(axios?: AxiosInstance, basePath?: string) => AxiosPromise<WorkspaceCreatedResponse>> {
            const localVarAxiosArgs = await localVarAxiosParamCreator.createWorkspace(createWorkspaceRequest, options);
            return createRequestFunction(localVarAxiosArgs, globalAxios, BASE_PATH, configuration);
        },
        /**
         * Delete workspace
         * @param {string} projectId id for the project
         * @param {string} branchId id for the branch
         * @param {string} workspaceId id id for the workspace
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        async deleteWorkspace(projectId: string, branchId: string, workspaceId: string, options?: AxiosRequestConfig): Promise<(axios?: AxiosInstance, basePath?: string) => AxiosPromise<void>> {
            const localVarAxiosArgs = await localVarAxiosParamCreator.deleteWorkspace(projectId, branchId, workspaceId, options);
            return createRequestFunction(localVarAxiosArgs, globalAxios, BASE_PATH, configuration);
        },
        /**
         * List available workspace templates
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        async listTemplates(options?: AxiosRequestConfig): Promise<(axios?: AxiosInstance, basePath?: string) => AxiosPromise<Array<Template>>> {
            const localVarAxiosArgs = await localVarAxiosParamCreator.listTemplates(options);
            return createRequestFunction(localVarAxiosArgs, globalAxios, BASE_PATH, configuration);
        },
        /**
         * Get workspaces
         * @param {string} [projectId] id for the project
         * @param {string} [branchId] id for the branch
         * @param {boolean} [all] If all workspaces should be returened or only workspaces that the user has assess rights too
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        async listWorkspaces(projectId?: string, branchId?: string, all?: boolean, options?: AxiosRequestConfig): Promise<(axios?: AxiosInstance, basePath?: string) => AxiosPromise<Array<Workspace>>> {
            const localVarAxiosArgs = await localVarAxiosParamCreator.listWorkspaces(projectId, branchId, all, options);
            return createRequestFunction(localVarAxiosArgs, globalAxios, BASE_PATH, configuration);
        },
    }
};

/**
 * WorkspaceApi - factory interface
 * @export
 */
export const WorkspaceApiFactory = function (configuration?: Configuration, basePath?: string, axios?: AxiosInstance) {
    const localVarFp = WorkspaceApiFp(configuration)
    return {
        /**
         * Create a new workspace
         * @param {CreateWorkspaceRequest} [createWorkspaceRequest] Definition of a new workspace
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        createWorkspace(createWorkspaceRequest?: CreateWorkspaceRequest, options?: any): AxiosPromise<WorkspaceCreatedResponse> {
            return localVarFp.createWorkspace(createWorkspaceRequest, options).then((request) => request(axios, basePath));
        },
        /**
         * Delete workspace
         * @param {string} projectId id for the project
         * @param {string} branchId id for the branch
         * @param {string} workspaceId id id for the workspace
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        deleteWorkspace(projectId: string, branchId: string, workspaceId: string, options?: any): AxiosPromise<void> {
            return localVarFp.deleteWorkspace(projectId, branchId, workspaceId, options).then((request) => request(axios, basePath));
        },
        /**
         * List available workspace templates
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        listTemplates(options?: any): AxiosPromise<Array<Template>> {
            return localVarFp.listTemplates(options).then((request) => request(axios, basePath));
        },
        /**
         * Get workspaces
         * @param {string} [projectId] id for the project
         * @param {string} [branchId] id for the branch
         * @param {boolean} [all] If all workspaces should be returened or only workspaces that the user has assess rights too
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        listWorkspaces(projectId?: string, branchId?: string, all?: boolean, options?: any): AxiosPromise<Array<Workspace>> {
            return localVarFp.listWorkspaces(projectId, branchId, all, options).then((request) => request(axios, basePath));
        },
    };
};

/**
 * WorkspaceApi - object-oriented interface
 * @export
 * @class WorkspaceApi
 * @extends {BaseAPI}
 */
export class WorkspaceApi extends BaseAPI {
    /**
     * Create a new workspace
     * @param {CreateWorkspaceRequest} [createWorkspaceRequest] Definition of a new workspace
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof WorkspaceApi
     */
    public createWorkspace(createWorkspaceRequest?: CreateWorkspaceRequest, options?: AxiosRequestConfig) {
        return WorkspaceApiFp(this.configuration).createWorkspace(createWorkspaceRequest, options).then((request) => request(this.axios, this.basePath));
    }

    /**
     * Delete workspace
     * @param {string} projectId id for the project
     * @param {string} branchId id for the branch
     * @param {string} workspaceId id id for the workspace
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof WorkspaceApi
     */
    public deleteWorkspace(projectId: string, branchId: string, workspaceId: string, options?: AxiosRequestConfig) {
        return WorkspaceApiFp(this.configuration).deleteWorkspace(projectId, branchId, workspaceId, options).then((request) => request(this.axios, this.basePath));
    }

    /**
     * List available workspace templates
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof WorkspaceApi
     */
    public listTemplates(options?: AxiosRequestConfig) {
        return WorkspaceApiFp(this.configuration).listTemplates(options).then((request) => request(this.axios, this.basePath));
    }

    /**
     * Get workspaces
     * @param {string} [projectId] id for the project
     * @param {string} [branchId] id for the branch
     * @param {boolean} [all] If all workspaces should be returened or only workspaces that the user has assess rights too
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof WorkspaceApi
     */
    public listWorkspaces(projectId?: string, branchId?: string, all?: boolean, options?: AxiosRequestConfig) {
        return WorkspaceApiFp(this.configuration).listWorkspaces(projectId, branchId, all, options).then((request) => request(this.axios, this.basePath));
    }
}

