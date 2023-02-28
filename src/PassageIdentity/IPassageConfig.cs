namespace PassageIdentity
{
    public interface IPassageConfig
    {
        string? AppId { get; set; }
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

        public string? AppId { get; set; }
    }
}