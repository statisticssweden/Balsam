/*
 * S3Provider
 *
 * This a service contract for the OicdProvider in Balsam.
 *
 * The version of the OpenAPI document: 2.0
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */

using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using RestSharp;
using Xunit;

using S3ProviderApiClient.Client;
using S3ProviderApiClient.Api;
// uncomment below to import models
//using S3ProviderApiClient.Model;

namespace S3ProviderApiClient.Test.Api
{
    /// <summary>
    ///  Class for testing BucketApi
    /// </summary>
    /// <remarks>
    /// This file is automatically generated by OpenAPI Generator (https://openapi-generator.tech).
    /// Please update the test case below to test the API endpoint.
    /// </remarks>
    public class BucketApiTests : IDisposable
    {
        private BucketApi instance;

        public BucketApiTests()
        {
            instance = new BucketApi();
        }

        public void Dispose()
        {
            // Cleanup when everything is done.
        }

        /// <summary>
        /// Test an instance of BucketApi
        /// </summary>
        [Fact]
        public void InstanceTest()
        {
            // TODO uncomment below to test 'IsType' BucketApi
            //Assert.IsType<BucketApi>(instance);
        }

        /// <summary>
        /// Test CreateAccessKey
        /// </summary>
        [Fact]
        public void CreateAccessKeyTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //string bucketId = null;
            //var response = instance.CreateAccessKey(bucketId);
            //Assert.IsType<AccessKeyCreatedResponse>(response);
        }

        /// <summary>
        /// Test CreateBucket
        /// </summary>
        [Fact]
        public void CreateBucketTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //CreateBucketRequest? createBucketRequest = null;
            //var response = instance.CreateBucket(createBucketRequest);
            //Assert.IsType<BucketCreatedResponse>(response);
        }

        /// <summary>
        /// Test CreateFolder
        /// </summary>
        [Fact]
        public void CreateFolderTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //string bucketId = null;
            //CreateFolderRequest? createFolderRequest = null;
            //var response = instance.CreateFolder(bucketId, createFolderRequest);
            //Assert.IsType<FolderCreatedResponse>(response);
        }
    }
}
