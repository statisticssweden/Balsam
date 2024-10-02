/*
 * BalsamApi
 *
 * This is the API for createing Baslam artifcats.
 *
 * The version of the OpenAPI document: 2.0
 * 
 * Generated by: https://openapi-generator.tech
 */

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using BalsamApi.Server.Converters;

namespace BalsamApi.Server.Models
{ 
    /// <summary>
    /// Payload for creating new project
    /// </summary>
    [DataContract]
    public class CreateProjectRequest : IEquatable<CreateProjectRequest>
    {
        /// <summary>
        /// The name
        /// </summary>
        /// <value>The name</value>
        [Required]
        [DataMember(Name="name", EmitDefaultValue=false)]
        public string Name { get; set; }

        /// <summary>
        /// The description of the project
        /// </summary>
        /// <value>The description of the project</value>
        [DataMember(Name="description", EmitDefaultValue=false)]
        public string? Description { get; set; }

        /// <summary>
        /// Name of the default branch
        /// </summary>
        /// <value>Name of the default branch</value>
        [Required]
        [DataMember(Name="branchName", EmitDefaultValue=false)]
        public string BranchName { get; set; }

        /// <summary>
        /// the location to a git-repository containing files that the repository will be initiated with
        /// </summary>
        /// <value>the location to a git-repository containing files that the repository will be initiated with</value>
        [DataMember(Name="sourceLocation", EmitDefaultValue=false)]
        public string? SourceLocation { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class CreateProjectRequest {\n");
            sb.Append("  Name: ").Append(Name).Append("\n");
            sb.Append("  Description: ").Append(Description).Append("\n");
            sb.Append("  BranchName: ").Append(BranchName).Append("\n");
            sb.Append("  SourceLocation: ").Append(SourceLocation).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="obj">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((CreateProjectRequest)obj);
        }

        /// <summary>
        /// Returns true if CreateProjectRequest instances are equal
        /// </summary>
        /// <param name="other">Instance of CreateProjectRequest to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(CreateProjectRequest other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return 
                (
                    Name == other.Name ||
                    Name != null &&
                    Name.Equals(other.Name)
                ) && 
                (
                    Description == other.Description ||
                    Description != null &&
                    Description.Equals(other.Description)
                ) && 
                (
                    BranchName == other.BranchName ||
                    BranchName != null &&
                    BranchName.Equals(other.BranchName)
                ) && 
                (
                    SourceLocation == other.SourceLocation ||
                    SourceLocation != null &&
                    SourceLocation.Equals(other.SourceLocation)
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
                var hashCode = 41;
                // Suitable nullity checks etc, of course :)
                    if (Name != null)
                    hashCode = hashCode * 59 + Name.GetHashCode();
                    if (Description != null)
                    hashCode = hashCode * 59 + Description.GetHashCode();
                    if (BranchName != null)
                    hashCode = hashCode * 59 + BranchName.GetHashCode();
                    if (SourceLocation != null)
                    hashCode = hashCode * 59 + SourceLocation.GetHashCode();
                return hashCode;
            }
        }

        #region Operators
        #pragma warning disable 1591

        public static bool operator ==(CreateProjectRequest left, CreateProjectRequest right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CreateProjectRequest left, CreateProjectRequest right)
        {
            return !Equals(left, right);
        }

        #pragma warning restore 1591
        #endregion Operators
    }
}
