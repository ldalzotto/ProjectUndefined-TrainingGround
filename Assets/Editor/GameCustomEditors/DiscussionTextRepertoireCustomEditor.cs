using AdventureGame;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Editor_GameCustomEditors
{
    [CustomEditor(typeof(DiscussionTextRepertoire))]
    public class DiscussionTextRepertoireCustomEditor : Editor
    {
        private TextDictionaryGUI<DisucssionSentenceTextId> SentecesGUI;

        public override void OnInspectorGUI()
        {
            DiscussionTextRepertoire DiscussionTestRepertoire = (DiscussionTextRepertoire)target;

            EditorGUI.BeginChangeCheck();
            this.SentecesGUI.GUiTick();
            EditorGUILayout.Separator();
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(DiscussionTestRepertoire);
            }

        }

        private void OnEnable()
        {
            DiscussionTextRepertoire DiscussionTestRepertoire = (DiscussionTextRepertoire)target;
            this.SentecesGUI = new TextDictionaryGUI<DisucssionSentenceTextId>(ref DiscussionTestRepertoire.SentencesText, "Discussion Sentences : ");
        }
    }

    class TextDictionaryGUI<T> where T : Enum
    {
        private Dictionary<T, string> values;
        private List<T> alphabeticalOrderedEnums;
        private string title;
        private Vector2 scrollPosition = Vector2.zero;

        public TextDictionaryGUI(ref Dictionary<T, string> initialValues, string title)
        {
            this.values = initialValues;
            this.OnValueChanged();
            this.title = title;
        }

        public void GUiTick()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(this.title);
            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("R", EditorStyles.miniButton))
            {
                bool updated = false;
                foreach (var enumValue in Enum.GetValues(typeof(T)))
                {
                    if (!values.Keys.ToList().Contains((T)enumValue))
                    {
                        this.values.Add((T)enumValue, string.Empty);
                        updated = true;
                    }
                }

                var involvedEnumValues = ((T[])Enum.GetValues(typeof(T))).ToList();

                foreach (var alreadyPresentValue in this.values.Keys.ToList())
                {
                    if (!involvedEnumValues.Contains(alreadyPresentValue))
                    {
                        this.values.Remove(alreadyPresentValue);
                        updated = true;
                    }
                }

                if (updated)
                {
                    this.OnValueChanged();
                }
            }
            EditorGUILayout.EndHorizontal();

            this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition);
            foreach (var alphabeticalOrderedEnum in alphabeticalOrderedEnums)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(alphabeticalOrderedEnum.ToString());
                values[alphabeticalOrderedEnum] = EditorGUILayout.TextField(values[alphabeticalOrderedEnum]);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();
        }

        private void OnValueChanged()
        {
            this.alphabeticalOrderedEnums = this.values.Keys.ToList();
            this.alphabeticalOrderedEnums.Sort((T e1, T e2) =>
            {
                return e1.ToString().CompareTo(e2.ToString());
            });
        }
    }
}