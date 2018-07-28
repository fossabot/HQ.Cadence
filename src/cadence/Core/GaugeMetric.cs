﻿using System;
using Newtonsoft.Json;

namespace cadence.Core
{
    public abstract class GaugeMetric
    {
        public abstract string ValueAsString { get; }
    }

    /// <summary>
    /// A gauge metric is an instantaneous reading of a partiular value. To
    /// instrument a queue's depth, for example:
    /// <example>
    /// <code> 
    /// var queue = new Queue{int}();
    /// var gauge = new GaugeMetric{int}(() => queue.Count);
    /// </code>
    /// </example>
    /// </summary>
    public sealed class GaugeMetric<T> : GaugeMetric, IMetric
    {
        private readonly Func<T> _evaluator;

        public GaugeMetric(Func<T> evaluator)
        {
            _evaluator = evaluator;
        }

        public T Value
        {
            get { return _evaluator.Invoke(); }
        }

        public override string ValueAsString
        {
            get { return Value.ToString(); }
        }

        [JsonIgnore]
        public IMetric Copy
        {
            get { return new GaugeMetric<T>(_evaluator); }
        }
    }
}