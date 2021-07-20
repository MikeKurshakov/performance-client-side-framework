using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumTest1.InfluxDB.Helpers
{
    public class Metric : IMetric
    {
        public DateTime CreatedOn { get; set; }
        public string ScenarioName { get; set; }
        public string ActionName { get; set; }
        public TimeSpan Elapsed { get; set; }
    }
}
