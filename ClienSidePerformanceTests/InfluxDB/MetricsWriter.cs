using System;
using System.Collections.Generic;
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
                CreatedOn = DateTime.Now,
                ScenarioName = scenarioName,
                ActionName = actionName,
                Elapsed = PageLoadTime()
            };
            Write(metric);
            return metric;
        }

        private static void Write(IMetric metric)
        {
            InfluxDBClient.Instance.Copy().Write(InfluxConfig.Measurement, new Dictionary<string, object>
            {
                ["action"] = metric.ActionName,
                ["scenario"] = metric.ScenarioName,
            }, new Dictionary<string, object>
            {
                ["elapsed"] = metric.Elapsed.TotalMilliseconds
            }, metric.CreatedOn);
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
