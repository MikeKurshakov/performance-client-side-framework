using System;

namespace SeleniumTest1.InfluxDB.Helpers
{
    public interface IMetric
    {
        DateTime CreatedOn { get; }

        string ScenarioName { get; }

        string ActionName { get; }

        TimeSpan Elapsed { get; }
    }
}
