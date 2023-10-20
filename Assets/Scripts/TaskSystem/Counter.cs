using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SkierFramework
{
    /// <summary>
    /// 计数器
    /// </summary>
    [System.Serializable]
    public class Counter
    {
        /// <summary>
        /// 类型
        /// </summary>
        public CounterType counterType;

        /// <summary>
        /// 累积值
        /// </summary>
        public Dictionary<int, int> accumulate = new Dictionary<int, int>();

        /// <summary>
        /// 计数器更新时回调，bool:是否为递增计数
        /// </summary>
        public event Action<CounterType, int, int, bool> OnCounterUpdate;

        /// <summary>
        /// 新增计数
        /// </summary>
        public void AddCount(int count = 1, int param = 0)
        {
            accumulate.TryGetValue(param, out int v);
            accumulate[param] = v + count;
            OnCounterUpdate?.Invoke(counterType, count, param, true);
        }

        /// <summary>
        /// 更新计数
        /// </summary>
        public void UpdateCount(int count, int param = 0, bool isForce = false)
        {
            if (!isForce && accumulate.TryGetValue(param, out int v))
            {
                accumulate[param] = Mathf.Max(v, count);
            }
            else
            {
                accumulate[param] = count;
            }
            OnCounterUpdate?.Invoke(counterType, accumulate[param], param, false);
        }

        /// <summary>
        /// 数量
        /// </summary>
        public int GetCount(int param = 0)
        {
            if (accumulate != null && accumulate.TryGetValue(param, out var value))
            {
                return value;
            }
            return 0;
        }
    }
}
