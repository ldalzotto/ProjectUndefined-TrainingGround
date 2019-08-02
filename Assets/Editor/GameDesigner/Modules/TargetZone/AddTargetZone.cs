using UnityEngine;
using System.Collections;
using CoreGame;
using RTPuzzle;
using System;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public class AddTargetZone : AddPrefabModule<TargetZoneModule>
    {
        protected override Func<GameObject> ParentGameObject
        {
            get
            {
                return () => GameObject.FindObjectOfType<LevelManager>().gameObject.FindChildObjectRecursively("TargetZones");
            }
        }
    }
}