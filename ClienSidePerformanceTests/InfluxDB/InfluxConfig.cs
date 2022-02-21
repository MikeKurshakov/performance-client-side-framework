namespace ClienSidePerformanceTests.InfluxDB
{
    public static class InfluxConfig
    {
        public static string Host => "http://localhost:8086";
        public static string Database => "cpt_metrics";
        public static string Measurement => "cpt_actions";
        public static bool Enabled => true;
    }
}
