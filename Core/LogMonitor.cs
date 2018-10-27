using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Aliyun.Log.Policy;

namespace Aliyun.Log.Core
{
    class LogMonitor<TLog> where TLog : LogBase, new()
    {
        private ILogger<TLog> _logger;

        /// <summary>
        /// 最大工作线程数
        /// </summary>
        private readonly int _maxThrdNum;
        /// <summary>
        /// 每次最大上传个数
        /// </summary>
        private readonly int _maxUploadNum;

        /// <summary>
        /// 是否启动
        /// </summary>
        private bool _start;
        /// <summary>
        /// 日志数据队列
        /// </summary>
        private readonly ConcurrentQueue<TLog> _dataQueue;
        /// <summary>
        /// 当前工作线程数
        /// </summary>
        private int _curThrdNum;

        /// <summary>
        /// 限流
        /// </summary>
        private readonly TokenBucket<TLog> _tokenBucket;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxThrdNum">最大工作线程数</param>
        /// <param name="maxUploadNum">每次最大上传个数</param>
        /// <param name="tokenBucket">令牌池</param>
        public LogMonitor(int maxThrdNum, int maxUploadNum, TokenBucket<TLog> tokenBucket)
        {
            _maxThrdNum = maxThrdNum;
            _maxUploadNum = maxUploadNum;
            _tokenBucket = tokenBucket;

            _start = false;
            _dataQueue = new ConcurrentQueue<TLog>();
            _curThrdNum = 0;
        }

        /// <summary>
        /// 启动日志监控
        /// </summary>
        /// <param name="logger">首选日志</param>
        public void Start(ILogger<TLog> logger)
        {
            if (logger == null)
                return;

            _logger = logger;

            _start = true;

            //主工作线程
            new Thread(new ThreadStart(WorkerMaster)) { IsBackground = true }.Start();
            //守护线程
            new Thread(new ThreadStart(Guard)) { IsBackground = true }.Start();
        }

        /// <summary>
        /// 停止日志监控
        /// </summary>
        public void Stop()
        {
            _start = false;
        }

        /// <summary>
        /// 记入日志
        /// </summary>
        /// <param name="logData"></param>
        public void WriteLog(TLog logData)
        {
            if (logData != null)
            {
                _dataQueue.Enqueue(logData);
            }
        }

        /// <summary>
        /// 主工作线程
        /// </summary>
        private void WorkerMaster()
        {
            Interlocked.Increment(ref _curThrdNum);
            while (_start)
            {
                if (_dataQueue.IsEmpty)
                {
                    Thread.Sleep(100);
                    continue;
                }

                WriteAndReleaseToken();
            }
            Interlocked.Decrement(ref _curThrdNum);
        }

        /// <summary>
        /// 副工作线程
        /// </summary>
        /// <param name="state"></param>
        private void WrokerSlaver(object state)
        {
            Interlocked.Increment(ref _curThrdNum);

            WriteAndReleaseToken();

            Interlocked.Decrement(ref _curThrdNum);
        }

        /// <summary>
        /// 守护线程
        /// </summary>
        private void Guard()
        {
            while (_start)
            {
                Thread.Sleep(500);
                if (!_start) break;
                
                if (_dataQueue.Count > 30 && _curThrdNum < _maxThrdNum)
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(WrokerSlaver));
                }
            }
        }

        /// <summary>
        /// 写入日志并释放令牌
        /// </summary>
        private void WriteAndReleaseToken()
        {
            var loglist = new List<TLog>();
            try
            {
                int count = 0;
                while (!_dataQueue.IsEmpty && count < _maxUploadNum)
                {
                    TLog data;
                    if (_dataQueue.TryDequeue(out data))
                        loglist.Add(data);
                    else
                        break;
                    count++;
                }

                if (loglist.Any())
                {
                    _logger.BatchWriteLog(loglist);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                foreach (var log in loglist)
                {
                    log.Clear();
                    _tokenBucket.ReleaseToken(log);
                }
            }
        }

        /// <summary>
        /// 写本地日志
        /// </summary>
        /// <param name="logData"></param>
        public void WriteLocalLog(TLog logData)
        {
            if (logData != null)
            {
                var list = new List<TLog> { logData };
                _logger.WriteLocalLog(list);
            }
        }
    }
}
