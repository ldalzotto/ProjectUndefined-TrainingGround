using Editor_GameDesigner;
using RTPuzzle;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor_GameCustomEditors
{
    [ExecuteInEditMode]
    [CustomEditor(typeof(ActionInteractableObjectModule))]
    public class ActionInteractiveObjectCustomEditor : AbstractGameCustomEditorWithLiveSelection<ActionInteractableObjectModule, ActionInteractiveObjectCustomEditorContext, ActionInteractableObjectConfigurationModule, EditActionInteractableObject>
    {
        private void OnEnable()
        {
            if (this.target != null)
            {
                this.drawModules = new List<GUIDrawModule<ActionInteractableObjectModule, ActionInteractiveObjectCustomEditorContext>>()
                {
                    new ActionInteractionObjectRange()
                };
                this.context = new ActionInteractiveObjectCustomEditorContext();
                var conf = AssetFinder.SafeSingleAssetFind<ActionInteractableObjectConfiguration>("t:" + typeof(ActionInteractableObjectConfiguration).Name);
                if (conf.ConfigurationInherentData.ContainsKey(((ActionInteractableObjectModule)this.target).ActionInteractableObjectID))
                {
                    this.context.ActionInteractableObjectInherentData = conf.ConfigurationInherentData[((ActionInteractableObjectModule)this.target).ActionInteractableObjectID];
                }
                this.context.ActionInteractableObjectModule = (ActionInteractableObjectModule)this.target;

            }
        }
    }

    public class ActionInteractiveObjectCustomEditorContext
    {
        public ActionInteractableObjectModule ActionInteractableObjectModule;
        public ActionInteractableObjectInherentData ActionInteractableObjectInherentData;
    }

    class ActionInteractionObjectRange : GUIDrawModule<ActionInteractableObjectModule, ActionInteractiveObjectCustomEditorContext>
    {
        public override void SceneGUI(ActionInteractiveObjectCustomEditorContext context, ActionInteractableObjectModule target)
        {
            if (context.ActionInteractableObjectInherentData != null)
            {
                Handles.color = Color.magenta;
                var position = context.ActionInteractableObjectModule.transform.position;
                Handles.Label(position + Vector3.up * context.ActionInteractableObjectInherentData.InteractionRange, nameof(context.ActionInteractableObjectInherentData.InteractionRange), MyEditorStyles.LabelMagenta);
                Handles.DrawWireDisc(position, Vector3.up, context.ActionInteractableObjectInherentData.InteractionRange);
            }
        }
    }


}