using AdventureGame;
using ConfigurationEditor;
using Editor_GameDesigner;
using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CustomEnum))]
public class CustomEnumPropertyDrawer : PropertyDrawer
{
    private EnumSearchGUIWindow windowInstance;

    private int lineNB = 0;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        CustomEnum searchableEnum = (CustomEnum)attribute;
        lineNB = 0;
        if (searchableEnum.IsCreateable)
        {
            lineNB += 1;
        }
        if (searchableEnum.IsSearchable)
        {
            lineNB += 1;
        }
        if (searchableEnum.ChoosedOpenRepertoire)
        {
            lineNB += 1;
        }
        return EditorGUI.GetPropertyHeight(property) * lineNB;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        CustomEnum searchableEnum = (CustomEnum)attribute;
        if (property.propertyType == SerializedPropertyType.Enum)
        {
            var targetEnum = SerializableObjectHelper.GetBaseProperty<Enum>(property);
            int currentLineNB = 0;

            if (searchableEnum.IsSearchable)
            {
                Rect lineRect = this.GetRectFromLineNb(currentLineNB, position);

                var labelFieldRect = new Rect(lineRect.x, lineRect.y, lineRect.width / 2, lineRect.height);
                EditorGUI.LabelField(labelFieldRect, label);
                var enumPopupRect = new Rect(lineRect.x + lineRect.width / 2, lineRect.y, lineRect.width / 2, lineRect.height);
                if (EditorGUI.DropdownButton(enumPopupRect, new GUIContent(targetEnum.ToString()), FocusType.Keyboard))
                {
                    if (windowInstance == null)
                    {
                        windowInstance = EditorWindow.CreateInstance<EnumSearchGUIWindow>();
                        windowInstance.Init(targetEnum, (newSelectedEnum) =>
                        {
                            property.longValue = (int)Convert.ChangeType(newSelectedEnum, newSelectedEnum.GetTypeCode());
                            property.serializedObject.ApplyModifiedProperties();
                            property.serializedObject.Update();
                            EditorUtility.SetDirty(property.serializedObject.targetObject);
                        });
                    }

                    var windowRect = new Rect(GUIUtility.GUIToScreenPoint(enumPopupRect.position), new Vector2(0, enumPopupRect.height));
                    windowInstance.ShowAsDropDown(windowRect, new Vector2(enumPopupRect.width, 500));

                }

                currentLineNB += 1;
            }

            if (searchableEnum.IsCreateable)
            {
                Rect lineRect = this.GetRectFromLineNb(currentLineNB, position);

                if (GUI.Button(lineRect, "Create ID"))
                {
                    EnumIDGeneration.Init(targetEnum.GetType());
                }
                currentLineNB += 1;
            }

            if (searchableEnum.ChoosedOpenRepertoire)
            {
                Rect lineRect = this.GetRectFromLineNb(currentLineNB, position);

                if (GUI.Button(lineRect, "Open Repertoire"))
                {
                    ConfigurationInspector.OpenConfigurationEditor(typeof(DiscussionRepertoireConfigurationModule));
                }
                currentLineNB += 1;
            }

        }
    }

    private Rect GetRectFromLineNb(int lineNb, Rect initialPosition)
    {
        Rect lineRect = new Rect(initialPosition);
        lineRect.y += ((initialPosition.height / this.lineNB) * (lineNb));
        lineRect.height = EditorGUIUtility.singleLineHeight;
        return lineRect;
    }
}