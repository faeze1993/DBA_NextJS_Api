using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JwtApi.Redis.RedisModel
{
    public class UserData
    {
        public int UserId { get; set; }
        public string Token { get; set; }
        public DateTime AccessTokenExpireTime { get; set; }
    }
}