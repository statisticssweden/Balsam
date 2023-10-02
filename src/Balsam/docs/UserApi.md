# S3ProviderApiClient.Api.UserApi

All URIs are relative to *http://git-provider.balsam-system.svc.cluster.local/api/v1*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**CreatePAT**](UserApi.md#createpat) | **POST** /users/{id}/PAT |  |

<a id="createpat"></a>
# **CreatePAT**
> UserPATCreatedResponse CreatePAT (string id)



Creates a personal access token for the user.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using S3ProviderApiClient.Api;
using S3ProviderApiClient.Client;
using S3ProviderApiClient.Model;

namespace Example
{
    public class CreatePATExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://git-provider.balsam-system.svc.cluster.local/api/v1";
            var apiInstance = new UserApi(config);
            var id = "id_example";  // string | The user identifier.

            try
            {
                UserPATCreatedResponse result = apiInstance.CreatePAT(id);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling UserApi.CreatePAT: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CreatePATWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<UserPATCreatedResponse> response = apiInstance.CreatePATWithHttpInfo(id);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling UserApi.CreatePATWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **string** | The user identifier. |  |

### Return type

[**UserPATCreatedResponse**](UserPATCreatedResponse.md)

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

