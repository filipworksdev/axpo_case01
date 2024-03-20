using Axpo;
using Microsoft.Extensions.Configuration;
using PowerTradePositions.Logging;

namespace PowerTradePositions.Utils
{
    internal class ReportGenerator
    {
        private readonly IConfiguration _configuration;
        private readonly PowerService _powerService;
        private readonly Logger _logger;

        private string _folderPath;

        public ReportGenerator(IConfiguration configuration, PowerService powerService, Logger logger)
        {
            _configuration = configuration;
            _powerService = powerService;
            _logger = logger;

            _folderPath = _configuration["FolderPath"];
        }

        public bool GenerateReport(DateTime dayAhead)
        {
            try
            {
                IEnumerable<PowerTrade>? trades = _powerService.GetTrades(dayAhead);

                if (trades == null || !trades.Any())
                {
                    _logger.LogInformation($"No trades found for day ahead: {dayAhead.ToString("yyyy-MM-dd")}");
                }

                Dictionary<int, double> aggregatedVolumes = AggregateTradeVolumes(trades);
                string fileName = GenerateFileName(dayAhead);
                string filePath = Path.Combine(_folderPath, fileName);

                WriteAggregatedTradeDataToCSV(aggregatedVolumes, filePath, trades.Count());

                _logger.LogInformation($"Trade report successfully generated for {dayAhead.ToString("yyyy-MM-dd")}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error generating report for day ahead: {dayAhead.ToString("yyyy-MM-dd")}");
                return false;
            }
        }

        private Dictionary<int, double> AggregateTradeVolumes(IEnumerable<PowerTrade> trades)
        {
            Dictionary<int, double> aggregatedVolumes = new Dictionary<int, double>();
            foreach (var trade in trades)
            {
                foreach (var period in trade.Periods)
                {
                    if (aggregatedVolumes.ContainsKey(period.Period))
                    {
                        aggregatedVolumes[period.Period] += period.Volume;
                    }
                    else
                    {
                        aggregatedVolumes.Add(period.Period, period.Volume);
                    }
                }
            }
            return aggregatedVolumes;
        }

        private string GenerateFileName(DateTime dayAhead)
        {
            string dayAheadDate = dayAhead.ToString("yyyyMMdd");
            string extractionDateTime = DateTime.UtcNow.ToString("yyyyMMddHHmm");
            return $"PowerPosition_{dayAheadDate}_{extractionDateTime}.csv";
        }

        private void WriteAggregatedTradeDataToCSV(Dictionary<int, double> aggregatedVolumes, string filePath, int count, char delimiter = ';')
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("Period (#)" + delimiter + "Volume");
                foreach (var kvp in aggregatedVolumes)
                {
                    double averageVolume = kvp.Value / count; // Assuming trades is not null
                    writer.WriteLine($"{kvp.Key}{delimiter}{averageVolume}");
                }
            }
        }
    }
}
