namespace Blog;

public static class Configuration
{
    public static string JwtKey = "4LeK1B4jlki69mbElqEnRg3xpwK5zuq0yiLB2kZb+Ynw";
    public static string ApiKeyName { get; set; }
    public static string ApiKey { get; set; }
    public static string LinkImagesPath { get; set; }

    public static SmtpConfiguration Smtp = new();

    public class SmtpConfiguration
    {
        public bool SendPasswordEmail { get; set; }
        public string Host { get; set; }
        public int Port { get; set; } = 25;
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
