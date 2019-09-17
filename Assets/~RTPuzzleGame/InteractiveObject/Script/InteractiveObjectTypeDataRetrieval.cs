using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    public interface IInteractiveObjectTypeDataRetrieval
    {
        Transform GetTransform();
        ModelObjectModule GetModelObjectModule();
        IContextMarkVisualFeedbackEvent GetIContextMarkVisualFeedbackEvent();
        ILineVisualFeedbackEvent GetILineVisualFeedbackEvent();
        IFovModuleDataRetrieval GetIFovModuleDataRetrieval();
        ILocalPuzzleCutsceneModuleEvent GetILocalPuzzleCutsceneModuleEvent();

        //TODO use interfaces when feature is enabled
        ObjectRepelModule GetObjectRepelModule();
        AILogicColliderModule GetAILogicColliderModule();
        ObjectSightModule GetObjectSightModule();
        LevelCompletionTriggerModule GetLevelCompletionTriggerModule();
        InteractiveObjectCutsceneControllerModule GetInteractiveObjectCutsceneControllerModule();
    }

    public partial class InteractiveObjectType : IInteractiveObjectTypeDataRetrieval
    {
        public Transform GetTransform()
        {
            return this.transform;
        }
        public ModelObjectModule GetModelObjectModule()
        {
            return this.GetModule<ModelObjectModule>();
        }
        public IContextMarkVisualFeedbackEvent GetIContextMarkVisualFeedbackEvent()
        {
            return this.GetModule<ContextMarkVisualFeedbackModule>();
        }
        public ILineVisualFeedbackEvent GetILineVisualFeedbackEvent()
        {
            return this.GetModule<LineVisualFeedbackModule>();
        }
        public IFovModuleDataRetrieval GetIFovModuleDataRetrieval()
        {
            return this.GetModule<FovModule>();
        }
        public ObjectRepelModule GetObjectRepelModule()
        {
            return this.GetModule<ObjectRepelModule>();
        }
        public AILogicColliderModule GetAILogicColliderModule()
        {
            return this.GetModule<AILogicColliderModule>();
        }
        public ObjectSightModule GetObjectSightModule()
        {
            return this.GetModule<ObjectSightModule>();
        }
        public LevelCompletionTriggerModule GetLevelCompletionTriggerModule()
        {
            return this.GetModule<LevelCompletionTriggerModule>();
        }

        public ILocalPuzzleCutsceneModuleEvent GetILocalPuzzleCutsceneModuleEvent()
        {
            return this.GetModule<LocalPuzzleCutsceneModule>();
        }

        public InteractiveObjectCutsceneControllerModule GetInteractiveObjectCutsceneControllerModule()
        {
            return this.GetModule<InteractiveObjectCutsceneControllerModule>();
        }
    }

}
