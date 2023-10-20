using System;
using System.Collections.Generic;
using UnityEngine;

namespace SkierFramework
{
    /// <summary>
    /// 跟实际业务绑定
    /// </summary>
    public enum TaskType
    {
        None = 0,
        [InspectorName("主线任务")]
        Main,
        [InspectorName("成就")]
        Achievement,
        [InspectorName("每日任务")]
        Daily,
        [InspectorName("每周任务")]
        Weekly,
        [InspectorName("每日活跃任务")]
        DailyActive,
        [InspectorName("每周活跃任务")]
        WeeklyActive,
        // 扩展..
    }

}
