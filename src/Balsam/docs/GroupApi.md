# OidcProviderApiClient.Api.GroupApi

All URIs are relative to *http://oidc-provider.balsam-system.svc.cluster.local/api/v1*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**AddUserToGroup**](GroupApi.md#addusertogroup) | **POST** /groups/{groupId}/users |  |
| [**CreateGroup**](GroupApi.md#creategroup) | **POST** /groups |  |

<a id="addusertogroup"></a>
# **AddUserToGroup**
> void AddUserToGroup (string groupId, AddUserToGroupRequest? addUserToGroupRequest = null)



Adds a user to the group

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using OidcProviderApiClient.Api;
using OidcProviderApiClient.Client;
using OidcProviderApiClient.Model;

namespace Example
{
    public class AddUserToGroupExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://oidc-provider.balsam-system.svc.cluster.local/api/v1";
            var apiInstance = new GroupApi(config);
            var groupId = "groupId_example";  // string | The id for the group
            var addUserToGroupRequest = new AddUserToGroupRequest?(); // AddUserToGroupRequest? | Definition of the user to add to the group (optional) 

            try
            {
                apiInstance.AddUserToGroup(groupId, addUserToGroupRequest);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling GroupApi.AddUserToGroup: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the AddUserToGroupWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.AddUserToGroupWithHttpInfo(groupId, addUserToGroupRequest);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling GroupApi.AddUserToGroupWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **groupId** | **string** | The id for the group |  |
| **addUserToGroupRequest** | [**AddUserToGroupRequest?**](AddUserToGroupRequest?.md) | Definition of the user to add to the group | [optional]  |

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/problem+json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Success |  -  |
| **400** | Error respsone for 400 |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="creategroup"></a>
# **CreateGroup**
> GroupCreatedResponse CreateGroup (CreateGroupRequest? createGroupRequest = null)



Creates a new group

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using OidcProviderApiClient.Api;
using OidcProviderApiClient.Client;
using OidcProviderApiClient.Model;

namespace Example
{
    public class CreateGroupExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://oidc-provider.balsam-system.svc.cluster.local/api/v1";
            var apiInstance = new GroupApi(config);
            var createGroupRequest = new CreateGroupRequest?(); // CreateGroupRequest? | Definition of a new group (optional) 

            try
            {
                GroupCreatedResponse result = apiInstance.CreateGroup(createGroupRequest);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling GroupApi.CreateGroup: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CreateGroupWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<GroupCreatedResponse> response = apiInstance.CreateGroupWithHttpInfo(createGroupRequest);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling GroupApi.CreateGroupWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **createGroupRequest** | [**CreateGroupRequest?**](CreateGroupRequest?.md) | Definition of a new group | [optional]  |

### Return type

[**GroupCreatedResponse**](GroupCreatedResponse.md)

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

