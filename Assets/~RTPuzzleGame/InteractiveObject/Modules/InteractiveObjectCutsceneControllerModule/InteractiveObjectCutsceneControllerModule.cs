//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using CoreGame;
using UnityEngine;

namespace RTPuzzle
{


    public class InteractiveObjectCutsceneControllerModule : RTPuzzle.InteractiveObjectModule
    {
        
        private InteractiveObjectCutsceneController interactiveObjectCutsceneController;

        internal InteractiveObjectCutsceneController InteractiveObjectCutsceneController { get => interactiveObjectCutsceneController; }

        public override void Init(InteractiveObjectInitializationObject interactiveObjectInitializationObject, InteractiveObjectType interactiveObjectType)
        {
            this.interactiveObjectCutsceneController = new InteractiveObjectCutsceneController(interactiveObjectType.GetModule<ModelObjectModule>(), interactiveObjectInitializationObject);
        }

        public void Tick(float d, float timeAttenuationFactor)
        {
            this.interactiveObjectCutsceneController.Tick(d);
        }

        #region Logical Conditions
        public bool IsCutscenePlaying() { return this.interactiveObjectCutsceneController.IsCutscenePlaying(); }
        #endregion
    }

    class InteractiveObjectCutsceneController : AbstractCutsceneController
    {
        public InteractiveObjectCutsceneController(ModelObjectModule ModelObjectModule, InteractiveObjectInitializationObject InteractiveObjectInitializationObject)
        {
            base.BaseInit(ModelObjectModule.AssociatedRigidbody, ModelObjectModule.AssociatedAgent, ModelObjectModule.Animator, InteractiveObjectInitializationObject.TransformMoveManagerComponent.InteractiveObjectSharedDataTypeInherentData.TransformMoveManagerComponent, null);
        }
    }
}
