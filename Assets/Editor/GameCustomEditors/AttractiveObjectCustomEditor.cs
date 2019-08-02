using Editor_GameDesigner;
using RTPuzzle;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor_GameCustomEditors
{
    [ExecuteInEditMode]
    [CustomEditor(typeof(AttractiveObjectModule))]
    public class AttractiveObjectCustomEditor : AbstractGameCustomEditorWithLiveSelection<AttractiveObjectModule, AttractiveObjectCustomEditorContext, AttractiveObjectConfigurationModule, EditAttractiveObject>
    {
        private void OnEnable()
        {
            if (this.target != null)
            {
                this.drawModules = new List<GUIDrawModule<AttractiveObjectModule, AttractiveObjectCustomEditorContext>>()
                {
                    new EffectRange()
                };
                this.context = new AttractiveObjectCustomEditorContext();
                this.context.AttractiveObjectInherentConfigurationData = AssetFinder.SafeSingleAssetFind<AttractiveObjectConfiguration>("t:" + typeof(AttractiveObjectConfiguration).Name)
                    .ConfigurationInherentData[((AttractiveObjectModule)this.target).AttractiveObjectId];
                this.context.AttractiveObjectType = (AttractiveObjectModule)this.target;
            }
        }
    }

    public class AttractiveObjectCustomEditorContext
    {
        public AttractiveObjectInherentConfigurationData AttractiveObjectInherentConfigurationData;
        public AttractiveObjectModule AttractiveObjectType;
    }

    class EffectRange : GUIDrawModule<AttractiveObjectModule, AttractiveObjectCustomEditorContext>
    {
        public override void SceneGUI(AttractiveObjectCustomEditorContext context, AttractiveObjectModule target)
        {
            Handles.color = Color.magenta;
            var position = context.AttractiveObjectType.transform.position;
            Handles.Label(position + Vector3.up * context.AttractiveObjectInherentConfigurationData.EffectRange, nameof(context.AttractiveObjectInherentConfigurationData.EffectRange), MyEditorStyles.LabelMagenta);
            Handles.DrawWireDisc(position, Vector3.up, context.AttractiveObjectInherentConfigurationData.EffectRange);
        }
    }

}