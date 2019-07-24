using AdventureGame;
using GameConfigurationID;
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
        private RegexTextFinder SentenceSearch;
        private TextDictionaryGUI<DisucssionSentenceTextId> SentecesGUI;

        public override void OnInspectorGUI()
        {
            DiscussionTextRepertoire DiscussionTestRepertoire = (DiscussionTextRepertoire)target;

            this.SentenceSearch.GUITick();

            EditorGUI.BeginChangeCheck();
            this.SentecesGUI.GUiTick(ref this.SentenceSearch);
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
            this.SentenceSearch = new RegexTextFinder();
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

        public void GUiTick(ref RegexTextFinder SentenceSearch)
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
                if(SentenceSearch.IsMatchingWith(alphabeticalOrderedEnum.ToString()))
                {
                    EditorGUILayout.LabelField(alphabeticalOrderedEnum.ToString());
                    values[alphabeticalOrderedEnum] = EditorGUILayout.TextArea(values[alphabeticalOrderedEnum]);
                }
       
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