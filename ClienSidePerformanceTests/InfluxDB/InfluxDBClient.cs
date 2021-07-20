using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace ClienSidePerformanceTests.InfluxDB
{
    /// <summary>
    /// Client for writing data to an InfluxDB instance over HTTP.
    /// </summary>
    public class InfluxDBClient
    {
        public long utcTicks = new DateTime(1970, 1, 1).Ticks;

        private static InfluxDBClient instance;
        /// <summary>
        /// Static instance using configuration from PerformanceConfigService.
        /// </summary>
        public static InfluxDBClient Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new InfluxDBClient(
                            InfluxConfig.Host,
                            InfluxConfig.Port, 
                            InfluxConfig.User,
                            InfluxConfig.Pass,
                            InfluxConfig.Database)
                    {
                        EnableWrites = InfluxConfig.Enabled,
                        DefaultTags =
                        {
                            ["project"] = InfluxConfig.Database,
                        }
                    };
                }
                return instance;
            }
        }

        /// <summary>
        /// Host URL string.
        /// </summary>
        public string Host { get; private set; }
        /// <summary>
        /// Port number.
        /// </summary>
        public int Port { get; private set; }
        /// <summary>
        /// User name used for authentication.
        /// </summary>
        public string Username { get; private set; }
        /// <summary>
        /// Password used for authentication.
        /// </summary>
        public string Password { get; private set; }

        public string Database { get; private set; }

        public bool EnableWrites { get; private set; } = true;

        public Dictionary<string, object> DefaultTags { get; set; }
        public Dictionary<string, object> DefaultFields { get; set; }

        private HttpClient client;

        /// <summary>
        /// Creates a new InfluxDBClient with the given configuration.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        public InfluxDBClient(string host, int port, string user, string pass, string database)
        {
            Host = host;
            Port = port;
            Username = user;
            Password = pass;
            Database = database;

            var handler = new HttpClientHandler();
            //#pragma warning disable CS0618 // Type or member is obsolete
            //            handler.Proxy = WebProxy.GetDefaultProxy();
            //#pragma warning restore CS0618 // Type or member is obsolete
            //            handler.Proxy.Credentials = CredentialCache.DefaultCredentials;
            client = new HttpClient(handler);
            client.DefaultRequestHeaders.Add("Authorization", $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user}:{pass}"))}");
            client.BaseAddress = new Uri($"{Host}:{Port}");

            DefaultTags = new Dictionary<string, object>();
            DefaultFields = new Dictionary<string, object>();
        }

        /// <summary>
        /// Performs a query to a database.
        /// </summary>
        /// <param name="query">The query to perform.</param>
        /// <returns>The response returned.</returns>
        public QueryResponse Query(string query)
        {
            var response = client.GetAsync(Database == null ?
                $"/query?q={WebUtility.UrlEncode(query)}" :
                $"/query?db={Database}&q={WebUtility.UrlEncode(query)}"
                ).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Non-OK status received while executing {nameof(InfluxDBClient)}::{nameof(Query)}: {response.StatusCode}.");

            return JsonConvert.DeserializeObject<QueryResponse>(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
        }

        /// <summary>
        /// Writes data to a database.
        /// </summary>
        /// <param name="name">The name of the data.</param>
        /// <param name="tags">A list of tags.  In the form "name=value".</param>
        /// <param name="values">A list of values.  In the form "name=value".</param>
        public void Write(string name, Dictionary<string, object> tags, Dictionary<string, object> values, long? timestamp = null)
        {
            if (!EnableWrites)
                return;

            var stringBuilder = new StringBuilder();

            stringBuilder.Append(name);
            foreach (var tag in tags)
                stringBuilder.Append(',').Append($"{tag.Key}={GetTagString(tag.Value)}");
            foreach (var tag in DefaultTags)
                stringBuilder.Append(',').Append($"{tag.Key}={GetTagString(tag.Value)}");
            stringBuilder.Append(' ');
            int i = 0;
            foreach (var value in DefaultFields)
            {
                if (i++ != 0)
                    stringBuilder.Append(',');
                stringBuilder.Append(value.Key).Append('=').Append(GetValueString(value.Value));
            }
            foreach (var value in values)
            {
                if (i++ != 0)
                    stringBuilder.Append(',');
                stringBuilder.Append(value.Key).Append('=').Append(GetValueString(value.Value));
            }
            if (timestamp.HasValue)
                stringBuilder.Append(' ').Append(timestamp.Value);

            var response = client.PostAsync(
                $"/write?db={Database}",
                new ByteArrayContent(Encoding.ASCII.GetBytes(stringBuilder.ToString()))
                ).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Non-OK status received while executing {nameof(InfluxDBClient)}::{nameof(Write)}: {response.StatusCode}.\r\nRequest body: {stringBuilder}\r\nResponse body: {response.Content.ReadAsStringAsync().Result}\r\nResponse Headers: {response.Headers}");
        }

        private object GetTagString(object value)
        {
            return value.ToString().Replace("\\", "\\\\").Replace(" ", "\\ ").Replace("\"", "\\\"").Replace(",", "\\,");
        }

        private object GetValueString(object value)
        {
            if (value is short || value is int || value is long || value is ushort || value is uint || value is ulong)
                return $"{value}"; //return $"{value}i";
            if (value is bool || value is float || value is double)
                return $"{value}";
            return $"\"{value.ToString().Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\\\n").Replace("\r", "\\\\r")}\"";
        }

        public void WriteUnix(string name, Dictionary<string, object> tags, Dictionary<string, object> values, long? timestamp = null)
        {
            Write(name, tags, values, timestamp.HasValue ? (timestamp * 1000 * 1000) : null);
        }

        public void Write(string name, Dictionary<string, object> tags, Dictionary<string, object> values, DateTimeOffset timestamp)
        {
            WriteTicks(name, tags, values, timestamp.ToUniversalTime().Ticks);
        }

        public void Write(string name, Dictionary<string, object> tags, Dictionary<string, object> values, DateTime timestamp)
        {
            WriteTicks(name, tags, values, timestamp.ToUniversalTime().Ticks);
        }

        public void WriteTicks(string name, Dictionary<string, object> tags, Dictionary<string, object> values, long ticks, long dupPrevent = 0)
        {
            Write(name, tags, values, (ticks - utcTicks) * (1000000 / TimeSpan.TicksPerMillisecond) + dupPrevent);
        }

        public InfluxDBClient Copy()
        {
            var copy = new InfluxDBClient(Host, Port, Username, Password, Database);
            copy.DefaultTags = new Dictionary<string, object>(DefaultTags);
            copy.DefaultTags.Add("testclass", TestContext.CurrentContext.Test.ClassName);
            copy.DefaultTags.Add("test", TestContext.CurrentContext.Test.Name);
            copy.EnableWrites = EnableWrites;
            return copy;
        }
    }
}
