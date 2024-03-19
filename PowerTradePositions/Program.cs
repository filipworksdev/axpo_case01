using System;
using System.Diagnostics;
using System.IO;
using Axpo;

float retryIntervalMinutes = 0.25f;
float fetchIntervalMinutes = 0.5f;
string folderPath = @"C:/CSV_DATA/";

if (!Directory.Exists(folderPath))
{
    Directory.CreateDirectory(folderPath);
}

PowerService powerService = new PowerService();

DateTime dayAhead = DateTime.UtcNow.Date.AddDays(1);
IEnumerable<PowerTrade>? trades = null;

Dictionary<int, double> aggregatedVolumes = new Dictionary<int, double>();

while (true)
{
    try
    {
        trades = powerService.GetTrades(dayAhead);
    }
    catch (Exception ex)
    {
        int retryTime = (int)(retryIntervalMinutes * 60);
        Console.WriteLine("Exception occurred while retrieving trades: " + ex.Message + " retrying in " + retryTime + " seconds");

        Thread.Sleep(retryTime * 1000);

        continue;
    }

    // Aggregate trade volumes
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

    // Write aggregated trade data to CSV
    string fileName = GenerateFileName(trades.First()); // Assuming trades is not null and contains at least one trade
    string filePath = Path.Combine(folderPath, fileName);
    WriteAggregatedTradeDataToCSV(aggregatedVolumes, filePath, trades.Count());
    aggregatedVolumes.Clear();

    Console.WriteLine("Trade successfully saved for " + dayAhead.ToString("yyyy-MM-dd"));

    int fetchTime = (int)(fetchIntervalMinutes * 60 * 1000);
    Thread.Sleep(fetchTime);
}

static string GenerateFileName(PowerTrade trade)
{
    string dayAheadDate = trade.Date.ToString("yyyyMMdd");
    string extractionDateTime = DateTime.UtcNow.ToString("yyyyMMddHHmm");
    return $"PowerPosition_{dayAheadDate}_{extractionDateTime}.csv";
}

static void WriteAggregatedTradeDataToCSV(Dictionary<int, double> aggregatedVolumes, string filePath, int count)
{
    using (StreamWriter writer = new StreamWriter(filePath))
    {
        writer.WriteLine("Period (#), Volume");

        foreach (var kvp in aggregatedVolumes)
        {
            double averageVolume = kvp.Value / count;
            writer.WriteLine($"{kvp.Key},{averageVolume}");
        }
    }
}