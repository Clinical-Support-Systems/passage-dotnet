namespace PassageIdentity
{
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
        public string? PublicKey { get; set; }
    }
}
