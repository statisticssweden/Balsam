/*
 * OidcProvider
 *
 * This a service contract for the OidcProvider in Balsam.
 *
 * The version of the OpenAPI document: 2.0
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Mime;
using OidcProviderApiClient.Client;
using OidcProviderApiClient.Model;

namespace OidcProviderApiClient.Api
{

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface IGroupApiSync : IApiAccessor
    {
        #region Synchronous Operations
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Adds a user to the group
        /// </remarks>
        /// <exception cref="OidcProviderApiClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="groupId">The id for the group</param>
        /// <param name="addUserToGroupRequest">Definition of the user to add to the group (optional)</param>
        /// <param name="operationIndex">Index associated with the operation.</param>
        /// <returns></returns>
        void AddUserToGroup(string groupId, AddUserToGroupRequest? addUserToGroupRequest = default(AddUserToGroupRequest?), int operationIndex = 0);

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Adds a user to the group
        /// </remarks>
        /// <exception cref="OidcProviderApiClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="groupId">The id for the group</param>
        /// <param name="addUserToGroupRequest">Definition of the user to add to the group (optional)</param>
        /// <param name="operationIndex">Index associated with the operation.</param>
        /// <returns>ApiResponse of Object(void)</returns>
        ApiResponse<Object> AddUserToGroupWithHttpInfo(string groupId, AddUserToGroupRequest? addUserToGroupRequest = default(AddUserToGroupRequest?), int operationIndex = 0);
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Creates a new group
        /// </remarks>
        /// <exception cref="OidcProviderApiClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="createGroupRequest">Definition of a new group (optional)</param>
        /// <param name="operationIndex">Index associated with the operation.</param>
        /// <returns>GroupCreatedResponse</returns>
        GroupCreatedResponse CreateGroup(CreateGroupRequest? createGroupRequest = default(CreateGroupRequest?), int operationIndex = 0);

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Creates a new group
        /// </remarks>
        /// <exception cref="OidcProviderApiClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="createGroupRequest">Definition of a new group (optional)</param>
        /// <param name="operationIndex">Index associated with the operation.</param>
        /// <returns>ApiResponse of GroupCreatedResponse</returns>
        ApiResponse<GroupCreatedResponse> CreateGroupWithHttpInfo(CreateGroupRequest? createGroupRequest = default(CreateGroupRequest?), int operationIndex = 0);
        #endregion Synchronous Operations
    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface IGroupApiAsync : IApiAccessor
    {
        #region Asynchronous Operations
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Adds a user to the group
        /// </remarks>
        /// <exception cref="OidcProviderApiClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="groupId">The id for the group</param>
        /// <param name="addUserToGroupRequest">Definition of the user to add to the group (optional)</param>
        /// <param name="operationIndex">Index associated with the operation.</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of void</returns>
        System.Threading.Tasks.Task AddUserToGroupAsync(string groupId, AddUserToGroupRequest? addUserToGroupRequest = default(AddUserToGroupRequest?), int operationIndex = 0, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Adds a user to the group
        /// </remarks>
        /// <exception cref="OidcProviderApiClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="groupId">The id for the group</param>
        /// <param name="addUserToGroupRequest">Definition of the user to add to the group (optional)</param>
        /// <param name="operationIndex">Index associated with the operation.</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of ApiResponse</returns>
        System.Threading.Tasks.Task<ApiResponse<Object>> AddUserToGroupWithHttpInfoAsync(string groupId, AddUserToGroupRequest? addUserToGroupRequest = default(AddUserToGroupRequest?), int operationIndex = 0, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Creates a new group
        /// </remarks>
        /// <exception cref="OidcProviderApiClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="createGroupRequest">Definition of a new group (optional)</param>
        /// <param name="operationIndex">Index associated with the operation.</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of GroupCreatedResponse</returns>
        System.Threading.Tasks.Task<GroupCreatedResponse> CreateGroupAsync(CreateGroupRequest? createGroupRequest = default(CreateGroupRequest?), int operationIndex = 0, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Creates a new group
        /// </remarks>
        /// <exception cref="OidcProviderApiClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="createGroupRequest">Definition of a new group (optional)</param>
        /// <param name="operationIndex">Index associated with the operation.</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of ApiResponse (GroupCreatedResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<GroupCreatedResponse>> CreateGroupWithHttpInfoAsync(CreateGroupRequest? createGroupRequest = default(CreateGroupRequest?), int operationIndex = 0, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));
        #endregion Asynchronous Operations
    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface IGroupApi : IGroupApiSync, IGroupApiAsync
    {

    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public partial class GroupApi : IGroupApi
    {
        private OidcProviderApiClient.Client.ExceptionFactory _exceptionFactory = (name, response) => null;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupApi"/> class.
        /// </summary>
        /// <returns></returns>
        public GroupApi() : this((string)null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupApi"/> class.
        /// </summary>
        /// <returns></returns>
        public GroupApi(string basePath)
        {
            this.Configuration = OidcProviderApiClient.Client.Configuration.MergeConfigurations(
                OidcProviderApiClient.Client.GlobalConfiguration.Instance,
                new OidcProviderApiClient.Client.Configuration { BasePath = basePath }
            );
            this.Client = new OidcProviderApiClient.Client.ApiClient(this.Configuration.BasePath);
            this.AsynchronousClient = new OidcProviderApiClient.Client.ApiClient(this.Configuration.BasePath);
            this.ExceptionFactory = OidcProviderApiClient.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupApi"/> class
        /// using Configuration object
        /// </summary>
        /// <param name="configuration">An instance of Configuration</param>
        /// <returns></returns>
        public GroupApi(OidcProviderApiClient.Client.Configuration configuration)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");

            this.Configuration = OidcProviderApiClient.Client.Configuration.MergeConfigurations(
                OidcProviderApiClient.Client.GlobalConfiguration.Instance,
                configuration
            );
            this.Client = new OidcProviderApiClient.Client.ApiClient(this.Configuration.BasePath);
            this.AsynchronousClient = new OidcProviderApiClient.Client.ApiClient(this.Configuration.BasePath);
            ExceptionFactory = OidcProviderApiClient.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupApi"/> class
        /// using a Configuration object and client instance.
        /// </summary>
        /// <param name="client">The client interface for synchronous API access.</param>
        /// <param name="asyncClient">The client interface for asynchronous API access.</param>
        /// <param name="configuration">The configuration object.</param>
        public GroupApi(OidcProviderApiClient.Client.ISynchronousClient client, OidcProviderApiClient.Client.IAsynchronousClient asyncClient, OidcProviderApiClient.Client.IReadableConfiguration configuration)
        {
            if (client == null) throw new ArgumentNullException("client");
            if (asyncClient == null) throw new ArgumentNullException("asyncClient");
            if (configuration == null) throw new ArgumentNullException("configuration");

            this.Client = client;
            this.AsynchronousClient = asyncClient;
            this.Configuration = configuration;
            this.ExceptionFactory = OidcProviderApiClient.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// The client for accessing this underlying API asynchronously.
        /// </summary>
        public OidcProviderApiClient.Client.IAsynchronousClient AsynchronousClient { get; set; }

        /// <summary>
        /// The client for accessing this underlying API synchronously.
        /// </summary>
        public OidcProviderApiClient.Client.ISynchronousClient Client { get; set; }

        /// <summary>
        /// Gets the base path of the API client.
        /// </summary>
        /// <value>The base path</value>
        public string GetBasePath()
        {
            return this.Configuration.BasePath;
        }

        /// <summary>
        /// Gets or sets the configuration object
        /// </summary>
        /// <value>An instance of the Configuration</value>
        public OidcProviderApiClient.Client.IReadableConfiguration Configuration { get; set; }

        /// <summary>
        /// Provides a factory method hook for the creation of exceptions.
        /// </summary>
        public OidcProviderApiClient.Client.ExceptionFactory ExceptionFactory
        {
            get
            {
                if (_exceptionFactory != null && _exceptionFactory.GetInvocationList().Length > 1)
                {
                    throw new InvalidOperationException("Multicast delegate for ExceptionFactory is unsupported.");
                }
                return _exceptionFactory;
            }
            set { _exceptionFactory = value; }
        }

        /// <summary>
        ///  Adds a user to the group
        /// </summary>
        /// <exception cref="OidcProviderApiClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="groupId">The id for the group</param>
        /// <param name="addUserToGroupRequest">Definition of the user to add to the group (optional)</param>
        /// <param name="operationIndex">Index associated with the operation.</param>
        /// <returns></returns>
        public void AddUserToGroup(string groupId, AddUserToGroupRequest? addUserToGroupRequest = default(AddUserToGroupRequest?), int operationIndex = 0)
        {
            AddUserToGroupWithHttpInfo(groupId, addUserToGroupRequest);
        }

        /// <summary>
        ///  Adds a user to the group
        /// </summary>
        /// <exception cref="OidcProviderApiClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="groupId">The id for the group</param>
        /// <param name="addUserToGroupRequest">Definition of the user to add to the group (optional)</param>
        /// <param name="operationIndex">Index associated with the operation.</param>
        /// <returns>ApiResponse of Object(void)</returns>
        public OidcProviderApiClient.Client.ApiResponse<Object> AddUserToGroupWithHttpInfo(string groupId, AddUserToGroupRequest? addUserToGroupRequest = default(AddUserToGroupRequest?), int operationIndex = 0)
        {
            // verify the required parameter 'groupId' is set
            if (groupId == null)
            {
                throw new OidcProviderApiClient.Client.ApiException(400, "Missing required parameter 'groupId' when calling GroupApi->AddUserToGroup");
            }

            OidcProviderApiClient.Client.RequestOptions localVarRequestOptions = new OidcProviderApiClient.Client.RequestOptions();

            string[] _contentTypes = new string[] {
                "application/json"
            };

            // to determine the Accept header
            string[] _accepts = new string[] {
                "application/problem+json"
            };

            var localVarContentType = OidcProviderApiClient.Client.ClientUtils.SelectHeaderContentType(_contentTypes);
            if (localVarContentType != null)
            {
                localVarRequestOptions.HeaderParameters.Add("Content-Type", localVarContentType);
            }

            var localVarAccept = OidcProviderApiClient.Client.ClientUtils.SelectHeaderAccept(_accepts);
            if (localVarAccept != null)
            {
                localVarRequestOptions.HeaderParameters.Add("Accept", localVarAccept);
            }

            localVarRequestOptions.PathParameters.Add("groupId", OidcProviderApiClient.Client.ClientUtils.ParameterToString(groupId)); // path parameter
            localVarRequestOptions.Data = addUserToGroupRequest;

            localVarRequestOptions.Operation = "GroupApi.AddUserToGroup";
            localVarRequestOptions.OperationIndex = operationIndex;


            // make the HTTP request
            var localVarResponse = this.Client.Post<Object>("/groups/{groupId}/users", localVarRequestOptions, this.Configuration);
            if (this.ExceptionFactory != null)
            {
                Exception _exception = this.ExceptionFactory("AddUserToGroup", localVarResponse);
                if (_exception != null)
                {
                    throw _exception;
                }
            }

            return localVarResponse;
        }

        /// <summary>
        ///  Adds a user to the group
        /// </summary>
        /// <exception cref="OidcProviderApiClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="groupId">The id for the group</param>
        /// <param name="addUserToGroupRequest">Definition of the user to add to the group (optional)</param>
        /// <param name="operationIndex">Index associated with the operation.</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of void</returns>
        public async System.Threading.Tasks.Task AddUserToGroupAsync(string groupId, AddUserToGroupRequest? addUserToGroupRequest = default(AddUserToGroupRequest?), int operationIndex = 0, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            await AddUserToGroupWithHttpInfoAsync(groupId, addUserToGroupRequest, operationIndex, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        ///  Adds a user to the group
        /// </summary>
        /// <exception cref="OidcProviderApiClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="groupId">The id for the group</param>
        /// <param name="addUserToGroupRequest">Definition of the user to add to the group (optional)</param>
        /// <param name="operationIndex">Index associated with the operation.</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of ApiResponse</returns>
        public async System.Threading.Tasks.Task<OidcProviderApiClient.Client.ApiResponse<Object>> AddUserToGroupWithHttpInfoAsync(string groupId, AddUserToGroupRequest? addUserToGroupRequest = default(AddUserToGroupRequest?), int operationIndex = 0, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            // verify the required parameter 'groupId' is set
            if (groupId == null)
            {
                throw new OidcProviderApiClient.Client.ApiException(400, "Missing required parameter 'groupId' when calling GroupApi->AddUserToGroup");
            }


            OidcProviderApiClient.Client.RequestOptions localVarRequestOptions = new OidcProviderApiClient.Client.RequestOptions();

            string[] _contentTypes = new string[] {
                "application/json"
            };

            // to determine the Accept header
            string[] _accepts = new string[] {
                "application/problem+json"
            };

            var localVarContentType = OidcProviderApiClient.Client.ClientUtils.SelectHeaderContentType(_contentTypes);
            if (localVarContentType != null)
            {
                localVarRequestOptions.HeaderParameters.Add("Content-Type", localVarContentType);
            }

            var localVarAccept = OidcProviderApiClient.Client.ClientUtils.SelectHeaderAccept(_accepts);
            if (localVarAccept != null)
            {
                localVarRequestOptions.HeaderParameters.Add("Accept", localVarAccept);
            }

            localVarRequestOptions.PathParameters.Add("groupId", OidcProviderApiClient.Client.ClientUtils.ParameterToString(groupId)); // path parameter
            localVarRequestOptions.Data = addUserToGroupRequest;

            localVarRequestOptions.Operation = "GroupApi.AddUserToGroup";
            localVarRequestOptions.OperationIndex = operationIndex;


            // make the HTTP request
            var localVarResponse = await this.AsynchronousClient.PostAsync<Object>("/groups/{groupId}/users", localVarRequestOptions, this.Configuration, cancellationToken).ConfigureAwait(false);

            if (this.ExceptionFactory != null)
            {
                Exception _exception = this.ExceptionFactory("AddUserToGroup", localVarResponse);
                if (_exception != null)
                {
                    throw _exception;
                }
            }

            return localVarResponse;
        }

        /// <summary>
        ///  Creates a new group
        /// </summary>
        /// <exception cref="OidcProviderApiClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="createGroupRequest">Definition of a new group (optional)</param>
        /// <param name="operationIndex">Index associated with the operation.</param>
        /// <returns>GroupCreatedResponse</returns>
        public GroupCreatedResponse CreateGroup(CreateGroupRequest? createGroupRequest = default(CreateGroupRequest?), int operationIndex = 0)
        {
            OidcProviderApiClient.Client.ApiResponse<GroupCreatedResponse> localVarResponse = CreateGroupWithHttpInfo(createGroupRequest);
            return localVarResponse.Data;
        }

        /// <summary>
        ///  Creates a new group
        /// </summary>
        /// <exception cref="OidcProviderApiClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="createGroupRequest">Definition of a new group (optional)</param>
        /// <param name="operationIndex">Index associated with the operation.</param>
        /// <returns>ApiResponse of GroupCreatedResponse</returns>
        public OidcProviderApiClient.Client.ApiResponse<GroupCreatedResponse> CreateGroupWithHttpInfo(CreateGroupRequest? createGroupRequest = default(CreateGroupRequest?), int operationIndex = 0)
        {
            OidcProviderApiClient.Client.RequestOptions localVarRequestOptions = new OidcProviderApiClient.Client.RequestOptions();

            string[] _contentTypes = new string[] {
                "application/json"
            };

            // to determine the Accept header
            string[] _accepts = new string[] {
                "application/json",
                "application/problem+json"
            };

            var localVarContentType = OidcProviderApiClient.Client.ClientUtils.SelectHeaderContentType(_contentTypes);
            if (localVarContentType != null)
            {
                localVarRequestOptions.HeaderParameters.Add("Content-Type", localVarContentType);
            }

            var localVarAccept = OidcProviderApiClient.Client.ClientUtils.SelectHeaderAccept(_accepts);
            if (localVarAccept != null)
            {
                localVarRequestOptions.HeaderParameters.Add("Accept", localVarAccept);
            }

            localVarRequestOptions.Data = createGroupRequest;

            localVarRequestOptions.Operation = "GroupApi.CreateGroup";
            localVarRequestOptions.OperationIndex = operationIndex;


            // make the HTTP request
            var localVarResponse = this.Client.Post<GroupCreatedResponse>("/groups", localVarRequestOptions, this.Configuration);
            if (this.ExceptionFactory != null)
            {
                Exception _exception = this.ExceptionFactory("CreateGroup", localVarResponse);
                if (_exception != null)
                {
                    throw _exception;
                }
            }

            return localVarResponse;
        }

        /// <summary>
        ///  Creates a new group
        /// </summary>
        /// <exception cref="OidcProviderApiClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="createGroupRequest">Definition of a new group (optional)</param>
        /// <param name="operationIndex">Index associated with the operation.</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of GroupCreatedResponse</returns>
        public async System.Threading.Tasks.Task<GroupCreatedResponse> CreateGroupAsync(CreateGroupRequest? createGroupRequest = default(CreateGroupRequest?), int operationIndex = 0, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            OidcProviderApiClient.Client.ApiResponse<GroupCreatedResponse> localVarResponse = await CreateGroupWithHttpInfoAsync(createGroupRequest, operationIndex, cancellationToken).ConfigureAwait(false);
            return localVarResponse.Data;
        }

        /// <summary>
        ///  Creates a new group
        /// </summary>
        /// <exception cref="OidcProviderApiClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="createGroupRequest">Definition of a new group (optional)</param>
        /// <param name="operationIndex">Index associated with the operation.</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of ApiResponse (GroupCreatedResponse)</returns>
        public async System.Threading.Tasks.Task<OidcProviderApiClient.Client.ApiResponse<GroupCreatedResponse>> CreateGroupWithHttpInfoAsync(CreateGroupRequest? createGroupRequest = default(CreateGroupRequest?), int operationIndex = 0, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {

            OidcProviderApiClient.Client.RequestOptions localVarRequestOptions = new OidcProviderApiClient.Client.RequestOptions();

            string[] _contentTypes = new string[] {
                "application/json"
            };

            // to determine the Accept header
            string[] _accepts = new string[] {
                "application/json",
                "application/problem+json"
            };

            var localVarContentType = OidcProviderApiClient.Client.ClientUtils.SelectHeaderContentType(_contentTypes);
            if (localVarContentType != null)
            {
                localVarRequestOptions.HeaderParameters.Add("Content-Type", localVarContentType);
            }

            var localVarAccept = OidcProviderApiClient.Client.ClientUtils.SelectHeaderAccept(_accepts);
            if (localVarAccept != null)
            {
                localVarRequestOptions.HeaderParameters.Add("Accept", localVarAccept);
            }

            localVarRequestOptions.Data = createGroupRequest;

            localVarRequestOptions.Operation = "GroupApi.CreateGroup";
            localVarRequestOptions.OperationIndex = operationIndex;


            // make the HTTP request
            var localVarResponse = await this.AsynchronousClient.PostAsync<GroupCreatedResponse>("/groups", localVarRequestOptions, this.Configuration, cancellationToken).ConfigureAwait(false);

            if (this.ExceptionFactory != null)
            {
                Exception _exception = this.ExceptionFactory("CreateGroup", localVarResponse);
                if (_exception != null)
                {
                    throw _exception;
                }
            }

            return localVarResponse;
        }

    }
}
