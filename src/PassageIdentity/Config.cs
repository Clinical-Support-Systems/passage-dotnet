namespace PassageIdentity;

public enum PassageAuthStrategy
{
    Cookie,
    Header
}

public interface IPassageConfig
{
    string? ApiKey { get; set; }

    string? AppId { get; set; }

    PassageAuthStrategy AuthStrategy { get; set; }

    string? PublicKey { get; set; }
}

public class PassageConfig : IPassageConfig
{
    public PassageConfig(string appId)
    {
        AppId = appId;
    }

    public PassageConfig()
    {
    }

    public string? ApiKey { get; set; }

    public string? AppId { get; set; }

    public PassageAuthStrategy AuthStrategy { get; set; } = PassageAuthStrategy.Cookie;

    /// <summary>
    /// <para>This is your public key Base64-encoded. It is in PEM format</para>
    /// <para>
    /// Your web application must use this key to verify the JWTs created by Passage on behalf of your users.
    /// For more information and sample code, see our docs on https://docs.passage.id/advanced/security
    /// </para>
    /// <para>This key is specific to this application and can only be used to verify users for this application.</para>
    /// </summary>
    public string? PublicKey { get; set; }
}
