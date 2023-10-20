using System;
using System.Collections.Generic;
using UnityEngine;

namespace SkierFramework
{
    public class Task
    {
        public TaskData taskData;

        public int taskId => taskData.taskId;
        public int[] curCounts => taskData.curCounts;
        public bool isGet => taskData.isGet;

        public bool IsFinish
        {
            get
            {
                for (int i = 0; i < TaskConfig.taskConditions.Count; i++)
                {
                    if (TaskConfig.taskConditions[i].maxCount > curCounts[i])
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        private TaskConfig _taskConfig;
        public TaskConfig TaskConfig => _taskConfig;

        /// <summary>
        /// 任务数据刷新
        /// </summary>
        public event Action<int, TaskProgress> OnTaskRefresh;

        public Task(int taskId, TaskConfig taskConfig)
        {
            taskData = new TaskData();
            taskData.taskId = taskId;
            _taskConfig = taskConfig;
            if (taskConfig != null)
            {
                taskData.curCounts = new int[taskConfig.taskConditions.Count];
            }
            taskData.isGet = false;
        }

        public Task(TaskData taskData, TaskConfig taskConfig)
        {
            this.taskData = taskData;
            _taskConfig = taskConfig;
        }

        /// <summary>
        /// 新增计数
        /// </summary>
        public void AddCount(CounterType counterType, int count = 1, int param = 0)
        {
            if (IsFinish) return;

            bool success = false;
            for (int i = 0; i < TaskConfig.taskConditions.Count; i++)
            {
                var condition = TaskConfig.taskConditions[i];
                if (condition.counterType == counterType && condition.param == param)
                {
                    success = true;
                    curCounts[i] = Mathf.Min(curCounts[i] + count, condition.maxCount);
                }
            }
            if (success)
            {
                OnTaskRefresh?.Invoke(taskId, IsFinish ? TaskProgress.ConditionFinish : TaskProgress.InProgress);
            }
        }

        /// <summary>
        /// 更新计数
        /// </summary>
        public void UpdateCount(CounterType counterType, int count, int param = 0)
        {
            if (IsFinish) return;

            bool success = false;
            for (int i = 0; i < TaskConfig.taskConditions.Count; i++)
            {
                var condition = TaskConfig.taskConditions[i];
                if (condition.counterType == counterType && condition.param == param)
                {
                    success = true;
                    curCounts[i] = Mathf.Min(count, condition.maxCount);
                }
            }
            if (success)
            {
                OnTaskRefresh?.Invoke(taskId, IsFinish ? TaskProgress.ConditionFinish : TaskProgress.InProgress);
            }
        }

        /// <summary>
        /// 尝试获得奖励
        /// </summary>
        /// <returns></returns>
        public bool TryGetReward()
        {
            if (IsFinish && !isGet)
            {
                taskData.isGet = true;
                OnTaskRefresh?.Invoke(taskId, TaskProgress.GetReward);
                return true;
            }
            return false;
        }
    }

}
