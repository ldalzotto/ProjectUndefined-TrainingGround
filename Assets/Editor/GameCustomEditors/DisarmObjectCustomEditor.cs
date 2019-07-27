using Editor_GameDesigner;
using RTPuzzle;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor_GameCustomEditors
{
    [ExecuteInEditMode]
    [CustomEditor(typeof(DisarmObjectModule))]
    public class DisarmObjectCustomEditor : AbstractGameCustomEditorWithLiveSelection<DisarmObjectModule, DisarmObjectCustomEditorContext, DisarmObjectConfigurationModule, EditDisarmObject>
    {
        private void OnEnable()
        {
            if (this.target != null)
            {
                this.drawModules = new List<GUIDrawModule<DisarmObjectModule, DisarmObjectCustomEditorContext>>()
                {
                    new InteractionRange()
                };
                this.context = new DisarmObjectCustomEditorContext();
                this.context.DisarmObjectInherentData = AssetFinder.SafeSingleAssetFind<DisarmObjectConfiguration>("t:" + typeof(DisarmObjectConfiguration).Name)
                    .ConfigurationInherentData[((DisarmObjectModule)this.target).DisarmObjectID];
                this.context.DisarmObjectModule = (DisarmObjectModule)this.target;

            }
        }
    }

    public class DisarmObjectCustomEditorContext
    {
        public DisarmObjectModule DisarmObjectModule;
        public DisarmObjectInherentData DisarmObjectInherentData;
    }

    class InteractionRange : GUIDrawModule<DisarmObjectModule, DisarmObjectCustomEditorContext>
    {
        public override void SceneGUI(DisarmObjectCustomEditorContext context, DisarmObjectModule target)
        {
            Handles.color = Color.magenta;
            var position = context.DisarmObjectModule.transform.position;
            Handles.Label(position + Vector3.up * context.DisarmObjectInherentData.DisarmInteractionRange, nameof(context.DisarmObjectInherentData.DisarmInteractionRange), MyEditorStyles.LabelMagenta);
            Handles.DrawWireDisc(position, Vector3.up, context.DisarmObjectInherentData.DisarmInteractionRange);
        }
    }


}