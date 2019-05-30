using UnityEngine;
using System.Collections;
using UnityEditor;
using AdventureGame;
using System.IO;
using System;
using System.Linq;
using UnityEditorInternal;

namespace Editor_GameCustomEditors
{
    [CustomEditor(typeof(AContextActionInherentDataChain))]
    public class AContextActionInherentDataChainCustomEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            this.DoContextActionInherentDataCreation();
            this.reorderableList.DoLayoutList();
            this.DoDisplaySelected();
        }

        private ReorderableList reorderableList;


        private Type[] allowedContextActionInherentDataToCreate;
        private int contextActionInherentDataSelectedIndex;

        private int lastFrameReorderableListSelectedIndex = -1;
        private Editor selectedActionEditor;

        private void DoContextActionInherentDataCreation()
        {
            var AContextActionInherentDataChain = (AContextActionInherentDataChain)target;

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+", EditorStyles.miniButton, GUILayout.Width(25f)))
            {
                AContextActionInherentDataChain.ContextActionChain.Add(this.Create());
            }
            this.contextActionInherentDataSelectedIndex = EditorGUILayout.Popup(this.contextActionInherentDataSelectedIndex, this.allowedContextActionInherentDataToCreate.ToList().ConvertAll(t => t.Name).ToArray());

            EditorGUILayout.EndHorizontal();
        }

        private void OnEnable()
        {
            var AContextActionInherentDataChain = (AContextActionInherentDataChain)target;
            this.allowedContextActionInherentDataToCreate = TypeHelper.GetAllTypeAssignableFrom(typeof(AContextActionInherentData));

            //sync assets
            var assets = AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(target)).ToList().ConvertAll(o => (AContextActionInherentData)o);
            bool deleted = false;
            for (int i = 0; i < assets.Count; i++)
            {
                if (!AContextActionInherentDataChain.ContextActionChain.Contains(assets[i]))
                {
                    UnityEngine.Object.DestroyImmediate(assets[i], true);
                    deleted = true;
                }
            }
            if (deleted)
            {
                EditorUtility.SetDirty(AContextActionInherentDataChain);
            }
            if (AContextActionInherentDataChain.ContextActionChain == null)
            {
                AContextActionInherentDataChain.ContextActionChain = new System.Collections.Generic.List<AContextActionInherentData>();
            }
            this.reorderableList = new ReorderableList(AContextActionInherentDataChain.ContextActionChain, typeof(AContextActionInherentData));
            this.reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                rect.height -= 5;
                EditorGUI.ObjectField(rect, AContextActionInherentDataChain.ContextActionChain[index], typeof(AContextActionInherentData), false);
            };

        }

        private AContextActionInherentData Create()
        {
            var createdContextAction = (AContextActionInherentData)ScriptableObject.CreateInstance(this.allowedContextActionInherentDataToCreate[contextActionInherentDataSelectedIndex]);
            AssetDatabase.AddObjectToAsset(createdContextAction, AssetDatabase.GetAssetPath(target));
            return createdContextAction;
        }

        private void DoDisplaySelected()
        {
            var AContextActionInherentDataChain = (AContextActionInherentDataChain)target;
            if (this.lastFrameReorderableListSelectedIndex != this.reorderableList.index)
            {
                if (this.reorderableList.index == -1)
                {
                    this.selectedActionEditor = null;
                }
                else
                {
                    this.selectedActionEditor = Editor.CreateEditor(AContextActionInherentDataChain.ContextActionChain[this.reorderableList.index]);
                }
            }

            if (this.selectedActionEditor != null)
            {
                this.selectedActionEditor.OnInspectorGUI();
            }


            this.lastFrameReorderableListSelectedIndex = this.reorderableList.index;
        }
    }
}