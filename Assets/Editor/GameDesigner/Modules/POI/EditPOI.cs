using AdventureGame;
using System;
using UnityEngine;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public class EditPOI : EditScriptableObjectModule<PointOfInterestType>
    {
        private PointOfInterestConfiguration PointOfInterestConfiguration;
        protected override Func<PointOfInterestType, ScriptableObject> scriptableObjectResolver
        {
            get
            {
                return (PointOfInterestType PointOfInterestType) =>
                {
                    if (PointOfInterestConfiguration != null && PointOfInterestConfiguration.ConfigurationInherentData.ContainsKey(PointOfInterestType.PointOfInterestId))
                    {
                        return PointOfInterestConfiguration.ConfigurationInherentData[PointOfInterestType.PointOfInterestId];
                    }
                    return null;
                };
            }
        }

        public override void OnEnabled()
        {
            this.PointOfInterestConfiguration = AssetFinder.SafeSingleAssetFind<PointOfInterestConfiguration>("t:" + typeof(PointOfInterestConfiguration));
        }
    }
}