# GitProviderApiClient.Api.RepositoryApi

All URIs are relative to *http://git-provider.balsam-system.svc.cluster.local/api/v1*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**CreateBranch**](RepositoryApi.md#createbranch) | **POST** /repos/{repositoryId}/branches |  |
| [**CreateRepository**](RepositoryApi.md#createrepository) | **POST** /repos |  |
| [**GetFilesInBranch**](RepositoryApi.md#getfilesinbranch) | **GET** /repos/{repositoryId}/branches/{branchId}/files |  |

<a id="createbranch"></a>
# **CreateBranch**
> BranchCreatedResponse CreateBranch (string repositoryId, CreateBranchRequest? createBranchRequest = null)



Create a branch from main branch in a existing repository

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using GitProviderApiClient.Api;
using GitProviderApiClient.Client;
using GitProviderApiClient.Model;

namespace Example
{
    public class CreateBranchExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://git-provider.balsam-system.svc.cluster.local/api/v1";
            var apiInstance = new RepositoryApi(config);
            var repositoryId = "repositoryId_example";  // string | The name of the repository where the branch should be created.
            var createBranchRequest = new CreateBranchRequest?(); // CreateBranchRequest? | Definition of a new repository (optional) 

            try
            {
                BranchCreatedResponse result = apiInstance.CreateBranch(repositoryId, createBranchRequest);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling RepositoryApi.CreateBranch: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CreateBranchWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<BranchCreatedResponse> response = apiInstance.CreateBranchWithHttpInfo(repositoryId, createBranchRequest);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling RepositoryApi.CreateBranchWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **repositoryId** | **string** | The name of the repository where the branch should be created. |  |
| **createBranchRequest** | [**CreateBranchRequest?**](CreateBranchRequest?.md) | Definition of a new repository | [optional]  |

### Return type

[**BranchCreatedResponse**](BranchCreatedResponse.md)

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

<a id="createrepository"></a>
# **CreateRepository**
> RepositoryCreatedResponse CreateRepository (CreateRepositoryRequest? createRepositoryRequest = null)



Creates a new repository

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using GitProviderApiClient.Api;
using GitProviderApiClient.Client;
using GitProviderApiClient.Model;

namespace Example
{
    public class CreateRepositoryExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://git-provider.balsam-system.svc.cluster.local/api/v1";
            var apiInstance = new RepositoryApi(config);
            var createRepositoryRequest = new CreateRepositoryRequest?(); // CreateRepositoryRequest? | Definition of a new repository (optional) 

            try
            {
                RepositoryCreatedResponse result = apiInstance.CreateRepository(createRepositoryRequest);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling RepositoryApi.CreateRepository: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CreateRepositoryWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<RepositoryCreatedResponse> response = apiInstance.CreateRepositoryWithHttpInfo(createRepositoryRequest);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling RepositoryApi.CreateRepositoryWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **createRepositoryRequest** | [**CreateRepositoryRequest?**](CreateRepositoryRequest?.md) | Definition of a new repository | [optional]  |

### Return type

[**RepositoryCreatedResponse**](RepositoryCreatedResponse.md)

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

<a id="getfilesinbranch"></a>
# **GetFilesInBranch**
> List&lt;File&gt; GetFilesInBranch (string repositoryId, string branchId)



Gets file descriptions of all files in a git branch for specified repository

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using GitProviderApiClient.Api;
using GitProviderApiClient.Client;
using GitProviderApiClient.Model;

namespace Example
{
    public class GetFilesInBranchExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://git-provider.balsam-system.svc.cluster.local/api/v1";
            var apiInstance = new RepositoryApi(config);
            var repositoryId = "repositoryId_example";  // string | The id of the repository.
            var branchId = "branchId_example";  // string | The id of the branch.

            try
            {
                List<File> result = apiInstance.GetFilesInBranch(repositoryId, branchId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling RepositoryApi.GetFilesInBranch: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetFilesInBranchWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<File>> response = apiInstance.GetFilesInBranchWithHttpInfo(repositoryId, branchId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling RepositoryApi.GetFilesInBranchWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **repositoryId** | **string** | The id of the repository. |  |
| **branchId** | **string** | The id of the branch. |  |

### Return type

[**List&lt;File&gt;**](File.md)

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

