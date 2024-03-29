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
    /// Payload for deleting an existing workspace
    /// </summary>
    [DataContract]
    public class DeleteWorkspaceRequest : IEquatable<DeleteWorkspaceRequest>
    {
        /// <summary>
        /// The id of the project
        /// </summary>
        /// <value>The id of the project</value>
        [Required]
        [DataMember(Name="projectId", EmitDefaultValue=false)]
        public string ProjectId { get; set; }

        /// <summary>
        /// The id of the branch
        /// </summary>
        /// <value>The id of the branch</value>
        [Required]
        [DataMember(Name="branchId", EmitDefaultValue=false)]
        public string BranchId { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class DeleteWorkspaceRequest {\n");
            sb.Append("  ProjectId: ").Append(ProjectId).Append("\n");
            sb.Append("  BranchId: ").Append(BranchId).Append("\n");
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
            return obj.GetType() == GetType() && Equals((DeleteWorkspaceRequest)obj);
        }

        /// <summary>
        /// Returns true if DeleteWorkspaceRequest instances are equal
        /// </summary>
        /// <param name="other">Instance of DeleteWorkspaceRequest to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(DeleteWorkspaceRequest other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return 
                (
                    ProjectId == other.ProjectId ||
                    ProjectId != null &&
                    ProjectId.Equals(other.ProjectId)
                ) && 
                (
                    BranchId == other.BranchId ||
                    BranchId != null &&
                    BranchId.Equals(other.BranchId)
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
                    if (ProjectId != null)
                    hashCode = hashCode * 59 + ProjectId.GetHashCode();
                    if (BranchId != null)
                    hashCode = hashCode * 59 + BranchId.GetHashCode();
                return hashCode;
            }
        }

        #region Operators
        #pragma warning disable 1591

        public static bool operator ==(DeleteWorkspaceRequest left, DeleteWorkspaceRequest right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DeleteWorkspaceRequest left, DeleteWorkspaceRequest right)
        {
            return !Equals(left, right);
        }

        #pragma warning restore 1591
        #endregion Operators
    }
}
