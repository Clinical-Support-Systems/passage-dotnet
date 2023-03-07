using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace PassageIdentity;

public partial class PassageMagicLink
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("magic_link")]
    public virtual MagicLink? MagicLink { get; set; }
}

public partial class MagicLink
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("id")]
    public virtual string? Id { get; set; }
}

public partial class CreateMagicLink
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("channel")]
    public virtual string? Channel { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("email")]
    public virtual string? Email { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("language")]
    public virtual string? Language { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("magic_link_path")]
    public virtual string? MagicLinkPath { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("phone")]
    public virtual string? Phone { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("redirect_url")]
    public virtual string? Redirect { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("send")]
    public virtual bool? Send { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("ttl")]
    public virtual int? TTL { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("user_id")]
    public virtual string? UserId { get; set; }
}

[JsonConverter(typeof(CustomJsonStringEnumConverter))]
public enum AllowedIdentifier
{
    [EnumMember(Value = "email")]
    Email,

    [EnumMember(Value = "phone")]
    Phone,

    [EnumMember(Value = "both")]
    Both
}

[JsonConverter(typeof(CustomJsonStringEnumConverter))]
public enum UserMetadataFieldType
{
    [EnumMember(Value = "date")]
    Date,

    [EnumMember(Value = "integer")]
    Number,

    [EnumMember(Value = "string")]
    Text,

    [EnumMember(Value = "boolean")]
    Switch,

    [EnumMember(Value = "phone")]
    Phone,

    [EnumMember(Value = "email")]
    Email
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

[JsonConverter(typeof(CustomJsonStringEnumConverter))]
public enum AdminRole
{
    [EnumMember(Value = "owner")]
    Owner,

    [EnumMember(Value = "admin")]
    Admin,

    AdminIDDoesNotExist = -1,
}

[JsonConverter(typeof(CustomJsonStringEnumConverter))]
public enum PaginatedEventType
{
    [EnumMember(Value = "webauthn.register.initiated")]
    WebAuthnRegisterInitiated,

    [EnumMember(Value = "webauthn.register.completed")]
    WebAuthnRegisterCompleted,

    [EnumMember(Value = "webauthn.login.initiated")]
    WebAuthnLoginInitiated,

    [EnumMember(Value = "webauthn.login.completed")]
    WebAuthnLoginCompleted,

    [EnumMember(Value = "magic_link.register.initiated")]
    MagicLinkRegisterInitiated,

    [EnumMember(Value = "magic_link.login.initiated")]
    MagicLinkLoginInitiated,

    [EnumMember(Value = "magic_link.completed")]
    MagicLinkCompleted,
}

public partial class App
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("allowed_identifier")]
    public virtual AllowedIdentifier? AllowedIdentifier { get; set; }

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
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>")]
    public virtual Collection<UserMetadata>? UserMetadataSchema { get; set; } = new();
}

public partial class AuthResult
{
    [JsonPropertyName("auth_token")]
    public virtual string AccessToken { get; set; } = string.Empty;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("redirect_url")]
    public virtual Uri? RedirectUrl { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("refresh_token")]
    public virtual string? RefreshToken { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("refresh_token_expiration")]
    public virtual long? RefreshTokenExpiration { get; set; }
}

public class Device
{
    /// <summary>
    /// The first time this webAuthn device was used to authenticate the user
    /// </summary>
    [JsonPropertyName("created_at")]
    public virtual DateTime CreatedAt { get; set; }

    /// <summary>
    /// The Credential Type
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("type")]
    public virtual string? CredentialType { get; set; }

    /// <summary>
    /// The CredID for this webAuthn device (encoded to match what is stored in psg_cred_obj)
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("cred_id")]
    public virtual string? CredId { get; set; }

    /// <summary>
    /// The friendly name for the webAuthn device used to authenticate
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("friendly_name")]
    public virtual string? FriendlyName { get; set; }

    /// <summary>
    /// The ID of the webAuthn device used for authentication
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("id")]
    public virtual string? Id { get; set; }

    /// <summary>
    /// The last time this webAuthn device was used to authenticate the user
    /// </summary>
    [JsonPropertyName("last_login_at")]
    public virtual DateTime LastLoginAt { get; set; }

    /// <summary>
    /// The last time this webAuthn device was updated
    /// </summary>
    [JsonPropertyName("updated_at")]
    public virtual DateTime UpdatedAt { get; set; }

    /// <summary>
    /// How many times this webAuthn device has been used to authenticate the user
    /// </summary>
    [JsonPropertyName("usage_count")]
    public virtual long UsageCount { get; set; }

    /// <summary>
    /// The UserID for this webAuthn device
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("user_id")]
    public virtual string? UserId { get; set; }
}

public partial class Layouts
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("profile")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>")]
    public virtual Collection<PassageProfile>? Profile { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("registration")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>")]
    public virtual Collection<PassageRegistration>? Registration { get; set; }
}

public partial class PassageApp
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("app")]
    public virtual App App { get; set; } = new();
}

public partial class PassageAuthResult
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("auth_result")]
    public virtual AuthResult? Result { get; set; }
}

public partial class PassageProfile
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("h")]
    public virtual long? Height { get; set; }

    [JsonPropertyName("id")]
    public virtual string Id { get; set; } = string.Empty;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("w")]
    public virtual long? Width { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("x")]
    public virtual long? X { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("y")]
    public virtual long? Y { get; set; }
}

public partial class PassageRegistration
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("h")]
    public virtual long? Height { get; set; }

    [JsonPropertyName("id")]
    public virtual string Id { get; set; } = string.Empty;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("w")]
    public virtual long? Width { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("x")]
    public virtual long? X { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("y")]
    public virtual long? Y { get; set; }
}

public partial class PassageUser
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("user")]
    public virtual User? User { get; set; }
}

public partial class PassageAdmins
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("admins")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>")]
    public virtual Collection<AdminMember> Admins { get; set; } = new();
}

public partial class PassageAdmin
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("admin")]
    public virtual AdminMember? Admin { get; set; } = new();
}

public partial class AdminMember
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("email")]
    public virtual string? Email { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("created_at")]
    public virtual DateTime? CreatedAt { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("id")]
    public virtual string? Id { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("user_id")]
    public virtual string? userId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("app_id")]
    public virtual string? AppId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("role")]
    public virtual AdminRole? Role { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("technologies")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>")]
    public virtual Collection<string>? Technologies { get; set; } = new();
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
    public virtual Collection<UserMetadata> UserMetadata { get; } = new();

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("webauthn")]
    public virtual bool? Webauthn { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("created_at")]
    public virtual DateTime? CreatedAt { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("updated_at")]
    public virtual DateTime? UpdatedAt { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("last_login_at")]
    public virtual DateTime? LastLoginAt { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("login_count")]
    public virtual long? LoginCount { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("webauthn_types")]
    public virtual Collection<string> WebauthnTypes { get; } = new();
}

public partial class UserMetadata
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("field_name")]
    public virtual string? FieldName { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("friendly_name")]
    public virtual string? FriendlyName { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("id")]
    public virtual string? Id { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("profile")]
    public virtual bool? Profile { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("registration")]
    public virtual bool? Registration { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("type")]
    public virtual UserMetadataFieldType? Type { get; set; }
}

public partial class PassageUserList
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("users")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>")]
    public virtual Collection<User> Users { get; set; } = new();
}

public partial class PassageWebAuthLogin
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("handshake")]
    public virtual HandShake? HandShake { get; set; }
}

public partial class HandShake
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("challenge")]
    public virtual HandShakeChallenge? Challenge { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("user")]
    public virtual User? User { get; set; }
}

public partial class HandShakeChallenge
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("publicKey")]
    public virtual PublicKey? PublicKey { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("id")]
    public virtual string? Id { get; set; }
}

public partial class PublicKey
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("challenge")]
    public virtual string? Challenge { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("rpId")]
    public virtual string? RpId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("timeout")]
    public virtual int? TimeOut { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("userVerification")]
    public virtual string? UserVerification { get; set; }
}

public partial class ListPassageUsersQuery : PassageQuery
{

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("login_count")]
    public virtual string? LoginCount { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("status")]
    public virtual string? Status { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("email_verified")]
    public virtual string? EmailVerified { get; set; }


    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("updated_at")]
    public virtual DateTime? UpdatedAt { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("last_login_at")]
    public virtual DateTime? LastLoginAt { get; set; }
}

public partial class PassagePaginatedEventsQuery : PassageQuery
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("user_id")]
    public virtual string? UserId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("ip_addr")]
    public virtual string? IpAddress { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("user_agent")]
    public virtual string? UserAgent { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("type")]
    public virtual PaginatedEventType? PaginatedEventType { get; set; }
}


public partial class PassageQuery
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("page")]
    public virtual int Page { get; set; } = 1;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("limit")]
    public virtual int? Limit { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("order_by")]
    public virtual string? OrderBy { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("created_before")]
    public virtual DateTime? CreatedBefore { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("identifier")]
    public virtual string? Identifier { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("id")]
    public virtual string? Id { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("created_at")]
    public virtual DateTime? CreatedAt { get; set; }
}


public partial class PassageApiKeys
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("api_keys")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>")]
    public virtual Collection<ApiKey> ApiKeys { get; set; } = new();
}

public partial class ApiKey
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("id")]
    public virtual string? Id { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("name")]
    public virtual string? Name { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("role")]
    public virtual string? Role { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("key_prefix")]
    public virtual string? KeyPrefix { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("created_at")]
    public virtual string? CreatedAt { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("plaintext_key")]
    public virtual string? PlainTextKey { get; set; }
}

public partial class PassageApiKey
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("api_key")]
    public virtual ApiKey? ApiKey { get; set; }
}

public partial class PassagePaginatedEvents
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("events")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>")]
    public virtual Collection<PaginatedEvent> Events { get; set; } = new();
}


public partial class PaginatedEvent
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("id")]
    public virtual string? Id { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("user_id")]
    public virtual string? UserId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("type")]
    public virtual PaginatedEventType? Type { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("identifier")]
    public virtual string? Identifier { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("ip_addr")]
    public virtual string? IpAddress { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("user_agent")]
    public virtual string? UserAgent { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("created_at")]
    public virtual DateTime? CreatedAt { get; set; }
}
