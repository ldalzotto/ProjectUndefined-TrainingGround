using UnityEngine;
using System.Collections;
using RTPuzzle;
using UnityEditor;
using CoreGame;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public class AddAI : IGameDesignerModule
    {

        private NPCAIManager aiToAdd;
        public void GUITick()
        {
            this.aiToAdd = (NPCAIManager)EditorGUILayout.ObjectField(this.aiToAdd, typeof(NPCAIManager), false);
            if (GUILayout.Button("ADD TO SCENE"))
            {
                if(this.aiToAdd != null)
                {
                    PrefabUtility.InstantiatePrefab(this.aiToAdd, GameObject.FindObjectOfType<LevelManager>().gameObject.FindChildObjectRecursively("AI").transform);
                }
            }
        }

        public void OnDisabled()
        {
        }

        public void OnEnabled()
        {
        }
    }

}
