using UnityEngine;

namespace RTPuzzle
{
    public abstract class AbstractContextMarkVisualFeedbackEvent
    {
        public ModelObjectModule ModelObjectModule;

        protected AbstractContextMarkVisualFeedbackEvent(ModelObjectModule modelObjectModule)
        {
            ModelObjectModule = modelObjectModule;
        }
    }

    public class ProjectileHittedFirstTimeEvent : AbstractContextMarkVisualFeedbackEvent
    {
        public ProjectileHittedFirstTimeEvent() : base(null)
        {
        }
    }

    public class AttractedStartEvent : AbstractContextMarkVisualFeedbackEvent
    {
        public AttractedStartEvent(ModelObjectModule modelObjectModule) : base(modelObjectModule)
        {
        }
    }

    public class EscapeWithoutTargetEvent : AbstractContextMarkVisualFeedbackEvent
    {
        public EscapeWithoutTargetEvent() : base(null)
        {
        }
    }

    public class DeleteEvent : AbstractContextMarkVisualFeedbackEvent
    {
        public DeleteEvent() : base(null)
        {
        }
    }
}