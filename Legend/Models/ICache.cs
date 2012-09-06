using System;
using System.Web.Caching;

namespace Legend.Models
{
    public interface ICache
    {
        object Get(string key);
        void Set(string key, object value, TimeSpan expiresIn);
        void Set(string key, object value, TimeSpan expiresIn, CacheItemRemovedCallback callback);
        void Remove(string key);
    }
}