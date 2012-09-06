using System;
using System.Collections.Concurrent;

namespace Legend.Infrastructure
{
    // Need this because serialization doesn't store case insensitivity on a normal concurrent dictionary
    public class CaseInsensitiveConcurrentDictionary<T> : ConcurrentDictionary<string, T>
    {
        public CaseInsensitiveConcurrentDictionary(): base(StringComparer.OrdinalIgnoreCase) { }
    }
}