using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aliyun.Log.Client;
using Aliyun.Log.Core;

namespace Aliyun.Log
{
    static class Config
    {
        private static AppLogLevel? _AppLogLevel;
        /// <summary>
        /// 日志记录级别
        /// </summary>
        public static AppLogLevel AppLogLevel
        {
            get
            {
                if (_AppLogLevel.HasValue)
                    return _AppLogLevel.Value;

                var level = ConfigurationManager.AppSettings["Log.AppLogLevel"] ?? "";
                switch (level.ToLower())
                {
                    case "fatal":
                        _AppLogLevel = AppLogLevel.Fatal;
                        break;
                    case "error":
                        _AppLogLevel = AppLogLevel.Error;
                        break;
                    case "warn":
                        _AppLogLevel = AppLogLevel.Warn;
                        break;
                    case "info":
                        _AppLogLevel = AppLogLevel.Info;
                        break;
                    case "debug":
                        _AppLogLevel = AppLogLevel.Debug;
                        break;
                    case "trace":
                        _AppLogLevel = AppLogLevel.Trace;
                        break;
                    case "off":
                        _AppLogLevel = AppLogLevel.Off;
                        break;
                    default:
                        _AppLogLevel = AppLogLevel.Debug;
                        break;
                }
                return _AppLogLevel.Value;
            }
        }

        /// <summary>
        /// 运行环境
        /// </summary>
        public static string RunPlat
        {
            get
            {
                var plat = ConfigurationManager.AppSettings["Log.Environment"] ?? "";
                switch (plat.ToLower())
                {
                    case "dev":
                        return "-dev";
                    case "qa":
                        return "-qa";
                    case "uat":
                        return "-uat";
                    case "prod":
                        return "";
                    default:
                        return "-dev";
                }
            }
        }

        /// <summary>
        /// 应用名称
        /// </summary>
        public static string AppName
        {
            get
            {
                var appName = ConfigurationManager.AppSettings["Log.AppName"];
                if (string.IsNullOrEmpty(appName))
                    return "logAppefault";

                return appName;
            }
        }


        /// <summary>
        /// 日志平台(默认阿里云)
        /// </summary>
        public static LogEnvironment Environment
        {
            get
            {
                var envir = ConfigurationManager.AppSettings["Log.Platform"];
                if (Enum.TryParse(envir, out LogEnvironment logEnvir))
                    return logEnvir;
                else
                    return LogEnvironment.Aliyun;
            }
        }

        /// <summary>
        /// 最大处理日志数
        /// </summary>
        public static int MaxHandleNum
        {
            get
            {
                var maxNum = ConfigurationManager.AppSettings["Log.MaxHandleNum"];
                if (int.TryParse(maxNum, out int num))
                    return num;

                return 100000;
            }
        }

        /// <summary>
        /// 最大工作者线程
        /// </summary>
        public static int MaxThrdNum
        {
            get
            {
                var thrdNum = ConfigurationManager.AppSettings["Log.MaxWorkerNum"];
                if (int.TryParse(thrdNum, out int num))
                    return num;

                return 5;
            }
        }

        /// <summary>
        /// 每次最大上传条数
        /// </summary>
        public static int MaxUploadNum
        {
            get
            {
                var uploadNum = ConfigurationManager.AppSettings["Log.MaxUploadNumOnce"];
                if (int.TryParse(uploadNum, out int num))
                    return num;

                return 200;
            }
        }
    }
}
