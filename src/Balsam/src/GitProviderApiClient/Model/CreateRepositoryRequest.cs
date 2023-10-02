/*
 * GitProvider
 *
 * This a service contract for the GitProvider in Balsam.
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
using OpenAPIDateConverter = GitProviderApiClient.Client.OpenAPIDateConverter;

namespace GitProviderApiClient.Model
{
    /// <summary>
    /// Payload for creating new repository
    /// </summary>
    [DataContract(Name = "CreateRepositoryRequest")]
    public partial class CreateRepositoryRequest : IEquatable<CreateRepositoryRequest>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateRepositoryRequest" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected CreateRepositoryRequest() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateRepositoryRequest" /> class.
        /// </summary>
        /// <param name="name">The name of the branch (required).</param>
        /// <param name="description">The description of the branch.</param>
        /// <param name="defaultBranchName">The branch from which this branch will be created. The default branch for the project will be used if not specified. (required).</param>
        public CreateRepositoryRequest(string name = default(string), string description = default(string), string defaultBranchName = default(string))
        {
            // to ensure "name" is required (not null)
            if (name == null)
            {
                throw new ArgumentNullException("name is a required property for CreateRepositoryRequest and cannot be null");
            }
            this.Name = name;
            // to ensure "defaultBranchName" is required (not null)
            if (defaultBranchName == null)
            {
                throw new ArgumentNullException("defaultBranchName is a required property for CreateRepositoryRequest and cannot be null");
            }
            this.DefaultBranchName = defaultBranchName;
            this.Description = description;
        }

        /// <summary>
        /// The name of the branch
        /// </summary>
        /// <value>The name of the branch</value>
        [DataMember(Name = "name", IsRequired = true, EmitDefaultValue = true)]
        public string Name { get; set; }

        /// <summary>
        /// The description of the branch
        /// </summary>
        /// <value>The description of the branch</value>
        [DataMember(Name = "description", EmitDefaultValue = false)]
        public string Description { get; set; }

        /// <summary>
        /// The branch from which this branch will be created. The default branch for the project will be used if not specified.
        /// </summary>
        /// <value>The branch from which this branch will be created. The default branch for the project will be used if not specified.</value>
        [DataMember(Name = "defaultBranchName", IsRequired = true, EmitDefaultValue = true)]
        public string DefaultBranchName { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("class CreateRepositoryRequest {\n");
            sb.Append("  Name: ").Append(Name).Append("\n");
            sb.Append("  Description: ").Append(Description).Append("\n");
            sb.Append("  DefaultBranchName: ").Append(DefaultBranchName).Append("\n");
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
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="input">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object input)
        {
            return this.Equals(input as CreateRepositoryRequest);
        }

        /// <summary>
        /// Returns true if CreateRepositoryRequest instances are equal
        /// </summary>
        /// <param name="input">Instance of CreateRepositoryRequest to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(CreateRepositoryRequest input)
        {
            if (input == null)
            {
                return false;
            }
            return 
                (
                    this.Name == input.Name ||
                    (this.Name != null &&
                    this.Name.Equals(input.Name))
                ) && 
                (
                    this.Description == input.Description ||
                    (this.Description != null &&
                    this.Description.Equals(input.Description))
                ) && 
                (
                    this.DefaultBranchName == input.DefaultBranchName ||
                    (this.DefaultBranchName != null &&
                    this.DefaultBranchName.Equals(input.DefaultBranchName))
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hashCode = 41;
                if (this.Name != null)
                {
                    hashCode = (hashCode * 59) + this.Name.GetHashCode();
                }
                if (this.Description != null)
                {
                    hashCode = (hashCode * 59) + this.Description.GetHashCode();
                }
                if (this.DefaultBranchName != null)
                {
                    hashCode = (hashCode * 59) + this.DefaultBranchName.GetHashCode();
                }
                return hashCode;
            }
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