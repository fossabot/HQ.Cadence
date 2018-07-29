﻿// Copyright (c) HQ Corporation. All rights reserved.
// Licensed under the Reciprocal Public License, Version 1.5. See LICENSE.md in the project root for license terms.

using System;
using System.Collections.Generic;
using System.Threading;
using HQ.Cadence.Internal;
using Newtonsoft.Json;
using Random = HQ.Cadence.Support.Random;

namespace HQ.Cadence.Stats
{
    /// <summary>
    /// A random sample of a stream of <code>long</code>s. Uses Vitter's Algorithm R to
    /// produce a statistically representative sample.
    /// <see href="http://www.cs.umd.edu/~samir/498/vitter.pdf">Random Sampling with a Reservoir</see>
    /// </summary>
    public class UniformSample : ISample<UniformSample>
    {
        private readonly AtomicLong _count = new AtomicLong(0);
        private /* atomic */ readonly long[] _values;
        
        public UniformSample(int reservoirSize)
        {
            _values = new long[reservoirSize];
            Clear();
        }

        private UniformSample(long[] values)
        {
            _values = values;
        }

        /// <summary>
        /// Clears all recorded values
        /// </summary>
        public void Clear()
        {
            for (var i = 0; i < _values.Length; i++)
            {
                Interlocked.Exchange(ref _values[i], 0);
            }
            _count.Set(0);
        }

        /// <summary>
        /// Returns the number of values recorded
        /// </summary>
        public int Count
        {
            get
            {
                var c = _count.Get();
                if (c > _values.Length)
                {
                    return _values.Length;
                }
                return (int) c;
            }
        }

        /// <summary>
        /// Adds a new recorded value to the sample
        /// </summary>
        public void Update(long value)
        {
            var count = _count.IncrementAndGet();
            if (count <= _values.Length)
            {
                var index = (int) count - 1;
                Interlocked.Exchange(ref _values[index], value);
            }
            else
            {
                var random = Math.Abs(Random.NextLong()) % count;
                if (random < _values.Length)
                {
                    var index = (int) random;
                    Interlocked.Exchange(ref _values[index], value);
                }
            }
        }
        
        /// <summary>
        /// Returns a copy of the sample's values
        /// </summary>
        public ICollection<long> Values
        {
            get
            {
                var size = Count;
                var copy = new List<long>(size);
                for (var i = 0; i < size; i++)
                {
                    copy.Add(Interlocked.Read(ref _values[i]));
                }
                return copy;       
            }
        }

        [JsonIgnore]
        public UniformSample Copy
        {
            get
            { 
                var copy = new UniformSample(_values);
                copy._count.Set(_count);
                return copy;
            }
        }
    }
}