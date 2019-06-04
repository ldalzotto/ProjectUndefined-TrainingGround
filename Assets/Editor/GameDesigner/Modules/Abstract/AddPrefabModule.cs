using System;
using UnityEditor;
using UnityEngine;

namespace Editor_GameDesigner
{
    public abstract class AddPrefabModule<T> : IGameDesignerModule where T : UnityEngine.Object
    {

        protected GameObject currentSelectedObjet;
        private T prefabToAdd;
        public void GUITick()
        {
            this.currentSelectedObjet = GameDesignerHelper.GetCurrentSceneSelectedObject();
            this.prefabToAdd = (T)EditorGUILayout.ObjectField(this.prefabToAdd, typeof(T), false);
            EditorGUI.BeginDisabledGroup(!this.IsAbleToAdd());
            if (GUILayout.Button("ADD TO SCENE"))
            {
                if (this.prefabToAdd != null)
                {
                    PrefabUtility.InstantiatePrefab(this.prefabToAdd, this.ParentGameObject.Invoke().transform);
                }
            }
            EditorGUI.EndDisabledGroup();
        }

        protected virtual bool IsAbleToAdd() { return true; }
        protected abstract Func<GameObject> ParentGameObject { get; }

        public void OnDisabled()
        {
        }

        public void OnEnabled()
        {
        }
    }
}