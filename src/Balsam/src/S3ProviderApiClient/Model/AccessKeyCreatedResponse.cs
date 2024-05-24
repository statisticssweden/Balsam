/*
 * S3Provider
 *
 * This a service contract for the OicdProvider in Balsam.
 *
 * The version of the OpenAPI document: 2.0
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */


using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using OpenAPIDateConverter = S3ProviderApiClient.Client.OpenAPIDateConverter;

namespace S3ProviderApiClient.Model
{
    /// <summary>
    /// Access key created description
    /// </summary>
    [DataContract(Name = "AccessKeyCreatedResponse")]
    public partial class AccessKeyCreatedResponse : IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccessKeyCreatedResponse" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected AccessKeyCreatedResponse() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="AccessKeyCreatedResponse" /> class.
        /// </summary>
        /// <param name="accessKey">The access key (required).</param>
        /// <param name="secretKey">The secret key (required).</param>
        public AccessKeyCreatedResponse(string accessKey = default(string), string secretKey = default(string))
        {
            // to ensure "accessKey" is required (not null)
            if (accessKey == null)
            {
                throw new ArgumentNullException("accessKey is a required property for AccessKeyCreatedResponse and cannot be null");
            }
            this.AccessKey = accessKey;
            // to ensure "secretKey" is required (not null)
            if (secretKey == null)
            {
                throw new ArgumentNullException("secretKey is a required property for AccessKeyCreatedResponse and cannot be null");
            }
            this.SecretKey = secretKey;
        }

        /// <summary>
        /// The access key
        /// </summary>
        /// <value>The access key</value>
        [DataMember(Name = "accessKey", IsRequired = true, EmitDefaultValue = true)]
        public string AccessKey { get; set; }

        /// <summary>
        /// The secret key
        /// </summary>
        /// <value>The secret key</value>
        [DataMember(Name = "secretKey", IsRequired = true, EmitDefaultValue = true)]
        public string SecretKey { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("class AccessKeyCreatedResponse {\n");
            sb.Append("  AccessKey: ").Append(AccessKey).Append("\n");
            sb.Append("  SecretKey: ").Append(SecretKey).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public virtual string ToJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
        }

        /// <summary>
        /// To validate all properties of the instance
        /// </summary>
        /// <param name="validationContext">Validation context</param>
        /// <returns>Validation Result</returns>
        IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }

}
