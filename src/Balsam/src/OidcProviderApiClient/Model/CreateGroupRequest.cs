/*
 * OidcProvider
 *
 * This a service contract for the OidcProvider in Balsam.
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
using OpenAPIDateConverter = OidcProviderApiClient.Client.OpenAPIDateConverter;

namespace OidcProviderApiClient.Model
{
    /// <summary>
    /// Group name
    /// </summary>
    [DataContract(Name = "CreateGroupRequest")]
    public partial class CreateGroupRequest : IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateGroupRequest" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected CreateGroupRequest() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateGroupRequest" /> class.
        /// </summary>
        /// <param name="name">The name of the group (required).</param>
        public CreateGroupRequest(string name = default(string))
        {
            // to ensure "name" is required (not null)
            if (name == null)
            {
                throw new ArgumentNullException("name is a required property for CreateGroupRequest and cannot be null");
            }
            this.Name = name;
        }

        /// <summary>
        /// The name of the group
        /// </summary>
        /// <value>The name of the group</value>
        [DataMember(Name = "name", IsRequired = true, EmitDefaultValue = true)]
        public string Name { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("class CreateGroupRequest {\n");
            sb.Append("  Name: ").Append(Name).Append("\n");
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
