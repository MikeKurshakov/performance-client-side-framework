using System;

namespace ClienSidePerformanceTests.InfluxDB.Helpers
{
    public interface IMetric
    {
        DateTime CreatedOn { get; }

        string ScenarioName { get; }

        string ActionName { get; }

        TimeSpan Elapsed { get; }
    }
}
