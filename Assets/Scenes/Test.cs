using SkierFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    TaskSystem taskSystem = new TaskSystem();

    void Start()
    {
        taskSystem.InitSystem();

        for (int i = 1; i <= 10; i++)
        {
            taskSystem.AddCount(CounterType.ItemGet, 1, i);
            taskSystem.AddCount(CounterType.ItemConsume, 1, i);
            taskSystem.AddCount(CounterType.ItemAttr, 1, i);
        }
    }

    private void OnDestroy()
    {
        taskSystem.ReleaseSystem();
    }
}
