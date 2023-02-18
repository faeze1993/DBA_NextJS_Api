using JwtApi.Redis.RedisModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JwtApi.Redis
{
    public class BaseUserDataRedisCacheService: RedisCacheService<UserData, string>
    {
        public BaseUserDataRedisCacheService(string key) : base(key)
        {
        }

        protected override List<UserData> GetDateFromPrimary(string key)
        {
            var data = new List<UserData>();
            return data;
        }


        protected override string getHashKey(string inKey)
        {
            return string.Format("Token-{0}", inKey);
        }
    }
}