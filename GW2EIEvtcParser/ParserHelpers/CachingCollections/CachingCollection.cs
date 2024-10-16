﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace GW2EIEvtcParser
{
    public class CachingCollection<T>(ParsedEvtcLog log, int initiaPrimaryCapacity, int initialSecondaryCapacity) : AbstractCachingCollection<T>(log)
    {
		readonly int _initialSecondaryCap = initialSecondaryCapacity;
        private readonly Dictionary<long, Dictionary<long, T>> _cache = new(initiaPrimaryCapacity);

        public bool TryGetValue(long start, long end, [NotNullWhen(true)] out T? value)
        {
            (start, end) = SanitizeTimes(start, end);
            if (_cache.TryGetValue(start, out Dictionary<long, T> subCache))
            {
                if (subCache.TryGetValue(end, out value!))
                {
                    return true;
                }
            }
            value = default;
            return false;
        }

        public void Set(long start, long end, T value)
        {
            (start, end) = SanitizeTimes(start, end);

            if (!_cache.TryGetValue(start, out Dictionary<long, T> subCache))
            {
                _cache[start] = new Dictionary<long, T>(_initialSecondaryCap);
                subCache = _cache[start];
            }
            subCache[end] = value;
        }

        public bool HasKeys(long start, long end)
        {
            return TryGetValue(start, end, out _);
        }

        public T? Get(long start, long end)
        {
            if (TryGetValue(start, end, out T? value))
            {
                return value;
            }
            return default;
        }

        public override void Clear()
        {
            _cache.Clear();
        }

    }
}
