using Aliyun.Api.LOG;
using Aliyun.Api.LOG.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aliyun.Log.Client;
using Aliyun.Log.Core;

namespace Aliyun.Log.Loggers
{
    /// <summary>
    /// 阿里日志
    /// </summary>
    class AliLogger : ILogger<LogBase>
    {
        private readonly string _localIP;

        private readonly LogStorageDisk _logStorageDisk;

        public AliLogger()
        {
            _localIP = Utils.GetLocalIP();
            _logStorageDisk = new LogStorageDisk("AliLog");
        }

        public void WriteLog(LogBase log)
        {
            BatchWriteLog(new List<LogBase> { log });
        }

        public void BatchWriteLog(List<LogBase> logs)
        {
            if (!logs.Any())
                return;

            var group = logs.GroupBy(log => log.LogLevel);

            foreach (var logGroup in group)
            {
                try
                {
                    var level = logGroup.Key;
                    var storeName = AliLogClient.GetStoreName(level);

                   AliLogClient.Client.PutLogs(new Aliyun.Api.LOG.Request.PutLogsRequest
                    {
                        Project = AliLogClient.LogProject,
                        Logstore = storeName,
                        Topic = "",
                        Source = _localIP,
                        LogItems = logGroup.ToList().ConvertAll(log => log.ToAliLogItem())
                    });
                }
                catch (LogException ex)
                {
                    if (ex.ErrorCode == "InvalidLogSize")
                    {
                        if (logGroup.Count() == 1)
                            WriteLocalLog(logGroup.ToList());
                        else
                        {
                            BatchWriteLog(logs.GetRange(0, logs.Count / 2));
                            BatchWriteLog(logs.GetRange(logs.Count / 2, logs.Count - logs.Count / 2));
                        }
                    }
                }
                catch (Exception ex)
                {
                    var list = logGroup.ToList();
                    list.Insert(0, new LogBase
                    {
                        LogLevel = AppLogLevel.Error,
                        Method = "AliLogger.PutLogs",
                        Message = ex.ToString()
                    });
                    WriteLocalLog(list);
                }
            }
        }

        public void WriteLocalLog(List<LogBase> logs)
        {
            _logStorageDisk.Save(logs);
        }
    }
}
