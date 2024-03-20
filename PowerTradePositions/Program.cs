using Axpo;
using Microsoft.Extensions.Configuration;
using PowerTradePositions.Utils;
using PowerTradePositions.Logging;

namespace PowerTradePositions
{

    internal class Program
    {
        public static void Main(string[] args)
        {
            var configuration = BuildConfiguration();
            var powerService = new PowerService();
            var logger = new Logger();
            var reportGenerator = new ReportGenerator(configuration, powerService, logger);
            var scheduler = new Scheduler(configuration, reportGenerator, logger);

            scheduler.StartScheduling();
        }

        private static IConfiguration BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);

            return builder.Build();
        }
    }
}
