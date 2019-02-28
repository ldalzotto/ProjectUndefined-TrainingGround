#if UNITY_EDITOR

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using timeline.serialized;
using UnityEditor;
using UnityEngine;

public class TimelineGraphsJSONSerialization : EditorWindow
{
    [MenuItem("Timeline/JSON Serialization")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(TimelineGraphsJSONSerialization));
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        if (GUILayout.Button("Generate JSON"))
        {
            var initialzers = new List<TimelineSerialized>();
            var types = typeof(TimelineInitializer).Assembly.GetTypes();
            for (var i = 0; i < types.Length; i++)
            {
                var type = types[i];
                if (type.IsSubclassOf(typeof(TimelineInitializer)))
                {
                    var timelineInitializerInstance = (TimelineInitializer)Activator.CreateInstance(type);
                    var serailizedNodes = timelineInitializerInstance.InitialNodes.ConvertAll(n => n.Map2Serialized());
                    initialzers.Add(new TimelineSerialized(timelineInitializerInstance.TimelineId.ToString(), serailizedNodes));
                }
            }
            var json = JsonConvert.SerializeObject(initialzers);
            Debug.Log(json);
        }
        EditorGUILayout.EndVertical();
    }
}

#endif