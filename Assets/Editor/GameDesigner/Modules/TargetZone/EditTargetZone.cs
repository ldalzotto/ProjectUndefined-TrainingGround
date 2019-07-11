using UnityEngine;
using System.Collections;
using RTPuzzle;
using System;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public class EditTargetZone : EditScriptableObjectModule<TargetZoneObjectModule>
    {
        private TargetZonesConfiguration TargetZonesConfiguration;

        protected override Func<TargetZoneObjectModule, ScriptableObject> scriptableObjectResolver
        {
            get
            {
                return (TargetZoneObjectModule TargetZone) =>
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
                this.TargetZonesConfiguration = AssetFinder.SafeSingleAssetFind<TargetZonesConfiguration>("t:" + typeof(TargetZonesConfiguration));
            }
        }
    }
}