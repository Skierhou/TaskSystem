using System;
using System.Collections.Generic;
using UnityEngine;

namespace SkierFramework
{
    /// <summary>
    /// 没啥实际效果，主要为测试使用
    /// </summary>
    public class ConfigManager
    {
        public static ConfigManager Instance = new ConfigManager();

        public Dictionary<Type, Dictionary<int, IConfig>> configs;
        private ConfigManager()
        {
            configs = new Dictionary<Type, Dictionary<int, IConfig>>();
            Dictionary<int, IConfig> taskConfigs = new Dictionary<int, IConfig>();
            configs.Add(typeof(TaskConfig), taskConfigs);

            for (int i = 1; i <= 10; i++)
            {
                taskConfigs.Add(i, new TaskConfig
                {
                    id = i,
                    name = "任务" + i,
                    taskType = TaskType.Main,
                    isDefaultOpen = true,
                    taskConditions = new List<TaskCondition>
                    {
                        new TaskCondition{
                            counterType = CounterType.ItemGet,
                            param = i,
                            maxCount = i * 10,
                            taskRecordType = TaskRecordType.Incremental
                        },
                        new TaskCondition{
                            counterType = CounterType.ItemGet,
                            param = i + 1,
                            maxCount = i * 10,
                            taskRecordType = TaskRecordType.Accumulate
                        },
                    }
                });
            }
        }
        public T GetConfig<T>(int id) where T : IConfig
        {
            if (configs != null
                && configs.TryGetValue(typeof(T), out var map)
                && map.TryGetValue(id, out var config))
            {
                return (T)config;
            }
            return default(T);
        }
        public Dictionary<int, IConfig> GetAll<T>() where T : IConfig
        {
            if (configs != null
                && configs.TryGetValue(typeof(T), out var map))
            {
                return map;
            }
            return default;
        }
    }
}
