using System;
using System.Collections.Generic;
using UnityEngine;

namespace SkierFramework
{
    /// <summary>
    /// 任务数据
    /// </summary>
    [Serializable]
    public class TaskData
    {
        /// <summary>
        /// 任务Id
        /// </summary>
        public int taskId;

        /// <summary>
        /// 当前计数
        /// </summary>
        public int[] curCounts;

        /// <summary>
        /// 是否已领取
        /// </summary>
        public bool isGet;
    }
}
