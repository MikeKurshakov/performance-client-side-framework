using System;

namespace ClienSidePerformanceTests.InfluxDB.Helpers
{
    public interface IMetric
    {
        string ScenarioName { get; }

        string ActionName { get; }

        TimeSpan Elapsed { get; }
    }
}
