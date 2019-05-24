using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

namespace Editor_GameDesigner
{
    public abstract class AddPrefabModule<T> : IGameDesignerModule where T : UnityEngine.Object
    {

        private T prefabToAdd;
        public void GUITick()
        {
            this.prefabToAdd = (T)EditorGUILayout.ObjectField(this.prefabToAdd, typeof(T), false);
            if (GUILayout.Button("ADD TO SCENE"))
            {
                if (this.prefabToAdd != null)
                {
                    PrefabUtility.InstantiatePrefab(this.prefabToAdd, this.ParentGameObject.Invoke().transform);
                }
            }
        }

        protected abstract Func<GameObject> ParentGameObject { get; }

        public void OnDisabled()
        {
        }

        public void OnEnabled()
        {
        }
    }
}