using System;
using LevelManagement;
using UnityEngine;

namespace Editor_GameDesigner
{
    [Serializable]
    public class AddEnvironmentModel : AddPrefabModule<GameObject>
    {
        protected override Func<GameObject> ParentGameObject
        {
            get { return () => this.currentSelectedObjet.FindChildObjectRecursively("Environment"); }
        }

        protected override bool IsAbleToAdd()
        {
            return this.currentSelectedObjet != null && this.currentSelectedObjet.GetComponent<LevelChunkType>() != null;
        }
    }
}