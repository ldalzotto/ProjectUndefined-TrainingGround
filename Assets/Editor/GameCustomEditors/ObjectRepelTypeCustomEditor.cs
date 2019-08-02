using Editor_GameDesigner;
using GameConfigurationID;
using RTPuzzle;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor_GameCustomEditors
{
    [ExecuteInEditMode]
    [CustomEditor(typeof(ObjectRepelModule))]
    public class ObjectRepelTypeCustomEditor : AbstractGameCustomEditorWithLiveSelection<ObjectRepelModule, ObjectRepelTypeCustomEditorContext, RepelableObjectsConfigurationModule, EditRepelableObject>
    {
        private void OnEnable()
        {
            if (target != null)
            {
                var repelableObjectsConfiguration = AssetFinder.SafeSingleAssetFind<ObjectRepelConfiguration>("t:" + typeof(ObjectRepelConfiguration));
                this.context = new ObjectRepelTypeCustomEditorContext();
                this.context.RepelableObjectsInherentData = repelableObjectsConfiguration.ConfigurationInherentData[((ObjectRepelModule)this.target).ObjectRepelID];

                this.drawModules = new List<GUIDrawModule<ObjectRepelModule, ObjectRepelTypeCustomEditorContext>>() {
                    new ObjectRepelDistanceComponent()
                };
            }
        }

    }

    public class ObjectRepelTypeCustomEditorContext
    {
        public ObjectRepelInherentData RepelableObjectsInherentData;
    }

    public class ObjectRepelDistanceComponent : IDPickGUIModule<ObjectRepelModule, ObjectRepelTypeCustomEditorContext, LaunchProjectileID, float>
    {
        public override Func<ObjectRepelTypeCustomEditorContext, ByEnumProperty<LaunchProjectileID, float>> GetByEnumProperty
        {
            get
            {
                return (ObjectRepelTypeCustomEditorContext ObjectRepelTypeCustomEditorContext) => ObjectRepelTypeCustomEditorContext.RepelableObjectsInherentData.RepelableObjectDistance;
            }
        }

        public override void SceneGUI(ObjectRepelTypeCustomEditorContext context, ObjectRepelModule target, LaunchProjectileID key)
        {
            Handles.color = Color.yellow;
            Handles.Label(target.transform.position + Vector3.up * context.RepelableObjectsInherentData.GetRepelableObjectDistance(key), nameof(context.RepelableObjectsInherentData.RepelableObjectDistance), MyEditorStyles.LabelYellow);
            Handles.DrawWireDisc(target.transform.position, Vector3.up, context.RepelableObjectsInherentData.GetRepelableObjectDistance(key));
        }
    }
}