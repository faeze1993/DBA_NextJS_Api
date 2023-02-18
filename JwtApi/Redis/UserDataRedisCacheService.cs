using JwtApi.Redis.RedisModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JwtApi.Redis
{
    public class UserDataRedisCacheService
    {
        /// <summary>
        /// موارد استفاده:
        /// logout
        /// اگر زمان استفاده نکردن از برنامه از یک زمانی مثلا ده دقسقه که در وب کانفیگ ست شده است بیشتر شد کاربر ار کلید توکن را از redis پاک کرده و لاگوت کند
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool RemoveTokenKeyFromRedis(string token)
        {
            var deleted = new BaseUserDataRedisCacheService(token).Remove();
            return deleted;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static List<UserData> Get(string token)
        {
            var userDataList = new BaseUserDataRedisCacheService(token).Get();
            return userDataList;
        }

        public static bool Set(string token, List<UserData> valueList)
        {
            var isInserted = new BaseUserDataRedisCacheService(token).Set(valueList);
            return isInserted;
        }

        //public static void GetAllKeyInRedisForTest(string token)
        //{
        //    var list = new BaseUserDataRedisCacheService(token).GetKeys();
        //}
    }
}