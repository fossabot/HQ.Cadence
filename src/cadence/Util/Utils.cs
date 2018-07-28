﻿using System.Collections.Generic;
using cadence.Core;

namespace cadence.Util
{
    internal static class Utils
    {
        internal static IDictionary<string, IDictionary<string, IMetric>> SortMetrics(IDictionary<MetricName, IMetric> metrics)
        {
            var sortedMetrics = new SortedDictionary<string, IDictionary<string, IMetric>>();

            foreach(var entry in metrics)
            {
                var className = entry.Key.Class.Name;
                IDictionary<string, IMetric> submetrics;
                if(!sortedMetrics.ContainsKey(className))
                {
                    submetrics = new SortedDictionary<string, IMetric>();
                    sortedMetrics.Add(className, submetrics);
                }
                else
                {
                    submetrics = sortedMetrics[className];
                }
                submetrics.Add(entry.Key.Name, entry.Value);
            }
            return sortedMetrics;
        }
    }
}
