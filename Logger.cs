using Aliyun.Api.LOG.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Aliyun.Log.Core;
using Aliyun.Log.Loggers;
using Aliyun.Log.Policy;

namespace Aliyun.Log
{
    /// <summary>
    /// 
    /// </summary>
    public static class Logger
    {
        private static readonly TokenBucket<LogBase> _tokenBucket;
        private static readonly LogMonitor<LogBase> _monitor;

        /// <summary>
        /// 初始化日志
        /// </summary>
        static Logger()
        {
            ReleaseRefDLL("zlib32.dll", Properties.Resources.zlib32);
            ReleaseRefDLL("zlib64.dll", Properties.Resources.zlib64);

            var logger = LoggerFactory.CreateLogger(Config.Environment);

            _tokenBucket = new TokenBucket<LogBase>(Config.MaxHandleNum);

            _monitor = new LogMonitor<LogBase>(Config.MaxThrdNum, Config.MaxUploadNum, _tokenBucket);
            _monitor.Start(logger);
        }

        private static void ReleaseRefDLL(string dllName, byte[] source)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string path = assembly.Location.Substring(0, assembly.Location.LastIndexOf('\\'));
            if (File.Exists(Path.Combine(path, dllName)))
                return;

            using (var fs = new FileStream(Path.Combine(path, dllName), FileMode.Create))
            {
                fs.Write(source, 0, source.Length);
            }
        }

        /// <summary>
        /// 记录调试日志
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method"></param>
        /// <param name="tag"></param>
        /// <param name="obj"></param>
        public static void Debug<T>(string method, string tag, T obj) where T : class
        {
            if (Config.AppLogLevel > AppLogLevel.Debug)
                return;

            var message = ConvertToString(obj);
            WriteLog(AppLogLevel.Debug, method, tag, message);
        }

        /// <summary>
        /// 记录一般日志
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method"></param>
        /// <param name="tag"></param>
        /// <param name="obj"></param>
        public static void Info<T>(string method, string tag, T obj) where T : class
        {
            if (Config.AppLogLevel > AppLogLevel.Info)
                return;

            var message = ConvertToString(obj);
            WriteLog(AppLogLevel.Info, method, tag, message);
        }

        /// <summary>
        /// 记录警告日志
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method"></param>
        /// <param name="tag"></param>
        /// <param name="obj"></param>
        public static void Warn<T>(string method, string tag, T obj) where T : class
        {
            if (Config.AppLogLevel > AppLogLevel.Warn)
                return;

            var message = ConvertToString(obj);
            WriteLog(AppLogLevel.Warn, method, tag, message);
        }

        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method"></param>
        /// <param name="tag"></param>
        /// <param name="msgObj"></param>
        /// <param name="ex"></param>
        public static void Error<T>(string method, string tag, T msgObj, Exception ex = null) where T : class
        {
            if (Config.AppLogLevel > AppLogLevel.Error)
                return;

            var exMsg = ex == null ? "" : ConvertToString(ex);
            var msg = msgObj == null ? "" : ConvertToString(msgObj);
            if (!string.IsNullOrEmpty(msg) && !string.IsNullOrEmpty(exMsg))
                exMsg = msg + Environment.NewLine + exMsg;
            else
                exMsg = msg + exMsg;
            WriteLog(AppLogLevel.Error, method, tag, exMsg);
        }

        /// <summary>
        /// 记录严重错误日志
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method"></param>
        /// <param name="tag"></param>
        /// <param name="msgObj"></param>
        /// <param name="ex"></param>
        public static void Fatal<T>(string method, string tag, T msgObj, Exception ex = null) where T : class
        {
            if (Config.AppLogLevel > AppLogLevel.Fatal)
                return;

            var exMsg = ex == null ? "" : ConvertToString(ex);
            var msg = msgObj == null ? "" : ConvertToString(msgObj);
            if (!string.IsNullOrEmpty(msg) && !string.IsNullOrEmpty(exMsg))
                exMsg = msg + Environment.NewLine + exMsg;
            else
                exMsg = msg + exMsg;
            WriteLog(AppLogLevel.Error, method, tag, exMsg);
        }

        /// <summary>
        /// 记录应用请求/返回日志（适用于移动端需求）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="userName"></param>
        /// <param name="direction"></param>
        /// <param name="method"></param>
        /// <param name="tag"></param>
        /// <param name="obj"></param>
        public static void AppMsg<T>(string userName, string direction, string method, string tag, T obj)
        {
            if (Config.AppLogLevel > AppLogLevel.Warn)
                return;

            var message = ConvertToString(obj);
            WriteLog(AppLogLevel.Info, method, tag, message, userName, direction);
        }

        /// <summary>
        /// 记录应用操作日志（适用于移动端需求）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method"></param>
        /// <param name="tag"></param>
        /// <param name="obj"></param>
        public static void AppWork<T>(string method, string tag, T obj)
        {
            if (Config.AppLogLevel > AppLogLevel.Warn)
                return;

            var message = ConvertToString(obj);
            WriteLog(AppLogLevel.Info, method, tag, message);
        }

        /// <summary>
        /// 记录应用错误日志（适用于移动端需求）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method"></param>
        /// <param name="tag"></param>
        /// <param name="msgObj"></param>
        /// <param name="ex"></param>
        public static void AppError<T>(string method, string tag, T msgObj, Exception ex = null)
        {
            if (Config.AppLogLevel > AppLogLevel.Fatal)
                return;

            var exMsg = ConvertToString(ex);
            var msg = ConvertToString(msgObj);
            if (!string.IsNullOrEmpty(msg) && !string.IsNullOrEmpty(exMsg))
                exMsg = msg + Environment.NewLine + exMsg;
            else
                exMsg = msg + exMsg;

            WriteLog(AppLogLevel.Error, method, tag, exMsg);
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static string ConvertToString<T>(T obj)
        {
            if (obj == null)
                return "";

            if (typeof(T) == typeof(string))
                return obj.ToString();

            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <param name="method">方法名</param>
        /// <param name="tag">标签</param>
        /// <param name="msg">消息</param>
        /// <param name="userName">用户名</param>
        /// <param name="direction">Request/Reply</param>
        private static void WriteLog(AppLogLevel level, string method, string tag, string msg, string userName = null, string direction = null)
        {
            LogBase log = null;
            try
            {
                if (!_tokenBucket.TryGetToken(out log))
                {
                    log = new LogBase().SetContent(level, method, tag, msg, userName, direction);
                    _monitor.WriteLocalLog(log);
                    return;
                }

                log.SetContent(level, method, tag, msg, userName, direction);

                _monitor.WriteLog(log);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
