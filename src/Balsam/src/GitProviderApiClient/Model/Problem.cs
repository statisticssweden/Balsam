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
    /// Problem
    /// </summary>
    [DataContract(Name = "Problem")]
    public partial class Problem : IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Problem" /> class.
        /// </summary>
        /// <param name="type">An absolute URI that identifies the problem type.  When dereferenced, it SHOULD provide human-readable documentation for the problem type (e.g., using HTML).  (default to &quot;about:blank&quot;).</param>
        /// <param name="title">A short, summary of the problem type. Written in english and readable for engineers (usually not suited for non technical stakeholders and not localized); example: Service Unavailable .</param>
        /// <param name="status">The HTTP status code generated by the origin server for this occurrence of the problem. .</param>
        /// <param name="detail">A human readable explanation specific to this occurrence of the problem. .</param>
        /// <param name="instance">An absolute URI that identifies the specific occurrence of the problem. It may or may not yield further information if dereferenced.        .</param>
        public Problem(string type = @"about:blank", string title = default(string), int status = default(int), string detail = default(string), string instance = default(string))
        {
            // use default value if no "type" provided
            this.Type = type ?? @"about:blank";
            this.Title = title;
            this.Status = status;
            this.Detail = detail;
            this.Instance = instance;
        }

        /// <summary>
        /// An absolute URI that identifies the problem type.  When dereferenced, it SHOULD provide human-readable documentation for the problem type (e.g., using HTML). 
        /// </summary>
        /// <value>An absolute URI that identifies the problem type.  When dereferenced, it SHOULD provide human-readable documentation for the problem type (e.g., using HTML). </value>
        /// <example>https://zalando.github.io/problem/constraint-violation</example>
        [DataMember(Name = "type", EmitDefaultValue = false)]
        public string Type { get; set; }

        /// <summary>
        /// A short, summary of the problem type. Written in english and readable for engineers (usually not suited for non technical stakeholders and not localized); example: Service Unavailable 
        /// </summary>
        /// <value>A short, summary of the problem type. Written in english and readable for engineers (usually not suited for non technical stakeholders and not localized); example: Service Unavailable </value>
        [DataMember(Name = "title", EmitDefaultValue = false)]
        public string Title { get; set; }

        /// <summary>
        /// The HTTP status code generated by the origin server for this occurrence of the problem. 
        /// </summary>
        /// <value>The HTTP status code generated by the origin server for this occurrence of the problem. </value>
        /// <example>503</example>
        [DataMember(Name = "status", EmitDefaultValue = false)]
        public int Status { get; set; }

        /// <summary>
        /// A human readable explanation specific to this occurrence of the problem. 
        /// </summary>
        /// <value>A human readable explanation specific to this occurrence of the problem. </value>
        /// <example>Connection to database timed out</example>
        [DataMember(Name = "detail", EmitDefaultValue = false)]
        public string Detail { get; set; }

        /// <summary>
        /// An absolute URI that identifies the specific occurrence of the problem. It may or may not yield further information if dereferenced.        
        /// </summary>
        /// <value>An absolute URI that identifies the specific occurrence of the problem. It may or may not yield further information if dereferenced.        </value>
        [DataMember(Name = "instance", EmitDefaultValue = false)]
        public string Instance { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("class Problem {\n");
            sb.Append("  Type: ").Append(Type).Append("\n");
            sb.Append("  Title: ").Append(Title).Append("\n");
            sb.Append("  Status: ").Append(Status).Append("\n");
            sb.Append("  Detail: ").Append(Detail).Append("\n");
            sb.Append("  Instance: ").Append(Instance).Append("\n");
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
            // Status (int) maximum
            if (this.Status > (int)600)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for Status, must be a value less than or equal to 600.", new [] { "Status" });
            }

            // Status (int) minimum
            if (this.Status < (int)100)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for Status, must be a value greater than or equal to 100.", new [] { "Status" });
            }

            yield break;
        }
    }

}
