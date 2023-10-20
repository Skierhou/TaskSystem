using System;
using System.Collections.Generic;
using UnityEngine;

namespace SkierFramework
{
    /// <summary>
    /// 计数器类型
    /// </summary>
    public enum CounterType
    {
        [InspectorName("道具属性变更")]
        ItemAttr,
        [InspectorName("道具获得")]
        ItemGet,
        [InspectorName("道具消耗")]
        ItemConsume,
        [InspectorName("道具数量")]
        ItemNum,
        [InspectorName("任务完成数量(对应TaskType)")]
        TaskFinish,
    }
}
