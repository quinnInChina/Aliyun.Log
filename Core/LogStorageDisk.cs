using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aliyun.Log.Core
{
    /// <summary>
    /// 日志记入硬盘
    /// </summary>
    class LogStorageDisk
    {
        private readonly string _customDir;

        public LogStorageDisk(string rootDir)
        {
            _customDir = Path.Combine("D:\\", rootDir, Config.AppName);
            if (!Directory.Exists(_customDir))
                Directory.CreateDirectory(_customDir);
        }

        public bool Save(List<LogBase> logs)
        {
            if (logs == null || !logs.Any())
                return true;

            try
            {
                var fileName = Path.Combine(_customDir, DateTime.Now.ToString("yyyy-MM-dd") + ".log");
                using (StreamWriter writer = new StreamWriter(fileName, true))
                {
                    foreach (var log in logs)
                    {
                        writer.WriteLine(JsonConvert.SerializeObject(log));
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
