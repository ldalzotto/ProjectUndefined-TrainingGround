using GameConfigurationID;
using RTPuzzle;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

public abstract class ByEnumPropertyCustomEditor<K, V> : Editor where K : Enum
{
    private EnumSearchGUIWindow windowInstance;
    private K selectedEnum;
    private Rect dropdownRect;
    private bool foldout = false;

    public override void OnInspectorGUI()
    {
        ByEnumProperty<K, V> projectileEscapeRange = (ByEnumProperty<K, V>)target;
        this.Init(projectileEscapeRange);

        this.foldout = EditorGUILayout.Foldout(this.foldout, target.name, true);
        if (this.foldout)
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("+", EditorStyles.miniButton, GUILayout.Width(20f)))
            {
                projectileEscapeRange.Values[(K)Enum.Parse(typeof(K), this.selectedEnum.ToString())] = default(V);
            }

            if (EditorGUILayout.DropdownButton(new GUIContent(this.selectedEnum.ToString()), FocusType.Passive))
            {
                this.DisplayEnumChoice();
            }
            if (Event.current.type == EventType.Repaint)
            {
                this.dropdownRect = GUILayoutUtility.GetLastRect();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginVertical();
            foreach (var key in projectileEscapeRange.Values.Keys.ToList())
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.EnumPopup(key);
                if (typeof(V) == typeof(float))
                {
                    projectileEscapeRange.Values[key] = (V)((object)EditorGUILayout.FloatField(float.Parse(projectileEscapeRange.Values[key].ToString())));
                }
                if (GUILayout.Button("-", EditorStyles.miniButton, GUILayout.Width(20f)))
                {
                    projectileEscapeRange.Values.Remove(key);
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }
        }

    }

    private void Init(ByEnumProperty<K, V> byEnumProperty)
    {
        if (byEnumProperty.Values == null)
        {
            byEnumProperty.Values = new System.Collections.Generic.Dictionary<K, V>();
        }
    }

    private void DisplayEnumChoice()
    {
        this.windowInstance = EditorWindow.CreateInstance<EnumSearchGUIWindow>();
        this.windowInstance.Init(((K[])Enum.GetValues(typeof(K)))[0], (e) =>
        {
            selectedEnum = (K)e;
        });
        var windowRect = new Rect(GUIUtility.GUIToScreenPoint(dropdownRect.position), dropdownRect.size);
        windowInstance.ShowAsDropDown(windowRect, new Vector2(dropdownRect.size.x, 200f));
    }

    private void HideEnumChoice()
    {
        this.windowInstance = null;
    }
}

[CustomEditor(typeof(ProjectileEscapeRange))]
public class ProjectileEscapeRangeCustomEditor : ByEnumPropertyCustomEditor<LaunchProjectileId, float> { }
[CustomEditor(typeof(ProjectileEscapeSemiAngle))]
public class ProjectileEscapeSemiAngleCustomEditor : ByEnumPropertyCustomEditor<LaunchProjectileId, float> { }
[CustomEditor(typeof(RepelableObjectDistance))]
public class RepelableObjectDistanceCustomEditor : ByEnumPropertyCustomEditor<LaunchProjectileId, float> { }

