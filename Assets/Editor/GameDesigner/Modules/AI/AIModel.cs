﻿using UnityEngine;
using System.Collections;
using UnityEditor;
using RTPuzzle;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public class AIModel : IGameDesignerModule
    {
        public GameObject AIModelObject;

        public void GUITick()
        {
           
            var currentSelectedObj = GameDesignerHelper.GetCurrentSceneSelectedObject();

            this.AIModelObject = (GameObject)EditorGUILayout.ObjectField("AI new Model : ", this.AIModelObject, typeof(GameObject), false);

            EditorGUI.BeginDisabledGroup(this.AIModelObject == null || currentSelectedObj == null || currentSelectedObj.GetComponent<NPCAIManager>() == null);
            if (GUILayout.Button("SET AI MODEL"))
            {
                PrefabUtility.InstantiatePrefab(this.AIModelObject, currentSelectedObj.FindChildObjectRecursively("Model").transform);
                
            }
            EditorGUI.EndDisabledGroup();

        }

        public void OnDisabled()
        {
        }

        public void OnEnabled()
        {
        }
    }
}