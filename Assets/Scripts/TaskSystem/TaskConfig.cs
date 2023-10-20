using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SkierFramework
{
    /// <summary>
    /// 任务记录方式
    /// </summary>
    public enum TaskRecordType
    {
        [InspectorName("累积型")]
        Accumulate,
        [InspectorName("递增型")]
        Incremental,
        [InspectorName("消耗型")]
        Consumable,
    }

    [System.Serializable]
    public class TaskCondition
    {
        [InspectorName("任务事件")]
        public CounterType counterType;
        [InspectorName("任务记录方式")]
        public TaskRecordType taskRecordType;
        [InspectorName("达成条件")]
        public int maxCount;
        [InspectorName("自定义参数")]
        public int param;
    }

    [System.Serializable]
    public class TaskConfig : IConfig
    {
        public int id;
        public int ID => id;

        [InspectorName("名称")]
        public string name;
        [InspectorName("任务描述")]
        public string desc;
        [InspectorName("任务类型")]
        public TaskType taskType;
        [InspectorName("任务条件")]
        public List<TaskCondition> taskConditions;
        [InspectorName("是否默认开启")]
        public bool isDefaultOpen;
        [InspectorName("后续开启的任务")]
        public List<int> nextIds;

        [InspectorName("任务奖励")]
        public int rewardId;
    }
}