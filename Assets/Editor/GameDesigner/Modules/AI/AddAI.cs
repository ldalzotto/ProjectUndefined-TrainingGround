using UnityEngine;
using System.Collections;
using RTPuzzle;
using UnityEditor;
using CoreGame;
using System;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public class AddAI : AddPrefabModule<AIObjectType>
    {
        protected override Func<GameObject> ParentGameObject
        {
            get
            {
                return () => GameObject.FindObjectOfType<LevelManager>().gameObject.FindChildObjectRecursively("AI");
            }
        }

    }

}
