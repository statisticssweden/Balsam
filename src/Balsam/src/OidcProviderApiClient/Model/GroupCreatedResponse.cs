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
    /// Group created response
    /// </summary>
    [DataContract(Name = "GroupCreatedResponse")]
    public partial class GroupCreatedResponse : IEquatable<GroupCreatedResponse>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupCreatedResponse" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected GroupCreatedResponse() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupCreatedResponse" /> class.
        /// </summary>
        /// <param name="name">The name of the group (required).</param>
        /// <param name="id">The id to the group that can be used for accessing it..</param>
        public GroupCreatedResponse(string name = default(string), string id = default(string))
        {
            // to ensure "name" is required (not null)
            if (name == null)
            {
                throw new ArgumentNullException("name is a required property for GroupCreatedResponse and cannot be null");
            }
            this.Name = name;
            this.Id = id;
        }

        /// <summary>
        /// The name of the group
        /// </summary>
        /// <value>The name of the group</value>
        [DataMember(Name = "name", IsRequired = true, EmitDefaultValue = true)]
        public string Name { get; set; }

        /// <summary>
        /// The id to the group that can be used for accessing it.
        /// </summary>
        /// <value>The id to the group that can be used for accessing it.</value>
        [DataMember(Name = "id", EmitDefaultValue = false)]
        public string Id { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("class GroupCreatedResponse {\n");
            sb.Append("  Name: ").Append(Name).Append("\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
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
            return this.Equals(input as GroupCreatedResponse);
        }

        /// <summary>
        /// Returns true if GroupCreatedResponse instances are equal
        /// </summary>
        /// <param name="input">Instance of GroupCreatedResponse to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(GroupCreatedResponse input)
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
                    this.Id == input.Id ||
                    (this.Id != null &&
                    this.Id.Equals(input.Id))
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
                if (this.Id != null)
                {
                    hashCode = (hashCode * 59) + this.Id.GetHashCode();
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