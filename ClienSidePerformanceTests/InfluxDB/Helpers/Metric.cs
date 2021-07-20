using System;

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
