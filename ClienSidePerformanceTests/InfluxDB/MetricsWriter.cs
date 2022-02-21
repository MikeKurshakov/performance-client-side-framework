using System;
using System.Collections.Generic;
using InfluxDB.Collector;
using ClienSidePerformanceTests.InfluxDB.Helpers;

namespace ClienSidePerformanceTests.InfluxDB
{
    public class MetricsWriter : TestBase
    {
        public static IMetric MeasurePageLoad(string scenarioName, string actionName, Action action)
        {
            var startTime = (long)js.ExecuteScript("return window.performance.timing.navigationStart");

            action.Invoke();

            fluentWait.Until(x => (long)js.ExecuteScript("return window.performance.timing.navigationStart") != startTime);

            Metric metric = new Metric
            {
                ScenarioName = scenarioName,
                ActionName = actionName,
                Elapsed = PageLoadTime()
            };

            Write(metric);
            return metric;
        }

        private static void Write(IMetric metric)
        {
            Metrics.Write(InfluxConfig.Measurement, new Dictionary<string, object>
            {
                ["elapsed"] = metric.Elapsed.TotalMilliseconds
            }, new Dictionary<string, string>
            {
                ["action"] = metric.ActionName,
                ["scenario"] = metric.ScenarioName,
            });
        }

        public static TimeSpan PageLoadTime()
        {
            double pageLoadTime = 0.0;
            fluentWait.Until(x =>
            {
                if (!(bool)js.ExecuteScript("return document.readyState==='complete'"))
                    return false;
                pageLoadTime = (double)(long)js.ExecuteScript("return performance.timing.loadEventEnd - performance.timing.navigationStart");
                return pageLoadTime >= 0.0;
            });
            return TimeSpan.FromMilliseconds(Convert.ToDouble(pageLoadTime));
        }
    }
}
