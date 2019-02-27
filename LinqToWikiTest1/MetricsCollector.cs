using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqToWikiTest1
{
    /// <summary>
    /// Duty - collect metrics, such as timings etc
    /// </summary>
    internal class MetricsCollector : IEnumerable<string>, IEnumerable<Metrics>
    {
        List<Metrics> metricsList = new List<Metrics>();

        public void LogAndInvoke(Action action, string tag)
        {
            var metrics = new Metrics(tag, DateTime.UtcNow);
            action();
            metrics.EndTime = DateTime.UtcNow;
            metricsList.Add(metrics);
        }

        public T LogAndInvoke<T>(Func<T> function, string tag)
        {
            var metrics = new Metrics(tag, DateTime.UtcNow);
            var result = function();
            metrics.EndTime = DateTime.UtcNow;
            metricsList.Add(metrics);
            return result;
        }

        public IEnumerator<string> GetEnumerator()
        {
            foreach(var metrics in metricsList)
                yield return metrics.ToString();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        IEnumerator<Metrics> IEnumerable<Metrics>.GetEnumerator()
        {
            foreach (var metrics in metricsList)
                yield return metrics;
        }
    }

    class Metrics
    {
        public Metrics()
        {
        }
        public Metrics(string tag)
        {
            Tag = tag;
        }
        public Metrics(string tag, DateTimeOffset startTime)
        {
            Tag = tag;
            StartTime = startTime;
        }

        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public TimeSpan Duration => EndTime - StartTime;
        public string Tag { get; set; }

        public override string ToString()
        {
            var totalMs = (EndTime - StartTime).TotalMilliseconds;
            return $"{Tag} {totalMs:F2}ms";
        }
    }
}
