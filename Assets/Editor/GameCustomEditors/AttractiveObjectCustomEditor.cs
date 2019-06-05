using Editor_GameDesigner;
using RTPuzzle;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor_GameCustomEditors
{
    [ExecuteInEditMode]
    [CustomEditor(typeof(AttractiveObjectType))]
    public class AttractiveObjectCustomEditor : AbstractGameCustomEditorWithLiveSelection<AttractiveObjectType, AttractiveObjectCustomEditorContext, AttractiveObjectConfiguration, EditAttractiveObject>
    {
        private void OnEnable()
        {
            if (this.target != null)
            {
                this.drawModules = new List<GUIDrawModule<AttractiveObjectType, AttractiveObjectCustomEditorContext>>()
                {
                    new EffectRange()
                };
                this.context = new AttractiveObjectCustomEditorContext();
                this.context.AttractiveObjectInherentConfigurationData = AssetFinder.SafeSingleAssetFind<AttractiveObjectConfiguration>("t:" + typeof(AttractiveObjectConfiguration).Name)
                    .ConfigurationInherentData[((AttractiveObjectType)this.target).AttractiveObjectId];
                this.context.AttractiveObjectType = (AttractiveObjectType)this.target;
            }
        }
    }

    public class AttractiveObjectCustomEditorContext
    {
        public AttractiveObjectInherentConfigurationData AttractiveObjectInherentConfigurationData;
        public AttractiveObjectType AttractiveObjectType;
    }

    class EffectRange : GUIDrawModule<AttractiveObjectType, AttractiveObjectCustomEditorContext>
    {
        public override void SceneGUI(AttractiveObjectCustomEditorContext context, AttractiveObjectType target)
        {
            Handles.color = Color.magenta;
            var position = context.AttractiveObjectType.transform.position;
            Handles.Label(position + Vector3.up * context.AttractiveObjectInherentConfigurationData.EffectRange, nameof(context.AttractiveObjectInherentConfigurationData.EffectRange), MyEditorStyles.LabelMagenta);
            Handles.DrawWireDisc(position, Vector3.up, context.AttractiveObjectInherentConfigurationData.EffectRange);
        }
    }

}