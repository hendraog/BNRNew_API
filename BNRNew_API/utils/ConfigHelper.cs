namespace BNRNew_API.utils
{
    public class ConfigHelper
    {
        public static T loadConfig<T>(ConfigurationBuilder configurationBuilder, string envName) where T : new()
        {
            var root = Directory.GetCurrentDirectory();
            var filePath = Path.Combine(root, ".env");
            System.Console.WriteLine(filePath.ToString());

            if (File.Exists(filePath))
            {
                var sa = File.ReadAllLines(filePath);
                foreach (var line in sa)
                {
                    var idx = line.IndexOf('=');
                    if (idx > 0)
                    {
                        var part1 = line.Substring(0, idx).Trim();
                        var part2 = "";
                        if (line.Length > (idx + 1))
                            part2 = line.Substring(idx + 1, line.Length - (idx + 1)).Trim();

                        Environment.SetEnvironmentVariable(part1, part2);
                    }

                }
            }

            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(root)
                .AddEnvironmentVariables();

            filePath = Path.Combine(root, "appsettings." + envName + ".json");
            System.Console.WriteLine(filePath.ToString());
            if (File.Exists(filePath))
                configBuilder.AddJsonFile(filePath);

            filePath = Path.Combine(root, "appsettings.json");
            System.Console.WriteLine(filePath.ToString());
            if (File.Exists(filePath))
                configBuilder.AddJsonFile(filePath);

            var config1 = configBuilder.Build();

            var a = new T();
            config1.GetSection(a.GetType().Name).Bind(a);
            ConfigurationBinder.Bind(config1, a);
            return a;
        }

    }
}
