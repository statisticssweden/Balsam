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
 * File information
 * @export
 * @interface ModelFile
 */
export interface ModelFile {
    /**
     * The full relative path
     * @type {string}
     * @memberof ModelFile
     */
    'path'?: string;
    /**
     * The name
     * @type {string}
     * @memberof ModelFile
     */
    'name': string;
    /**
     * Description of the template
     * @type {string}
     * @memberof ModelFile
     */
    'type'?: ModelFileTypeEnum;
    /**
     * Url to the raw content of the file
     * @type {string}
     * @memberof ModelFile
     */
    'contentUrl'?: string;
}

export const ModelFileTypeEnum = {
    File: 'File',
    Folder: 'Folder'
} as const;

export type ModelFileTypeEnum = typeof ModelFileTypeEnum[keyof typeof ModelFileTypeEnum];

