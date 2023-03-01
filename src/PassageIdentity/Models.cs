using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace PassageIdentity
{
    public partial class App
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("allowed_identifier")]
        public virtual string? AllowedIdentifier { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("auth_origin")]
        public virtual Uri? AuthOrigin { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("default_language")]
        public virtual string? DefaultLanguage { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("ephemeral")]
        public virtual bool? Ephemeral { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("id")]
        public virtual string? Id { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("layouts")]
        public virtual Layouts? Layouts { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("login_url")]
        public virtual Uri? LoginUrl { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("name")]
        public virtual string? Name { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("public_signup")]
        public virtual bool? PublicSignup { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("redirect_url")]
        public virtual Uri? RedirectUrl { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("required_identifier")]
        public virtual string? RequiredIdentifier { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("require_email_verification")]
        public virtual bool? RequireEmailVerification { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("require_identifier_verification")]
        public virtual bool? RequireIdentifierVerification { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("session_timeout_length")]
        public virtual long? SessionTimeoutLength { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("user_metadata_schema")]
        public virtual Dictionary<string, string>? UserMetadataSchema { get; } = new();
    }

    public class Device
    {
        /// <summary>
        /// The ID of the webAuthn device used for authentication
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("id")]
        public virtual string? Id { get; set; }

        /// <summary>
        /// The CredID for this webAuthn device (encoded to match what is stored in psg_cred_obj)
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("cred_id")]
        public virtual string? CredId { get; set; }

        /// <summary>
        /// The UserID for this webAuthn device
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("user_id")]
        public virtual string? UserId { get; set; }

        /// <summary>
        /// The friendly name for the webAuthn device used to authenticate
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("friendly_name")]
        public virtual string? FriendlyName { get; set; }

        /// <summary>
        /// The Credential Type
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("type")]
        public virtual string? CredentialType { get; set; }

        /// <summary>
        /// How many times this webAuthn device has been used to authenticate the user
        /// </summary>
        [JsonPropertyName("usage_count")]
        public virtual long UsageCount { get; set; }

        /// <summary>
        /// The last time this webAuthn device was updated
        /// </summary>
        [JsonPropertyName("updated_at")]
        public virtual DateTime UpdatedAt { get; set; }

        /// <summary>
        /// The first time this webAuthn device was used to authenticate the user
        /// </summary>
        [JsonPropertyName("created_at")]
        public virtual DateTime CreatedAt { get; set; }

        /// <summary>
        /// The last time this webAuthn device was used to authenticate the user
        /// </summary>
        [JsonPropertyName("last_login_at")]
        public virtual DateTime LastLoginAt { get; set; }
    }

    public partial class Layouts
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("profile")]
        public virtual Dictionary<string, object>? Profile { get; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("registration")]
        public virtual Dictionary<string, object>? Registration { get; }
    }

    public partial class PassageApp
    {
        /// <summary>
        ///
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("app")]
        public virtual App? App { get; set; }
    }

    public partial class PassageUser
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("user")]
        public virtual User? User { get; set; }
    }

    [JsonConverter(typeof(CustomJsonStringEnumConverter))]
    public enum UserStatus
    {
        [EnumMember(Value = "active")]
        Active,
        [EnumMember(Value = "inactive")]
        Inactive,
        [EnumMember(Value = "pending")]
        Pending,
        UserIDDoesNotExist = -1,
    }

    public partial class User
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("email")]
        public virtual string? Email { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("email_verified")]
        public virtual bool? EmailVerified { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("id")]
        public virtual string? Id { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("phone")]
        public virtual string? Phone { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("phone_verified")]
        public virtual bool? PhoneVerified { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("status")]
        public virtual UserStatus? Status { get; set; }

        [JsonPropertyName("user_metadata")]
        public virtual Dictionary<string, object> UserMetadata { get; } = new();

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("webauthn")]
        public virtual bool? Webauthn { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("webauthn_types")]
        public virtual Collection<string> WebauthnTypes { get; } = new();
    }
}
