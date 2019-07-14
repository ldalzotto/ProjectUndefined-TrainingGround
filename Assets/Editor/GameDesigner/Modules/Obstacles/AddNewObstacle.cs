using CoreGame;
using RTPuzzle;
using System;
using UnityEngine;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public class AddNewObstacle : AddPrefabModule<SquareObstacle>
    {
        protected override Func<GameObject> ParentGameObject
        {
            get
            {
                return () =>
                {
                    var GroundEffectsManagerV2 = GameObject.FindObjectOfType<GroundEffectsManagerV2>();
                    if(GroundEffectsManagerV2 != null)
                    {
                        return GroundEffectsManagerV2.gameObject.FindChildObjectRecursively("PuzzleObstacles");
                    }
                    else
                    {
                        return null;
                    }
                };
            }
        }

        public override void GUITick(ref GameDesignerEditorProfile GameDesignerEditorProfile)
        {
            base.GUITick(ref GameDesignerEditorProfile);
            if (this.prefabToAdd == null)
            {
                this.prefabToAdd = AssetFinder.SafeSingleAssetFind<SquareObstacle>("BaseSquareObstaclePrefab");
            }
        }
    }
}