using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aliyun.Log.Policy
{
    /// <summary>
    /// 令牌桶限流
    /// </summary>
    class TokenBucket<TToken> where TToken : new()
    {
        /// <summary>
        /// 是否启用限流
        /// </summary>
        private readonly bool _enabledLimit;

        /// <summary>
        /// 限流池
        /// </summary>
        private readonly ConcurrentStack<TToken> _tokenPool;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="limitNum">最大处理数</param>
        public TokenBucket(int limitNum)
        {
            if (bool.TryParse(ConfigurationManager.AppSettings["Log.EnabledLimit"] ?? "", out bool enabledLimit))
                _enabledLimit = enabledLimit;
            else
                _enabledLimit = true;

            _tokenPool = new ConcurrentStack<TToken>();
            for (int i = 0; i < limitNum; i++)
                _tokenPool.Push(new TToken());
        }

        /// <summary>
        /// 获取处理令牌
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool TryGetToken(out TToken token)
        {
            if (!_enabledLimit)
            {
                token = new TToken();
                return true;
            }

            return _tokenPool.TryPop(out token);
        }

        /// <summary>
        /// 释放处理令牌
        /// </summary>
        /// <param name="token"></param>
        public void ReleaseToken(TToken token)
        {
            if (!_enabledLimit)
                return;

            if (token == null)
                token = new TToken();

            _tokenPool.Push(token);
        }
    }
}
