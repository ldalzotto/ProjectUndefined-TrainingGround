using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TimelineGraphsYamlSerialization : EditorWindow
{
    [MenuItem("Timeline/YAML Serialization")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(TimelineGraphsYamlSerialization));
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
                    initialzers.Add(new TimelineSerialized(timelineInitializerInstance.InitialNodes, timelineInitializerInstance.TimelineId.ToString()));
                }
            }
            var json = JsonConvert.SerializeObject(initialzers);
            Debug.Log(json);
        }
        EditorGUILayout.EndVertical();
    }

    class TimelineSerialized
    {
        public List<TimelineNode> nodes { get; }
        public string timelineId { get; }

        public TimelineSerialized(List<TimelineNode> nodes, string timelineId)
        {
            this.nodes = nodes;
            this.timelineId = timelineId;
        }
    }
}
