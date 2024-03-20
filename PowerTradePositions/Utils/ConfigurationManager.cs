using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace PowerTradePositions.Utils
{
    internal class ConfigurationManager
    {
        private readonly IConfiguration _configuration;

        public ConfigurationManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string FolderPath => _configuration["FolderPath"];

        public double FetchIntervalMinutes => double.Parse(_configuration["FetchIntervalSeconds"]);

        public double RetryIntervalMinutes => double.Parse(_configuration["RetryIntervalSeconds"]);
    }
}
