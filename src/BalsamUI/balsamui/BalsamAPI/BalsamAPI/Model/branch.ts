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
 * 
 * @export
 * @interface Branch
 */
export interface Branch {
    /**
     * The identifier
     * @type {string}
     * @memberof Branch
     */
    'id': string;
    /**
     * The name
     * @type {string}
     * @memberof Branch
     */
    'name': string;
    /**
     * Description of the branch
     * @type {string}
     * @memberof Branch
     */
    'description'?: string;
    /**
     * If the branch is the default branch of the project
     * @type {boolean}
     * @memberof Branch
     */
    'isDefault': boolean;
}

