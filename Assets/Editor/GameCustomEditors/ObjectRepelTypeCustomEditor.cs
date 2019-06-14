using Editor_GameDesigner;
using RTPuzzle;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor_GameCustomEditors
{
    [ExecuteInEditMode]
    [CustomEditor(typeof(ObjectRepelType))]
    public class ObjectRepelTypeCustomEditor : AbstractGameCustomEditorWithLiveSelection<ObjectRepelType, ObjectRepelTypeCustomEditorContext, RepelableObjectsConfiguration, EditRepelableObject>
    {
        private void OnEnable()
        {
            if (target != null)
            {
                var repelableObjectsConfiguration = AssetFinder.SafeSingleAssetFind<RepelableObjectsConfiguration>("t:" + typeof(RepelableObjectsConfiguration));
                this.context = new ObjectRepelTypeCustomEditorContext();
                this.context.RepelableObjectsInherentData = repelableObjectsConfiguration.ConfigurationInherentData[((ObjectRepelType)this.target).RepelableObjectID];

                this.drawModules = new List<GUIDrawModule<ObjectRepelType, ObjectRepelTypeCustomEditorContext>>() {
                    new ObjectRepelDistanceComponent()
                };
            }
        }

    }

    public class ObjectRepelTypeCustomEditorContext
    {
        public RepelableObjectsInherentData RepelableObjectsInherentData;
    }

    public class ObjectRepelDistanceComponent : IDPickGUIModule<ObjectRepelType, ObjectRepelTypeCustomEditorContext, LaunchProjectileId, float>
    {
        public override Func<ObjectRepelTypeCustomEditorContext, ByEnumProperty<LaunchProjectileId, float>> GetByEnumProperty
        {
            get
            {
                return (ObjectRepelTypeCustomEditorContext ObjectRepelTypeCustomEditorContext) => ObjectRepelTypeCustomEditorContext.RepelableObjectsInherentData.RepelableObjectDistance;
            }
        }

        public override void SceneGUI(ObjectRepelTypeCustomEditorContext context, ObjectRepelType target, LaunchProjectileId key)
        {
            Handles.color = Color.yellow;
            Handles.Label(target.transform.position + Vector3.up * context.RepelableObjectsInherentData.GetRepelableObjectDistance(key), nameof(context.RepelableObjectsInherentData.RepelableObjectDistance), MyEditorStyles.LabelYellow);
            Handles.DrawWireDisc(target.transform.position, Vector3.up, context.RepelableObjectsInherentData.GetRepelableObjectDistance(key));
        }
    }
}