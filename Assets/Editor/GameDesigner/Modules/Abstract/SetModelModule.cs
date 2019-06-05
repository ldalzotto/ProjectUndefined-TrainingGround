using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public abstract class SetModelModule<T> : IGameDesignerModule where T : UnityEngine.Object
    {
        public GameObject ModelObject;

        public void GUITick()
        {

            var currentSelectedObj = GameDesignerHelper.GetCurrentSceneSelectedObject();

            this.ModelObject = (GameObject)EditorGUILayout.ObjectField("New Model : ", this.ModelObject, typeof(GameObject), false);

            EditorGUI.BeginDisabledGroup(this.ModelObject == null || currentSelectedObj == null || currentSelectedObj.GetComponent<T>() == null);
            if (GUILayout.Button("SET MODEL"))
            {
                PrefabUtility.InstantiatePrefab(this.ModelObject, this.FindParent.Invoke(currentSelectedObj.GetComponent<T>()) /*currentSelectedObj.FindChildObjectRecursively("Model").transform*/);

            }
            EditorGUI.EndDisabledGroup();
        }

        protected abstract Func<T, Transform> FindParent { get; }

        public void OnDisabled()
        {
      
        }

        public void OnEnabled()
        {
           
        }
    }
}