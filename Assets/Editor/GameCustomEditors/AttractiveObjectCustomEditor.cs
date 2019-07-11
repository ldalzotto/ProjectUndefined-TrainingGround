using Editor_GameDesigner;
using RTPuzzle;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor_GameCustomEditors
{
    [ExecuteInEditMode]
    [CustomEditor(typeof(AttractiveObjectTypeModule))]
    public class AttractiveObjectCustomEditor : AbstractGameCustomEditorWithLiveSelection<AttractiveObjectTypeModule, AttractiveObjectCustomEditorContext, AttractiveObjectConfiguration, EditAttractiveObject>
    {
        private void OnEnable()
        {
            if (this.target != null)
            {
                this.drawModules = new List<GUIDrawModule<AttractiveObjectTypeModule, AttractiveObjectCustomEditorContext>>()
                {
                    new EffectRange()
                };
                this.context = new AttractiveObjectCustomEditorContext();
                this.context.AttractiveObjectInherentConfigurationData = AssetFinder.SafeSingleAssetFind<AttractiveObjectConfiguration>("t:" + typeof(AttractiveObjectConfiguration).Name)
                    .ConfigurationInherentData[((AttractiveObjectTypeModule)this.target).AttractiveObjectId];
                this.context.AttractiveObjectType = (AttractiveObjectTypeModule)this.target;
            }
        }
    }

    public class AttractiveObjectCustomEditorContext
    {
        public AttractiveObjectInherentConfigurationData AttractiveObjectInherentConfigurationData;
        public AttractiveObjectTypeModule AttractiveObjectType;
    }

    class EffectRange : GUIDrawModule<AttractiveObjectTypeModule, AttractiveObjectCustomEditorContext>
    {
        public override void SceneGUI(AttractiveObjectCustomEditorContext context, AttractiveObjectTypeModule target)
        {
            Handles.color = Color.magenta;
            var position = context.AttractiveObjectType.transform.position;
            Handles.Label(position + Vector3.up * context.AttractiveObjectInherentConfigurationData.EffectRange, nameof(context.AttractiveObjectInherentConfigurationData.EffectRange), MyEditorStyles.LabelMagenta);
            Handles.DrawWireDisc(position, Vector3.up, context.AttractiveObjectInherentConfigurationData.EffectRange);
        }
    }

}