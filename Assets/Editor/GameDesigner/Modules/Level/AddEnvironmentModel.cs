using CoreGame;
using System;
using UnityEngine;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public class AddEnvironmentModel : AddPrefabModule<GameObject>
    {
        protected override Func<GameObject> ParentGameObject
        {
            get
            {
                return () => this.currentSelectedObjet.FindChildObjectRecursively("Environment");
            }
        }

        protected override bool IsAbleToAdd()
        {
            return this.currentSelectedObjet != null && this.currentSelectedObjet.GetComponent<LevelChunkType>() != null;
        }
    }
}