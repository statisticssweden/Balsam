# RocketChatChatProviderApiClient.Api.AreaApi

All URIs are relative to *http://chat-provider.balsam-system.svc.cluster.local/api/v1*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**CreateArea**](AreaApi.md#createarea) | **POST** /areas |  |

<a id="createarea"></a>
# **CreateArea**
> AreaCreatedResponse CreateArea (CreateAreaRequest? createAreaRequest = null)



Creates a new Area

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using RocketChatChatProviderApiClient.Api;
using RocketChatChatProviderApiClient.Client;
using RocketChatChatProviderApiClient.Model;

namespace Example
{
    public class CreateAreaExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://chat-provider.balsam-system.svc.cluster.local/api/v1";
            var apiInstance = new AreaApi(config);
            var createAreaRequest = new CreateAreaRequest?(); // CreateAreaRequest? | Definition of a new area (optional) 

            try
            {
                AreaCreatedResponse result = apiInstance.CreateArea(createAreaRequest);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling AreaApi.CreateArea: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CreateAreaWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<AreaCreatedResponse> response = apiInstance.CreateAreaWithHttpInfo(createAreaRequest);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling AreaApi.CreateAreaWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **createAreaRequest** | [**CreateAreaRequest?**](CreateAreaRequest?.md) | Definition of a new area | [optional]  |

### Return type

[**AreaCreatedResponse**](AreaCreatedResponse.md)

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

