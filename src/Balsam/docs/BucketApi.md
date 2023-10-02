# S3ProviderApiClient.Api.BucketApi

All URIs are relative to *http://s3-provider.balsam-system.svc.cluster.local/api/v1*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**CreateAccessKey**](BucketApi.md#createaccesskey) | **POST** /buckets/{bucketId}/acceskey |  |
| [**CreateBucket**](BucketApi.md#createbucket) | **POST** /buckets |  |
| [**CreateFolder**](BucketApi.md#createfolder) | **POST** /buckets/{bucketId}/folder |  |

<a id="createaccesskey"></a>
# **CreateAccessKey**
> AccessKeyCreatedResponse CreateAccessKey (string bucketId)



Creates a new acces key for the bucket.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using S3ProviderApiClient.Api;
using S3ProviderApiClient.Client;
using S3ProviderApiClient.Model;

namespace Example
{
    public class CreateAccessKeyExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://s3-provider.balsam-system.svc.cluster.local/api/v1";
            var apiInstance = new BucketApi(config);
            var bucketId = "bucketId_example";  // string | the name of the bucket

            try
            {
                AccessKeyCreatedResponse result = apiInstance.CreateAccessKey(bucketId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling BucketApi.CreateAccessKey: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CreateAccessKeyWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<AccessKeyCreatedResponse> response = apiInstance.CreateAccessKeyWithHttpInfo(bucketId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling BucketApi.CreateAccessKeyWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **bucketId** | **string** | the name of the bucket |  |

### Return type

[**AccessKeyCreatedResponse**](AccessKeyCreatedResponse.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json, application/problem+json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Success |  -  |
| **400** | Error respsone for 400 |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="createbucket"></a>
# **CreateBucket**
> BucketCreatedResponse CreateBucket (CreateBucketRequest? createBucketRequest = null)



Creates a new Bucket

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using S3ProviderApiClient.Api;
using S3ProviderApiClient.Client;
using S3ProviderApiClient.Model;

namespace Example
{
    public class CreateBucketExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://s3-provider.balsam-system.svc.cluster.local/api/v1";
            var apiInstance = new BucketApi(config);
            var createBucketRequest = new CreateBucketRequest?(); // CreateBucketRequest? | Definition of a new role (optional) 

            try
            {
                BucketCreatedResponse result = apiInstance.CreateBucket(createBucketRequest);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling BucketApi.CreateBucket: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CreateBucketWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<BucketCreatedResponse> response = apiInstance.CreateBucketWithHttpInfo(createBucketRequest);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling BucketApi.CreateBucketWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **createBucketRequest** | [**CreateBucketRequest?**](CreateBucketRequest?.md) | Definition of a new role | [optional]  |

### Return type

[**BucketCreatedResponse**](BucketCreatedResponse.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json, application/problem+json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Success |  -  |
| **400** | Error respsone for 400 |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="createfolder"></a>
# **CreateFolder**
> FolderCreatedResponse CreateFolder (string bucketId, CreateFolderRequest? createFolderRequest = null)



Creates a new virtual folder in the bucket.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using S3ProviderApiClient.Api;
using S3ProviderApiClient.Client;
using S3ProviderApiClient.Model;

namespace Example
{
    public class CreateFolderExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://s3-provider.balsam-system.svc.cluster.local/api/v1";
            var apiInstance = new BucketApi(config);
            var bucketId = "bucketId_example";  // string | the name of the bucket
            var createFolderRequest = new CreateFolderRequest?(); // CreateFolderRequest? | Definition of a new role (optional) 

            try
            {
                FolderCreatedResponse result = apiInstance.CreateFolder(bucketId, createFolderRequest);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling BucketApi.CreateFolder: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CreateFolderWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<FolderCreatedResponse> response = apiInstance.CreateFolderWithHttpInfo(bucketId, createFolderRequest);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling BucketApi.CreateFolderWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **bucketId** | **string** | the name of the bucket |  |
| **createFolderRequest** | [**CreateFolderRequest?**](CreateFolderRequest?.md) | Definition of a new role | [optional]  |

### Return type

[**FolderCreatedResponse**](FolderCreatedResponse.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json, application/problem+json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Success |  -  |
| **400** | Error respsone for 400 |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

