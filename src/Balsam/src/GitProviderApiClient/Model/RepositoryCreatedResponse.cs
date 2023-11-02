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
    /// Repository description
    /// </summary>
    [DataContract(Name = "RepositoryCreatedResponse")]
    public partial class RepositoryCreatedResponse : IEquatable<RepositoryCreatedResponse>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryCreatedResponse" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected RepositoryCreatedResponse() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryCreatedResponse" /> class.
        /// </summary>
        /// <param name="id">The identity of the repository (required).</param>
        /// <param name="preferredName">The preferred name for the repository (required).</param>
        /// <param name="name">The name of the repository (required).</param>
        /// <param name="path">The path to the repository that can be used for clone it. (required).</param>
        /// <param name="defaultBranchName">The name of the default git branch (required).</param>
        public RepositoryCreatedResponse(string id = default(string), string preferredName = default(string), string name = default(string), string path = default(string), string defaultBranchName = default(string))
        {
            // to ensure "id" is required (not null)
            if (id == null)
            {
                throw new ArgumentNullException("id is a required property for RepositoryCreatedResponse and cannot be null");
            }
            this.Id = id;
            // to ensure "preferredName" is required (not null)
            if (preferredName == null)
            {
                throw new ArgumentNullException("preferredName is a required property for RepositoryCreatedResponse and cannot be null");
            }
            this.PreferredName = preferredName;
            // to ensure "name" is required (not null)
            if (name == null)
            {
                throw new ArgumentNullException("name is a required property for RepositoryCreatedResponse and cannot be null");
            }
            this.Name = name;
            // to ensure "path" is required (not null)
            if (path == null)
            {
                throw new ArgumentNullException("path is a required property for RepositoryCreatedResponse and cannot be null");
            }
            this.Path = path;
            // to ensure "defaultBranchName" is required (not null)
            if (defaultBranchName == null)
            {
                throw new ArgumentNullException("defaultBranchName is a required property for RepositoryCreatedResponse and cannot be null");
            }
            this.DefaultBranchName = defaultBranchName;
        }

        /// <summary>
        /// The identity of the repository
        /// </summary>
        /// <value>The identity of the repository</value>
        [DataMember(Name = "id", IsRequired = true, EmitDefaultValue = true)]
        public string Id { get; set; }

        /// <summary>
        /// The preferred name for the repository
        /// </summary>
        /// <value>The preferred name for the repository</value>
        [DataMember(Name = "preferredName", IsRequired = true, EmitDefaultValue = true)]
        public string PreferredName { get; set; }

        /// <summary>
        /// The name of the repository
        /// </summary>
        /// <value>The name of the repository</value>
        [DataMember(Name = "name", IsRequired = true, EmitDefaultValue = true)]
        public string Name { get; set; }

        /// <summary>
        /// The path to the repository that can be used for clone it.
        /// </summary>
        /// <value>The path to the repository that can be used for clone it.</value>
        [DataMember(Name = "path", IsRequired = true, EmitDefaultValue = true)]
        public string Path { get; set; }

        /// <summary>
        /// The name of the default git branch
        /// </summary>
        /// <value>The name of the default git branch</value>
        [DataMember(Name = "defaultBranchName", IsRequired = true, EmitDefaultValue = true)]
        public string DefaultBranchName { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("class RepositoryCreatedResponse {\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  PreferredName: ").Append(PreferredName).Append("\n");
            sb.Append("  Name: ").Append(Name).Append("\n");
            sb.Append("  Path: ").Append(Path).Append("\n");
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
            return this.Equals(input as RepositoryCreatedResponse);
        }

        /// <summary>
        /// Returns true if RepositoryCreatedResponse instances are equal
        /// </summary>
        /// <param name="input">Instance of RepositoryCreatedResponse to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(RepositoryCreatedResponse input)
        {
            if (input == null)
            {
                return false;
            }
            return 
                (
                    this.Id == input.Id ||
                    (this.Id != null &&
                    this.Id.Equals(input.Id))
                ) && 
                (
                    this.PreferredName == input.PreferredName ||
                    (this.PreferredName != null &&
                    this.PreferredName.Equals(input.PreferredName))
                ) && 
                (
                    this.Name == input.Name ||
                    (this.Name != null &&
                    this.Name.Equals(input.Name))
                ) && 
                (
                    this.Path == input.Path ||
                    (this.Path != null &&
                    this.Path.Equals(input.Path))
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
                if (this.Id != null)
                {
                    hashCode = (hashCode * 59) + this.Id.GetHashCode();
                }
                if (this.PreferredName != null)
                {
                    hashCode = (hashCode * 59) + this.PreferredName.GetHashCode();
                }
                if (this.Name != null)
                {
                    hashCode = (hashCode * 59) + this.Name.GetHashCode();
                }
                if (this.Path != null)
                {
                    hashCode = (hashCode * 59) + this.Path.GetHashCode();
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
