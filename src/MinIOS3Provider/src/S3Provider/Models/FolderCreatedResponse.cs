/*
 * S3Provider
 *
 * This a service contract for the OicdProvider in Balsam.
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
using S3Provider.Converters;

namespace S3Provider.Models
{ 
    /// <summary>
    /// Folder created description
    /// </summary>
    [DataContract]
    public class FolderCreatedResponse : IEquatable<FolderCreatedResponse>
    {
        /// <summary>
        /// The requested name of the virtual folder
        /// </summary>
        /// <value>The requested name of the virtual folder</value>
        [Required]
        [DataMember(Name="requestedName", EmitDefaultValue=false)]
        public string RequestedName { get; set; }

        /// <summary>
        /// The name of the virtual folder
        /// </summary>
        /// <value>The name of the virtual folder</value>
        [Required]
        [DataMember(Name="name", EmitDefaultValue=false)]
        public string Name { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class FolderCreatedResponse {\n");
            sb.Append("  RequestedName: ").Append(RequestedName).Append("\n");
            sb.Append("  Name: ").Append(Name).Append("\n");
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
            return obj.GetType() == GetType() && Equals((FolderCreatedResponse)obj);
        }

        /// <summary>
        /// Returns true if FolderCreatedResponse instances are equal
        /// </summary>
        /// <param name="other">Instance of FolderCreatedResponse to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(FolderCreatedResponse other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return 
                (
                    RequestedName == other.RequestedName ||
                    RequestedName != null &&
                    RequestedName.Equals(other.RequestedName)
                ) && 
                (
                    Name == other.Name ||
                    Name != null &&
                    Name.Equals(other.Name)
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
                    if (RequestedName != null)
                    hashCode = hashCode * 59 + RequestedName.GetHashCode();
                    if (Name != null)
                    hashCode = hashCode * 59 + Name.GetHashCode();
                return hashCode;
            }
        }

        #region Operators
        #pragma warning disable 1591

        public static bool operator ==(FolderCreatedResponse left, FolderCreatedResponse right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(FolderCreatedResponse left, FolderCreatedResponse right)
        {
            return !Equals(left, right);
        }

        #pragma warning restore 1591
        #endregion Operators
    }
}
