using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;
using ConfigurationEditor;
using NodeGraph;

namespace Editor_EditorTool
{
    public class CustomIcon : EditorWindow
    {
        [MenuItem("EditorTool/CustomIcon")]
        public static void Init()
        {
            EditorWindow.GetWindow(typeof(CustomIcon));
        }

        private void OnGUI()
        {
            if (GUILayout.Button("REFRESH"))
            {
                Texture2D engrenageIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/IconTextures/Engrenage.png");
                Texture2D nodeIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/IconTextures/Node.jpg");
                var configurationTypes =TypeHelper.GetAllTypeAssignableFrom(typeof(ConfigurationSerialization<,>));
                foreach(var configurationType in configurationTypes)
                {
                    var configuration = AssetFinder.SafeSingleAssetFind<Object>("t:" + configurationType.Name);
                    SetInspectorIcon(engrenageIcon, configuration);
                }

                var nodeEditorProfileTypes = TypeHelper.GetAllTypeAssignableFrom(typeof(NodeEditorProfile));
                foreach(var nodeEditorProfileType in nodeEditorProfileTypes)
                {
                    var nodeGraphs = AssetFinder.SafeAssetFind<Object>("t:" + nodeEditorProfileType.Name);
                    foreach(var nodeGraph in nodeGraphs)
                    {
                        SetInspectorIcon(nodeIcon, nodeGraph);
                    }
                }
            }
        }

        private static void SetInspectorIcon(Texture2D icon, Object obj)
        {
            var editorGUIUtilityType = typeof(EditorGUIUtility);
            var bindingFlags = BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic;
            var args = new object[] { obj, icon };
            editorGUIUtilityType.InvokeMember("SetIconForObject", bindingFlags, null, null, args);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(obj));
        }
    }
}

