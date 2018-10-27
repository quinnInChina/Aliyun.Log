using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aliyun.Log.Client
{
    /// <summary>
    /// 阿里日志静态变量
    /// </summary>
    static class AliLogConsts
    {
        /// <summary>
        /// 应用程序名
        /// </summary>
        public const string APPNAME = "appname";
        /// <summary>
        /// 日志时间
        /// </summary>
        public const string LOGTIME = "logtime";
        /// <summary>
        /// 日志级别
        /// </summary>
        public const string LEVEL = "level";
        /// <summary>
        /// 方法
        /// </summary>
        public const string METHOD = "method";
        /// <summary>
        /// 标签
        /// </summary>
        public const string TAG = "tag";
        /// <summary>
        /// 日志内容
        /// </summary>
        public const string MESSAGE = "message";
        /// <summary>
        /// 调用链
        /// </summary>
        public const string CHAINID = "chainid";
        /// <summary>
        /// 跟踪ID
        /// </summary>
        public const string TRACEID = "traceid";
        /// <summary>
        /// 上级ID
        /// </summary>
        public const string PARENT_TRACEID = "parent_traceid";
    }
}
