﻿using cadence.Support;
using Newtonsoft.Json;

namespace cadence.Core
{
    /// <summary>
    /// An atomic counter metric
    /// </summary>
    public sealed class CounterMetric : IMetric
    {
        private readonly AtomicLong _count = new AtomicLong(0);

        public CounterMetric()
        {
            
        }

        private CounterMetric(long count)
        {
            _count.Set(count);
        }

        public void Increment()
        {
            Increment(1);
        }

        public void Increment(long amount)
        {
            _count.AddAndGet(amount);
        }

        public void Decrement()
        {
            Decrement(1);
        }

        public void Decrement(long amount)
        {
            _count.AddAndGet(0 - amount);
        }

        public void Clear()
        {
            _count.Set(0);
        }

        public long Count
        {
            get { return _count.Get(); }
        }

        [JsonIgnore]
        public IMetric Copy
        {
            get { return new CounterMetric(_count.Get()); }
        }
    }
}