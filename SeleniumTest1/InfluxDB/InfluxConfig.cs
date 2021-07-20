using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumTest1.InfluxDB
{
    public static class InfluxConfig
    {
        public static string Host => "http://localhost";
        public static int Port => 8086;
        public static string User => "";
        public static string Pass => "";
        public static string Database => "cpt_metrics";
        public static string Measurement => "cpt_actions";
        public static bool Enabled => true;
    }
}
