using UnityEngine;
using System.Collections;
using CoreGame;
using UnityEditor;

namespace Editor_GameCustomEditors
{
    [ExecuteInEditMode]
    [CustomEditor(typeof(TimelineInitializerScriptableObject), true)]
    public class TimelineInitializerScriptableObjectCustomEditor : AbstractConfigurationDataCustomEditor<TimelineConfiguration>
    {
    }
}