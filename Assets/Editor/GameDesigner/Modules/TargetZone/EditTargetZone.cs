using UnityEngine;
using System.Collections;
using RTPuzzle;
using System;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public class EditTargetZone : EditScriptableObjectModule<TargetZoneModule>
    {
        private TargetZoneConfiguration TargetZonesConfiguration;

        protected override Func<TargetZoneModule, ScriptableObject> scriptableObjectResolver
        {
            get
            {
                return (TargetZoneModule TargetZone) =>
                {
                    if(this.TargetZonesConfiguration != null && TargetZonesConfiguration.ConfigurationInherentData.ContainsKey(TargetZone.TargetZoneID))
                    {
                        return TargetZonesConfiguration.ConfigurationInherentData[TargetZone.TargetZoneID];
                    }
                    return null;
                };
            }
        }

        public override void OnEnabled()
        {
            if (this.TargetZonesConfiguration == null)
            {
                this.TargetZonesConfiguration = AssetFinder.SafeSingleAssetFind<TargetZoneConfiguration>("t:" + typeof(TargetZoneConfiguration));
            }
        }
    }
}