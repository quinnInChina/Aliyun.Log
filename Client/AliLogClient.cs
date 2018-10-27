using Aliyun.Api.LOG;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aliyun.Log.Client
{
    /// <summary>
    /// 阿里日志客户端
    /// </summary>
    static class AliLogClient
    {
        private static readonly string _defaultLogProject = "log_project";

        private static readonly Dictionary<AppLogLevel, string> _storeNames = new Dictionary<AppLogLevel, string>
        {
            { AppLogLevel.Trace, "debug" },
            { AppLogLevel.Debug, "debug" },
            { AppLogLevel.Info, "info" },
            { AppLogLevel.Warn, "warn" },
            { AppLogLevel.Error, "error" },
            { AppLogLevel.Fatal, "error" }
        };

        static AliLogClient()
        {
            Client = CreateClient();

            var proj = ConfigurationManager.AppSettings["Log.Project"];
            if (string.IsNullOrEmpty(proj))
                proj = _defaultLogProject;

            LogProject = proj + Config.RunPlat;
        }

        /// <summary>
        /// 日志客户端
        /// </summary>
        public static LogClient Client { get; private set; }
        /// <summary>
        /// 日志项目名
        /// </summary>
        public static string LogProject { get; private set; }

        /// <summary>
        /// 获取储存文件名
        /// </summary>
        /// <param name="logLevel"></param>
        /// <returns></returns>
        public static string GetStoreName(AppLogLevel logLevel)
        {
            if (!_storeNames.ContainsKey(logLevel))
                return _storeNames[AppLogLevel.Info];

            return _storeNames[logLevel];
        }

        private static LogClient CreateClient()
        {
            string ak = ConfigurationManager.AppSettings["Log.AccessKey"];
            string sk = ConfigurationManager.AppSettings["Log.SecretKey"];
            if (string.IsNullOrEmpty(ak) || string.IsNullOrEmpty(sk))
                throw new Exception("create aliyun log client failed, ak/sk cannot be empty");

            string endpoint = ConfigurationManager.AppSettings["Log.AliEndpoint"];
            if (string.IsNullOrEmpty(endpoint))
                throw new Exception("create aliyun log client failed, endpoint cannot be empty");

            return new LogClient(endpoint, ak, sk);
        }
    }
}
