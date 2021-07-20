namespace ClienSidePerformanceTests.InfluxDB
{
    public class Series
    {
        public string name { get; set; }
        public string[] columns { get; set; }
        public object[] values { get; set; }
    }
}
