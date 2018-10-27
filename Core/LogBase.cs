using Aliyun.Api.LOG.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Aliyun.Log.Client;

namespace Aliyun.Log.Core
{
    /// <summary>
    /// 日志模型
    /// </summary>
    public class LogBase
    {
        /// <summary>
        /// 日志时间
        /// </summary>
        public string LogTime { get; private set; }
        /// <summary>
        /// 日志级别
        /// </summary>
        public AppLogLevel LogLevel { get; set; }
        /// <summary>
        /// 方法
        /// </summary>
        public string Method { get; set; }
        /// <summary>
        /// 标签
        /// </summary>
        public string Tag { get; set; }
        /// <summary>
        /// 日志信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 调用链
        /// </summary>
        public string ChainId { get; private set; }
        /// <summary>
        /// 跟踪ID
        /// </summary>
        public string TraceId { get; private set; }
        /// <summary>
        /// 上级跟踪ID
        /// </summary>
        public string ParentTraceId { get; private set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Direction { get; set; }

        /// <summary>
        /// 设置日志内容
        /// </summary>
        /// <param name="level"></param>
        /// <param name="method"></param>
        /// <param name="tag"></param>
        /// <param name="msg"></param>
        /// <param name="userName"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public LogBase SetContent(AppLogLevel level, string method, string tag, string msg, string userName, string direction)
        {
            this.LogTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            this.LogLevel = level;
            this.Method = method;
            this.Tag = tag;
            this.Message = msg;
            this.UserName = userName;
            this.Direction = direction;

            try
            {
                this.ChainId = (CallContext.LogicalGetData("Rpc.ChainId")?.ToString());
                this.TraceId = (CallContext.LogicalGetData("Rpc.TrackId")?.ToString());
                this.ParentTraceId = (CallContext.LogicalGetData("Rpc.ParentTrackId")?.ToString());
            }
            catch
            {
                //no code
            }

            return this;
        }

        /// <summary>
        /// 清理数据
        /// </summary>
        /// <returns></returns>
        public LogBase Clear()
        {
            this.LogTime = null;
            this.LogLevel = AppLogLevel.Trace;
            this.Method = null;
            this.Tag = null;
            this.Message = null;
            this.UserName = null;
            this.Direction = null;
            this.ChainId = null;
            this.TraceId = null;
            this.ParentTraceId = null;

            return this;
        }
    }


    /// <summary>
    /// 扩展方法
    /// </summary>
    public static class LogBaseExterns
    {
        /// <summary>
        /// 转为阿里日志模型
        /// </summary>
        /// <param name="logBase"></param>
        /// <returns></returns>
        public static LogItem ToAliLogItem(this LogBase logBase)
        {
            if (logBase == null)
                return null;

            var logItem = new LogItem
            {
                Time = Utils.GetTimestamp()
            };

            logItem.PushBack(AliLogConsts.APPNAME, Config.AppName ?? "");

            logItem.PushBack(AliLogConsts.LOGTIME, logBase.LogTime ?? "");
            logItem.PushBack(AliLogConsts.LEVEL, logBase.LogLevel.ToString());
            logItem.PushBack(AliLogConsts.METHOD, logBase.Method ?? "");
            logItem.PushBack(AliLogConsts.TAG, logBase.Tag ?? "");
            logItem.PushBack(AliLogConsts.MESSAGE, logBase.Message ?? "");

            logItem.PushBack(AliLogConsts.CHAINID, logBase.ChainId ?? "");
            logItem.PushBack(AliLogConsts.TRACEID, logBase.TraceId ?? "");
            logItem.PushBack(AliLogConsts.PARENT_TRACEID, logBase.ParentTraceId ?? "");

            return logItem;
        }
    }
}
