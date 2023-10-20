using System;
using System.Collections.Generic;
using UnityEngine;

namespace SkierFramework
{
    /// <summary>
    /// 方便任务计数的管理
    /// </summary>
    public class TaskCounter
    {
        /// <summary>
        /// 数据
        /// </summary>
        public Counter counterData;
        /// <summary>
        /// 任务类型
        /// </summary>
        public CounterType counterType => counterData.counterType;
        /// <summary>
        /// 当前任务的累积值
        /// </summary>
        public Dictionary<int, int> accumulate => counterData.accumulate;
        /// <summary>
        /// 该任务类型的任务
        /// </summary>
        public Dictionary<int, Task> tasks = new Dictionary<int, Task>();

        public TaskCounter(Counter counter)
        {
            this.counterData = counter;
        }

        public TaskCounter(CounterType counterType)
        {
            counterData = new Counter();
            counterData.counterType = counterType;
        }

        /// <summary>
        /// 新增计数
        /// </summary>
        public void AddCount(int count = 1, int param = 0)
        {
            counterData.AddCount(count, param);
            foreach (var task in tasks.Values)
            {
                task.AddCount(counterType, count, param);
            }
        }

        /// <summary>
        /// 更新计数
        /// </summary>
        public void UpdateCount(int count, int param = 0, bool isForce = false)
        {
            counterData.UpdateCount(count, param, isForce);
            foreach (var task in tasks.Values)
            {
                task.UpdateCount(counterType, counterData.GetCount(param), param);
            }
        }

        /// <summary>
        /// 数量
        /// </summary>
        public int GetCount(int param = 0)
        {
            return counterData.GetCount(param);
        }

        public void AddTask(Task task)
        {
            if (tasks.ContainsKey(task.taskId)) return;

            for (int i = 0; i < task.TaskConfig.taskConditions.Count; i++)
            {
                var condition = task.TaskConfig.taskConditions[i];
                if (condition.taskRecordType == TaskRecordType.Accumulate
                    && condition.counterType == counterType)
                {
                    if (accumulate.TryGetValue(condition.param, out int count))
                    {
                        task.UpdateCount(counterType, count, condition.param);
                    }
                }
            }
            tasks.Add(task.taskId, task);
        }

        public void RemoveTask(int taskId)
        {
            tasks.Remove(taskId);
        }
    }

}
