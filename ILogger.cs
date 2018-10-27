using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aliyun.Log.Core;

namespace Aliyun.Log
{
    /// <summary>
    /// 日志接口
    /// </summary>
    public interface ILogger<TLog> where TLog : LogBase
    {
        /// <summary>
        /// 写远程日志
        /// </summary>
        /// <param name="log"></param>
        void WriteLog(TLog log);

        /// <summary>
        /// 批量写远程日志
        /// </summary>
        /// <param name="logs"></param>
        void BatchWriteLog(List<TLog> logs);

        /// <summary>
        /// 写本地日志
        /// </summary>
        /// <param name="logs"></param>
        void WriteLocalLog(List<TLog> logs);
    }
}
