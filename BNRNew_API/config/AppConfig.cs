namespace BNRNew_API.config
{
    public class AppConfig
    {
        public int accessTokenExpired { get; set; } = 60*24;
        public int refreshTokenExpired { get; set; } = 600;

        public string jwtSecret { get; set; }
        public string jwtSecretRefresh { get; set; }
        public string location { get; set; }

    }
}
