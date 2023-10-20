using System;
using System.Collections.Generic;
using UnityEngine;

namespace SkierFramework
{
    /// <summary>
    /// 任务进度
    /// </summary>
    public enum TaskProgress
    {
        /// <summary>
        /// 任务开始
        /// </summary>
        Start,
        /// <summary>
        /// 进行中
        /// </summary>
        InProgress,
        /// <summary>
        /// 条件达成
        /// </summary>
        ConditionFinish,
        /// <summary>
        /// 领取奖励
        /// </summary>
        GetReward,
    }

    /// <summary>
    /// 任务系统
    /// </summary>
    public class TaskSystem
    {
        public static TaskSystem Instance { get; private set; }
        /// <summary>
        /// 所有计数器
        /// </summary>
        private Dictionary<CounterType, TaskCounter> _taskCounters = new Dictionary<CounterType, TaskCounter>();
        /// <summary>
        /// 所有任务
        /// </summary>
        private Dictionary<int, Task> _tasks = new Dictionary<int, Task>();
        /// <summary>
        /// 所有类型的任务归类，方便查找
        /// </summary>
        private Dictionary<TaskType, List<TaskConfig>> _taskConfigs = new Dictionary<TaskType, List<TaskConfig>>();
        /// <summary>
        /// 具体的系统存档数据
        /// </summary>
        private TaskSystemData _taskSystemData;

        public Dictionary<CounterType, TaskCounter> TaskCounters => _taskCounters;
        public Dictionary<int, Task> Tasks => _tasks;
        public Dictionary<TaskType, List<TaskConfig>> TaskConfigs => _taskConfigs;

        /// <summary>
        /// 计数器刷新事件
        /// </summary>
        public event Action<CounterType, int> OnTaskCounterRefresh;
        /// <summary>
        /// 任务刷新事件：任务开始，进度变化，完成，领取奖励都会刷新
        /// </summary>
        public event Action<int, TaskProgress> OnTaskRefresh;

        public void InitSystem(TaskSystemData taskSystemData = null)
        {
            Instance = this;
            _taskCounters.Clear();
            _tasks.Clear();

            if (taskSystemData != null)
            {
                if (taskSystemData.counters != null)
                {
                    foreach (var counter in taskSystemData.counters)
                    {
                        counter.OnCounterUpdate += OnCounterUpdate;
                        _taskCounters.Add(counter.counterType, new TaskCounter(counter));
                    }
                }
                if (taskSystemData.taskDatas != null)
                {
                    foreach (var data in taskSystemData.taskDatas)
                    {
                        var task = new Task(data, ConfigManager.Instance.GetConfig<TaskConfig>(data.taskId));
                        InnerAddTask(task);
                    }
                }
                _taskSystemData = taskSystemData;
            }
            else
            {
                _taskSystemData = new TaskSystemData();
            }

            _taskConfigs.Clear();
            // 默认开启，以及成就等任务自动开启
            var configs = ConfigManager.Instance.GetAll<TaskConfig>();
            if (configs != null)
            {
                foreach (TaskConfig taskConfig in configs.Values)
                {
                    if ((taskConfig.isDefaultOpen || taskConfig.taskType == TaskType.Achievement) && !_tasks.ContainsKey(taskConfig.id))
                    {
                        StartTask(taskConfig.id);
                    }

                    if (!_taskConfigs.TryGetValue(taskConfig.taskType, out var list))
                    {
                        list = new List<TaskConfig>();
                        _taskConfigs.Add(taskConfig.taskType, list);
                    }
                    list.Add(taskConfig);
                }
            }
        }

        public TaskSystemData ReleaseSystem()
        {
            Instance = null;
            _taskCounters.Clear();
            _tasks.Clear();
            return _taskSystemData;
        }

        public List<TaskConfig> GetTaskConfigs(TaskType taskType)
        {
            if (_taskConfigs.TryGetValue(taskType, out var list))
            {
                return list;
            }
            return null;
        }

        public int GetCount(CounterType counterType)
        {
            return GetCounter(counterType).GetCount();
        }

        public Task GetTask(int taskId)
        {
            if (_tasks.TryGetValue(taskId, out Task task))
            {
                return task;
            }
            return null;
        }

        /// <summary>
        /// 开始一个任务
        /// </summary>
        public void StartTask(int taskId)
        {
            if (_tasks.ContainsKey(taskId))
            {
                Debug.LogError($"已存在该任务：{taskId}");
                return;
            }
            var taskConfig = ConfigManager.Instance.GetConfig<TaskConfig>(taskId);
            if (taskConfig == null)
            {
                Debug.LogError($"不存在任务配置：{taskId}");
                return;
            }
            StartTask(new Task(taskId, taskConfig));
        }

        /// <summary>
        /// 开始一个任务
        /// </summary>
        public void StartTask(Task task)
        {
            if (_tasks.ContainsKey(task.taskId))
            {
                Debug.LogError($"已存在该任务：{task.taskId}");
                return;
            }
            InnerAddTask(task);
            OnTaskRefresh?.Invoke(task.taskId, TaskProgress.Start);
        }

        /// <summary>
        /// 完成一个任务，可领奖
        /// </summary>
        public bool TaskGetReward(int taskId)
        {
            if (_tasks.TryGetValue(taskId, out Task task))
            {
                if (task.isGet)
                {
                    Debug.Log(string.Format("当前任务已被领取：{0}", taskId));
                }
                else
                {
                    if (task.TryGetReward())
                    {
                        // 实际奖励 TODO task.TaskConfig.rewardId;
                        return true;
                    }
                    else
                    {
                        Debug.Log(string.Format("当前任务未完成：{0}/{1}", task.curCounts[0], task.TaskConfig.taskConditions[0].maxCount));
                    }
                }
            }
            else
            {
                Debug.LogError("不存在该Id的任务：" + taskId);
            }
            return false;
        }

        private void InnerAddTask(Task task)
        {
            if (_tasks.ContainsKey(task.taskId)) return;

            task.OnTaskRefresh += (id, progress) => { OnTaskRefresh?.Invoke(id, progress); };
            _tasks.Add(task.taskId, task);
            _taskSystemData.taskDatas.Add(task.taskData);

            foreach (var taskCondition in task.TaskConfig.taskConditions)
            {
                GetCounter(taskCondition.counterType).AddTask(task);
            }
        }

        /// <summary>
        /// 移除一个任务
        /// </summary>
        public void RemoveTask(int id)
        {
            if (_tasks.TryGetValue(id, out var task))
            {
                foreach (var condition in task.TaskConfig.taskConditions)
                {
                    if (_taskCounters.TryGetValue(condition.counterType, out var taskCounter))
                    {
                        taskCounter.RemoveTask(id);
                    }
                }
                _taskSystemData.taskDatas.Remove(task.taskData);
                _tasks.Remove(id);
            }
        }

        /// <summary>
        /// 外部业务触发了该事件时调用
        /// </summary>
        public void AddCount(CounterType counterType, int num = 1, int param = 0)
        {
            GetCounter(counterType).AddCount(num, param);
        }

        /// <summary>
        /// 外部业务触发了该事件时调用
        /// </summary>
        public void UpdateCount(CounterType counterType, int count, int param = 0, bool isForce = false)
        {
            GetCounter(counterType).UpdateCount(count, param, isForce);
        }

        /// <summary>
        /// 获取任务计数器
        /// </summary>
        public TaskCounter GetCounter(CounterType counterType)
        {
            if (!_taskCounters.TryGetValue(counterType, out var counter))
            {
                counter = new TaskCounter(counterType);
                counter.counterData.OnCounterUpdate += OnCounterUpdate;
                _taskCounters.Add(counterType, counter);
                _taskSystemData.counters.Add(counter.counterData);
            }
            return counter;
        }

        private void OnCounterUpdate(CounterType counterType, int count, int param, bool isAdd)
        {
            OnTaskCounterRefresh?.Invoke(counterType, param);
        }
    }
}
