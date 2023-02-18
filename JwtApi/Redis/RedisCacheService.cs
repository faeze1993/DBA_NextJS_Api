using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace JwtApi.Redis
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">value</typeparam>
    /// <typeparam name="TK">key</typeparam>
    public class RedisCacheService<T, TK>
    {
        private static IDatabase redisContext;
        static RedisCacheService()
        {
            try
            {

                var connectionString = ConfigurationManager.ConnectionStrings["RedisDB"].ConnectionString;
                var redisConnection = ConnectionMultiplexer.Connect(connectionString);

                redisContext = redisConnection.GetDatabase();

            }
            catch (Exception ex)
            {
                ErroLog(ex);
            }
        }


        private TK _key;
        public RedisCacheService(TK key)
        {

            _key = key;
        }


        public bool Set(List<T> value)
        {
            var newHashKey = getHashKey();
            var strValue = JsonConvert.SerializeObject(value);
            return redisContext.StringSet(newHashKey, strValue);
        }

        public bool Refresh()
        {
            var primaryData = GetDateFromPrimary();
            return Set(primaryData);
        }

        public List<T> Get()
        {
            var newHashKey = getHashKey();
            try
            {
                var value = redisContext.StringGet(newHashKey);
                return JsonConvert.DeserializeObject<List<T>>(value);
            }
            catch (Exception ex)
            {
                var setResult = Refresh();
                if (setResult)
                    return Get();

                return default(List<T>);
            }
        }

        public IEnumerable<RedisKey> GetKeys()
        {
            var hosts = redisContext.Multiplexer.GetEndPoints();
            var valueList = redisContext.Multiplexer.GetServer(hosts.First()).Keys();
            return valueList;
        }

        public bool Remove()
        {
            var newHashKey = getHashKey();
            return redisContext.KeyDelete(newHashKey);
        }

        private List<T> GetDateFromPrimary()
        {

            return GetDateFromPrimary(_key);
        }
        protected virtual List<T> GetDateFromPrimary(TK key) { return null; }

        private string getHashKey()
        {
            return getHashKey(_key);
        }
        protected virtual string getHashKey(TK inKey) { return null; }

        public static bool Remove(string key)
        {
            return redisContext.KeyDelete(key);
        }


        static void ErroLog(Exception ex)
        {
            
        }
    }
}