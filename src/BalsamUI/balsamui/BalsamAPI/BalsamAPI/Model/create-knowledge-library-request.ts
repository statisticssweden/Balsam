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



/**
 * Payload for creating new branch
 * @export
 * @interface CreateKnowledgeLibraryRequest
 */
export interface CreateKnowledgeLibraryRequest {
    /**
     * The name of the knowledge library
     * @type {string}
     * @memberof CreateKnowledgeLibraryRequest
     */
    'name': string;
    /**
     * The desciption of the knowledge library
     * @type {string}
     * @memberof CreateKnowledgeLibraryRequest
     */
    'description'?: string;
    /**
     * The url to the git repository
     * @type {string}
     * @memberof CreateKnowledgeLibraryRequest
     */
    'repositoryUrl': string;
}
