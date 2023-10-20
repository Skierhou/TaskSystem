#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Progress;

namespace SkierFramework
{
    class TaskWindow : EditorWindow
    {
        private const string WindowName = "任务调试";

        [MenuItem("Tools/系统/"+ WindowName)]
        public static void OpenWindow()
        {
            var window = GetWindow<TaskWindow>(WindowName);
            if (window == null)
                window = CreateWindow<TaskWindow>(WindowName);
            window.name = WindowName;
            window.Focus();
        }

        private Vector2 _scroll;
        private Vector2 _scroll2;
        private Dictionary<string, bool> _foldoutValues = new Dictionary<string, bool>();
        private Dictionary<string, int> _values = new Dictionary<string, int>();
        private Dictionary<int, int> tempParams = new Dictionary<int, int>();

        private void OnGUI()
        {
            if (TaskSystem.Instance == null)
            {
                EditorGUILayout.HelpBox("未找到任务系统实例！", MessageType.Error);
                return;
            }
            EditorGUILayout.BeginHorizontal(GUILayout.Width(position.width), GUILayout.Height(30));
            EditorGUILayout.HelpBox("计数器中的更新，与强制的区别是：更新时计数会取当前和输入中的最大值，而强制则为强制设置，加Key相当于：怪物Id,道具Id等，没有则使用0", MessageType.Info);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal(GUILayout.Width(position.width), GUILayout.Height(30));
            EditorGUILayout.LabelField("计数器", EditorStyles.helpBox, GUILayout.Width(position.width * 0.5f));
            EditorGUILayout.LabelField("任务", EditorStyles.helpBox, GUILayout.Width(position.width * 0.5f));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal(GUILayout.Width(position.width), GUILayout.Height(position.height - 60));
            {
                _scroll = EditorGUILayout.BeginScrollView(_scroll, GUILayout.Width(position.width * 0.5f - 10));
                {
                    DrawCounter();
                }
                EditorGUILayout.EndScrollView();
                GUILayout.Space(20);
                _scroll2 = EditorGUILayout.BeginScrollView(_scroll2, GUILayout.Width(position.width * 0.5f - 10));
                {
                    DrawTask();
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawCounter()
        {
            GUILayout.BeginVertical();
            var values = Enum.GetValues(typeof(CounterType));
            foreach (CounterType type in values)
            {
                TaskSystem.Instance.GetCounter(type);
            }
            foreach (var counter in TaskSystem.Instance.TaskCounters.Values)
            {
                string name = counter.counterType.ToString();
                if (!_foldoutValues.ContainsKey(name))
                {
                    _foldoutValues[name] = false;
                }

                GUILayout.BeginHorizontal("box", GUILayout.Width(position.width * 0.5f));

                GUILayout.BeginVertical("box", GUILayout.Width(30));
                {
                    _foldoutValues[name] = GUILayout.Toggle(_foldoutValues[name], "");
                }
                GUILayout.EndVertical();
                GUILayout.BeginVertical("box", GUILayout.Width(position.width * 0.5f - 170f));
                {
                    if (EditorGUILayout.DropdownButton(new GUIContent($"{name}  Count:{counter.counterData.accumulate.Count}"), FocusType.Keyboard))
                    {
                        _foldoutValues[name] = !_foldoutValues[name];
                    }
                }
                GUILayout.EndVertical();

                _values.TryGetValue(name, out var key);
                key = EditorGUILayout.IntField(key, GUILayout.Width(50));
                _values[name] = key;
                if (GUILayout.Button("加Key", GUILayout.Width(50)))
                {
                    if(!counter.counterData.accumulate.ContainsKey(key))
                        counter.counterData.accumulate.Add(key, 0);
                }
                GUILayout.Space(20);
                GUILayout.EndHorizontal();

                if (_foldoutValues[name])
                {
                    tempParams.Clear();
                    foreach (var item in counter.accumulate)
                    {
                        tempParams.Add(item.Key, item.Value);
                    }
                    foreach (var item in tempParams)
                    {
                        GUILayout.BeginHorizontal("box", GUILayout.Width(position.width * 0.5f));
                        GUILayout.Space(20);

                        string name2 = $"{name}.{item.Key}";
                        GUILayout.Label($"{name2} = {item.Value}");
                        if (!_values.TryGetValue(name2, out var count))
                        {
                            count = 1;
                        }
                        count = EditorGUILayout.IntField(count, GUILayout.Width(50));
                        if (GUILayout.Button("递增", GUILayout.Width(50)))
                        {
                            counter.AddCount(count, item.Key);
                        }
                        if (GUILayout.Button("更新", GUILayout.Width(50)))
                        {
                            counter.UpdateCount(count, item.Key, false);
                        }
                        if (GUILayout.Button("强制", GUILayout.Width(50)))
                        {
                            counter.UpdateCount(count, item.Key, true);
                        }
                        GUILayout.Space(20);
                        _values[name2] = count;

                        GUILayout.EndHorizontal();
                    }
                }
            }
            GUILayout.EndVertical();
        }

        private void DrawTask()
        {
            GUILayout.BeginVertical();
            foreach (var task in TaskSystem.Instance.Tasks.Values)
            {
                GUILayout.BeginHorizontal("box", GUILayout.Width(position.width * 0.5f));

                string name = $"{task.taskId}-{task.TaskConfig.name}";
                if (!_foldoutValues.ContainsKey(name))
                {
                    _foldoutValues[name] = false;
                }
                GUILayout.BeginVertical("box", GUILayout.Width(30));
                {
                    _foldoutValues[name] = GUILayout.Toggle(_foldoutValues[name], "");
                }
                GUILayout.EndVertical();

                Color defaultColor = GUI.color;
                GUILayout.BeginVertical("box", GUILayout.Width(position.width * 0.5f - 200f));
                {
                    if (task.IsFinish)
                        GUI.color = Color.green;
                    if (EditorGUILayout.DropdownButton(new GUIContent($"{name}"), FocusType.Keyboard))
                    {
                        _foldoutValues[name] = !_foldoutValues[name];
                    }
                    GUI.color = defaultColor;
                }
                GUILayout.EndVertical();

                string status = task.isGet ? "已领取奖励" : (task.IsFinish ? "完成未领取" : "未完成");
                GUILayout.Label($"状态：{status}", GUILayout.Width(150));

                GUILayout.Space(20);
                GUILayout.EndHorizontal();

                if (_foldoutValues[name])
                {
                    for (int i = 0; i < task.TaskConfig.taskConditions.Count; i++)
                    {
                        var condition = task.TaskConfig.taskConditions[i];
                        GUILayout.BeginHorizontal("box", GUILayout.Width(position.width * 0.5f));
                        GUILayout.Space(20);
                        if (task.curCounts[i] >= condition.maxCount) 
                            GUI.color = Color.green;
                        GUILayout.Label($"{name}    {condition.counterType}.{condition.param} = {task.curCounts[i]}/{condition.maxCount}");
                        GUI.color = defaultColor;
                        GUILayout.EndHorizontal();
                    }
                }
            }
            GUILayout.EndVertical();
        }
    }
}
#endif
