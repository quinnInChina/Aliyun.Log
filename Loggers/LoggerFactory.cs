using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aliyun.Log.Core;

namespace Aliyun.Log.Loggers
{
    /// <summary>
    /// 日志工厂
    /// </summary>
    static class LoggerFactory
    {
        /// <summary>
        /// 创建日志处理器
        /// </summary>
        /// <param name="logEnvir"></param>
        /// <returns></returns>
        public static ILogger<LogBase> CreateLogger(LogEnvironment logEnvir)
        {
            switch (logEnvir)
            {
                case LogEnvironment.Aliyun:
                    return new AliLogger();

                default:
                    return new AliLogger();
            }
        }
    }
}
