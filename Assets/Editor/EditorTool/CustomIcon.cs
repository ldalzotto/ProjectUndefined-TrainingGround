using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;
using ConfigurationEditor;

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
                Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/IconTextures/Engrenage.png");
                var configurationTypes =TypeHelper.GetAllTypeAssignableFrom(typeof(ConfigurationSerialization<,>));
                foreach(var configurationType in configurationTypes)
                {
                    var configuration = AssetFinder.SafeSingleAssetFind<Object>("t:" + configurationType.Name);
                    var editorGUIUtilityType = typeof(EditorGUIUtility);
                    var bindingFlags = BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic;
                    var args = new object[] { configuration, icon };
                    editorGUIUtilityType.InvokeMember("SetIconForObject", bindingFlags, null, null, args);
                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(configuration));
                }
            }
        }
    }
}

