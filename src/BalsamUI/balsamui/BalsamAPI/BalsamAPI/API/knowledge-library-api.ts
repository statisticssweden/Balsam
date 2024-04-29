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
import type { AxiosPromise, AxiosInstance, RawAxiosRequestConfig } from 'axios';
import globalAxios from 'axios';
// Some imports not used depending on template conditions
// @ts-ignore
import { DUMMY_BASE_URL, assertParamExists, setApiKeyToObject, setBasicAuthToObject, setBearerAuthToObject, setOAuthToObject, setSearchParams, serializeDataIfNeeded, toPathString, createRequestFunction } from '../../common';
// @ts-ignore
import { BASE_PATH, COLLECTION_FORMATS, RequestArgs, BaseAPI, RequiredError, operationServerMap } from '../../base';
// @ts-ignore
import { KnowledgeLibrary } from '../../BalsamAPI/Model';
// @ts-ignore
import { Problem } from '../../BalsamAPI/Model';
// @ts-ignore
import { RepoFile } from '../../BalsamAPI/Model';
/**
 * KnowledgeLibraryApi - axios parameter creator
 * @export
 */
export const KnowledgeLibraryApiAxiosParamCreator = function (configuration?: Configuration) {
    return {
        /**
         * Fetch content for file in knowledge library
         * @param {string} libraryId id for the knowledge library
         * @param {string} fileId id for the file
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        getKnowledgeLibraryFileContent: async (libraryId: string, fileId: string, options: RawAxiosRequestConfig = {}): Promise<RequestArgs> => {
            // verify required parameter 'libraryId' is not null or undefined
            assertParamExists('getKnowledgeLibraryFileContent', 'libraryId', libraryId)
            // verify required parameter 'fileId' is not null or undefined
            assertParamExists('getKnowledgeLibraryFileContent', 'fileId', fileId)
            const localVarPath = `/knowledge-libraries/{libraryId}/files/{fileId}`
                .replace(`{${"libraryId"}}`, encodeURIComponent(String(libraryId)))
                .replace(`{${"fileId"}}`, encodeURIComponent(String(fileId)));
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
         * List available knowledge Libraries
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        listKnowledgeLibaries: async (options: RawAxiosRequestConfig = {}): Promise<RequestArgs> => {
            const localVarPath = `/knowledge-libraries`;
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
         * List all files for a knowledge library
         * @param {string} libraryId id for the knowledge library
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        listKnowledgeLibraryFiles: async (libraryId: string, options: RawAxiosRequestConfig = {}): Promise<RequestArgs> => {
            // verify required parameter 'libraryId' is not null or undefined
            assertParamExists('listKnowledgeLibraryFiles', 'libraryId', libraryId)
            const localVarPath = `/knowledge-libraries/{libraryId}/files`
                .replace(`{${"libraryId"}}`, encodeURIComponent(String(libraryId)));
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
    }
};

/**
 * KnowledgeLibraryApi - functional programming interface
 * @export
 */
export const KnowledgeLibraryApiFp = function(configuration?: Configuration) {
    const localVarAxiosParamCreator = KnowledgeLibraryApiAxiosParamCreator(configuration)
    return {
        /**
         * Fetch content for file in knowledge library
         * @param {string} libraryId id for the knowledge library
         * @param {string} fileId id for the file
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        async getKnowledgeLibraryFileContent(libraryId: string, fileId: string, options?: RawAxiosRequestConfig): Promise<(axios?: AxiosInstance, basePath?: string) => AxiosPromise<string>> {
            const localVarAxiosArgs = await localVarAxiosParamCreator.getKnowledgeLibraryFileContent(libraryId, fileId, options);
            const localVarOperationServerIndex = configuration?.serverIndex ?? 0;
            const localVarOperationServerBasePath = operationServerMap['KnowledgeLibraryApi.getKnowledgeLibraryFileContent']?.[localVarOperationServerIndex]?.url;
            return (axios, basePath) => createRequestFunction(localVarAxiosArgs, globalAxios, BASE_PATH, configuration)(axios, localVarOperationServerBasePath || basePath);
        },
        /**
         * List available knowledge Libraries
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        async listKnowledgeLibaries(options?: RawAxiosRequestConfig): Promise<(axios?: AxiosInstance, basePath?: string) => AxiosPromise<Array<KnowledgeLibrary>>> {
            const localVarAxiosArgs = await localVarAxiosParamCreator.listKnowledgeLibaries(options);
            const localVarOperationServerIndex = configuration?.serverIndex ?? 0;
            const localVarOperationServerBasePath = operationServerMap['KnowledgeLibraryApi.listKnowledgeLibaries']?.[localVarOperationServerIndex]?.url;
            return (axios, basePath) => createRequestFunction(localVarAxiosArgs, globalAxios, BASE_PATH, configuration)(axios, localVarOperationServerBasePath || basePath);
        },
        /**
         * List all files for a knowledge library
         * @param {string} libraryId id for the knowledge library
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        async listKnowledgeLibraryFiles(libraryId: string, options?: RawAxiosRequestConfig): Promise<(axios?: AxiosInstance, basePath?: string) => AxiosPromise<Array<RepoFile>>> {
            const localVarAxiosArgs = await localVarAxiosParamCreator.listKnowledgeLibraryFiles(libraryId, options);
            const localVarOperationServerIndex = configuration?.serverIndex ?? 0;
            const localVarOperationServerBasePath = operationServerMap['KnowledgeLibraryApi.listKnowledgeLibraryFiles']?.[localVarOperationServerIndex]?.url;
            return (axios, basePath) => createRequestFunction(localVarAxiosArgs, globalAxios, BASE_PATH, configuration)(axios, localVarOperationServerBasePath || basePath);
        },
    }
};

/**
 * KnowledgeLibraryApi - factory interface
 * @export
 */
export const KnowledgeLibraryApiFactory = function (configuration?: Configuration, basePath?: string, axios?: AxiosInstance) {
    const localVarFp = KnowledgeLibraryApiFp(configuration)
    return {
        /**
         * Fetch content for file in knowledge library
         * @param {string} libraryId id for the knowledge library
         * @param {string} fileId id for the file
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        getKnowledgeLibraryFileContent(libraryId: string, fileId: string, options?: any): AxiosPromise<string> {
            return localVarFp.getKnowledgeLibraryFileContent(libraryId, fileId, options).then((request) => request(axios, basePath));
        },
        /**
         * List available knowledge Libraries
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        listKnowledgeLibaries(options?: any): AxiosPromise<Array<KnowledgeLibrary>> {
            return localVarFp.listKnowledgeLibaries(options).then((request) => request(axios, basePath));
        },
        /**
         * List all files for a knowledge library
         * @param {string} libraryId id for the knowledge library
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        listKnowledgeLibraryFiles(libraryId: string, options?: any): AxiosPromise<Array<RepoFile>> {
            return localVarFp.listKnowledgeLibraryFiles(libraryId, options).then((request) => request(axios, basePath));
        },
    };
};

/**
 * KnowledgeLibraryApi - object-oriented interface
 * @export
 * @class KnowledgeLibraryApi
 * @extends {BaseAPI}
 */
export class KnowledgeLibraryApi extends BaseAPI {
    /**
     * Fetch content for file in knowledge library
     * @param {string} libraryId id for the knowledge library
     * @param {string} fileId id for the file
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof KnowledgeLibraryApi
     */
    public getKnowledgeLibraryFileContent(libraryId: string, fileId: string, options?: RawAxiosRequestConfig) {
        return KnowledgeLibraryApiFp(this.configuration).getKnowledgeLibraryFileContent(libraryId, fileId, options).then((request) => request(this.axios, this.basePath));
    }

    /**
     * List available knowledge Libraries
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof KnowledgeLibraryApi
     */
    public listKnowledgeLibaries(options?: RawAxiosRequestConfig) {
        return KnowledgeLibraryApiFp(this.configuration).listKnowledgeLibaries(options).then((request) => request(this.axios, this.basePath));
    }

    /**
     * List all files for a knowledge library
     * @param {string} libraryId id for the knowledge library
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof KnowledgeLibraryApi
     */
    public listKnowledgeLibraryFiles(libraryId: string, options?: RawAxiosRequestConfig) {
        return KnowledgeLibraryApiFp(this.configuration).listKnowledgeLibraryFiles(libraryId, options).then((request) => request(this.axios, this.basePath));
    }
}

