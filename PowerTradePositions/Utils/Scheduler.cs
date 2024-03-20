using Microsoft.Extensions.Configuration;

namespace PowerTradePositions.Utils
{
    internal class Scheduler
    {
        private readonly IConfiguration _configuration;
        private readonly ReportGenerator _reportGenerator;

        public Scheduler(IConfiguration configuration, ReportGenerator reportGenerator)
        {
            _configuration = configuration;
            _reportGenerator = reportGenerator;
        }

        public void StartScheduling()
        {
            double fetchIntervalSeconds = double.Parse(_configuration["FetchIntervalSeconds"]);

            while (true)
            {
                DateTime dayAhead = DateTime.UtcNow.Date.AddDays(1);
                _reportGenerator.GenerateReport(dayAhead);

                int sleepTime = (int)(fetchIntervalSeconds * 1000);
                Thread.Sleep(sleepTime);
            }
        }
    }
}
