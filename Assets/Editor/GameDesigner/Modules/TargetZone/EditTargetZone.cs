using UnityEngine;
using System.Collections;
using RTPuzzle;
using System;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public class EditTargetZone : EditScriptableObjectModule<TargetZone>
    {
        private TargetZonesConfiguration TargetZonesConfiguration;

        protected override Func<TargetZone, ScriptableObject> scriptableObjectResolver
        {
            get
            {
                return (TargetZone TargetZone) =>
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