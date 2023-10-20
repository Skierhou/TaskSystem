using System;
using System.Collections.Generic;

namespace SkierFramework
{
    /// <summary>
    /// 存档数据
    /// </summary>
    [System.Serializable]
    public class TaskSystemData
    {
        public List<Counter> counters = new List<Counter>();
        public List<TaskData> taskDatas = new List<TaskData>();
    }
}
