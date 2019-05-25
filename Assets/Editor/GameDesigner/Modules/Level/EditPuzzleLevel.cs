using UnityEngine;
using System.Collections;
using RTPuzzle;
using System;
using CoreGame;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public class EditPuzzleLevel : EditScriptableObjectModule<LevelManager>
    {
        private LevelConfiguration levelConfiguration;

        protected override Func<LevelManager, ScriptableObject> scriptableObjectResolver
        {
            get
            {
                return (LevelManager LevelManager) =>
                {
                    return levelConfiguration.ConfigurationInherentData[LevelManager.LevelID];
                };
            }
        }

        public override void OnEnabled()
        {
            levelConfiguration = AssetFinder.SafeSingleAssetFind<LevelConfiguration>("t:" + typeof(LevelConfiguration));
        }
    }
}