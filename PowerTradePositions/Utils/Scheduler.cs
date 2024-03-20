using Microsoft.Extensions.Configuration;
using PowerTradePositions.Logging;

namespace PowerTradePositions.Utils
{
    internal class Scheduler
    {
        private readonly IConfiguration _configuration;
        private readonly ReportGenerator _reportGenerator;
        private readonly Logger _logger;

        private int _fetchIntervalSeconds;
        private int _retryIntervalSeconds;

        public Scheduler(IConfiguration configuration, ReportGenerator reportGenerator, Logger logger)
        {
            _configuration = configuration;
            _reportGenerator = reportGenerator;
            _logger = logger;

            _fetchIntervalSeconds = int.Parse(_configuration["FetchIntervalSeconds"]);
            _retryIntervalSeconds = int.Parse(_configuration["RetryIntervalSeconds"]);
        }

        public void StartScheduling()
        {
            var timer = new System.Timers.Timer(_fetchIntervalSeconds * 1000);
            timer.Elapsed += OnTimerElapsed;
            timer.AutoReset = true;
            timer.Enabled = true;

            // keep application alive but reduce CPU usage
            while (true)
            {
                Thread.Sleep(1000);
            }
        }

        private void OnTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            DateTime dayAhead = DateTime.UtcNow.Date.AddDays(1);
            bool success = _reportGenerator.GenerateReport(dayAhead);
            int retries = 3;
            while (!success && retries-- >= 0)
            {
                _logger.LogInformation("Retrying in " + _retryIntervalSeconds + " seconds");
                Thread.Sleep(_retryIntervalSeconds * 1000);
                success = _reportGenerator.GenerateReport(dayAhead);
            }
        }
    }
}
