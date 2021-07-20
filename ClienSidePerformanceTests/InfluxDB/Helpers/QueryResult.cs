namespace SeleniumTest1.InfluxDB
{
    public class QueryResult
    {
        public int statement_id { get; set; }
        public Series[] series { get; set; }
    }
}
