using UnityEngine;
using System.Collections;
using CoreGame;
using UnityEditor;
using Editor_GameDesigner;

namespace Editor_GameCustomEditors
{
    [ExecuteInEditMode]
    [CustomEditor(typeof(TimelineInitializerScriptableObject), true)]
    public class TimelineInitializerScriptableObjectCustomEditor : AbstractConfigurationDataCustomEditor<TimelineConfigurationModule>
    {
    }
}