/*
 * GitProvider
 *
 * This a service contract for the GitProvider in Balsam.
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
using GitProvider.Converters;

namespace GitProvider.Models
{ 
    /// <summary>
    /// A PAT description
    /// </summary>
    [DataContract]
    public class UserPATCreatedResponse : IEquatable<UserPATCreatedResponse>
    {
        /// <summary>
        /// The name of the token.
        /// </summary>
        /// <value>The name of the token.</value>
        [DataMember(Name="name", EmitDefaultValue=true)]
        public string? Name { get; set; }

        /// <summary>
        /// The token that was created.
        /// </summary>
        /// <value>The token that was created.</value>
        [Required]
        [DataMember(Name="token", EmitDefaultValue=false)]
        public string Token { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class UserPATCreatedResponse {\n");
            sb.Append("  Name: ").Append(Name).Append("\n");
            sb.Append("  Token: ").Append(Token).Append("\n");
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
            return obj.GetType() == GetType() && Equals((UserPATCreatedResponse)obj);
        }

        /// <summary>
        /// Returns true if UserPATCreatedResponse instances are equal
        /// </summary>
        /// <param name="other">Instance of UserPATCreatedResponse to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(UserPATCreatedResponse other)
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
                    Token == other.Token ||
                    Token != null &&
                    Token.Equals(other.Token)
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
                    if (Token != null)
                    hashCode = hashCode * 59 + Token.GetHashCode();
                return hashCode;
            }
        }

        #region Operators
        #pragma warning disable 1591

        public static bool operator ==(UserPATCreatedResponse left, UserPATCreatedResponse right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(UserPATCreatedResponse left, UserPATCreatedResponse right)
        {
            return !Equals(left, right);
        }

        #pragma warning restore 1591
        #endregion Operators
    }
}