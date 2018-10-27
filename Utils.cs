using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Aliyun.Log
{
    static class Utils
    {
        private static DateTime _1970StartTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));

        /// <summary>
        /// 获取本地IP地址
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIP()
        {
            var ip = "";

            var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var item in ipHostInfo.AddressList)
            {
                // 获取本机的IPv4地址
                if (item.AddressFamily == AddressFamily.InterNetwork)
                {
                    ip = item.ToString();

                    if (ip.StartsWith("10.") || ip.StartsWith("172.") ||
                        ip.StartsWith("192.") || ip.StartsWith("169.254"))
                    {
                        // 局域网地址
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return ip;
        }

        /// <summary>
        /// 获取秒级时间戳
        /// </summary>
        /// <returns></returns>
        public static uint GetTimestamp()
        {
            return (uint)Math.Round((DateTime.Now - _1970StartTime).TotalSeconds, MidpointRounding.AwayFromZero);
        }
    }
}
