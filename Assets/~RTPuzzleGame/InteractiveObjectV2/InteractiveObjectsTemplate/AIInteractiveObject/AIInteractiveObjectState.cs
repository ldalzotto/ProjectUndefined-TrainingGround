namespace InteractiveObjectTest
{
    public class AIAttractiveObjectState
    {
        public delegate void OnAIIsJustAttractedByAttractiveObjectDelegate(CoreInteractiveObject AttractedInteractiveObject);
        public delegate void OnAIisNoMoreAttractedByAttractiveObjectDelegate(CoreInteractiveObject AttractedInteractiveObject);

        private OnAIIsJustAttractedByAttractiveObjectDelegate OnAIIsJustAttractedByAttractiveObject;
        private OnAIisNoMoreAttractedByAttractiveObjectDelegate OnAIisNoMoreAttractedByAttractiveObject;

        private CoreInteractiveObject AttractedInteractiveObject;

        public AIAttractiveObjectState(OnAIIsJustAttractedByAttractiveObjectDelegate onAIIsJustAttractedByAttractiveObject, OnAIisNoMoreAttractedByAttractiveObjectDelegate onAIisNoMoreAttractedByAttractiveObject)
        {
            OnAIIsJustAttractedByAttractiveObject = onAIIsJustAttractedByAttractiveObject;
            OnAIisNoMoreAttractedByAttractiveObject = onAIisNoMoreAttractedByAttractiveObject;
        }

        public bool IsAttractedByAttractiveObject { get; private set; }

        public void SetIsAttractedByAttractiveObject(bool value, CoreInteractiveObject AttractedInteractiveObject)
        {
            bool hasChanged = this.IsAttractedByAttractiveObject != value;
            bool changedTo = value;
            this.IsAttractedByAttractiveObject = value;

            if (value) { this.AttractedInteractiveObject = AttractedInteractiveObject; }

            if (hasChanged)
            {
                if (changedTo)
                {
                    this.OnAIIsJustAttractedByAttractiveObject.Invoke(this.AttractedInteractiveObject);
                }
                else
                {
                    this.OnAIisNoMoreAttractedByAttractiveObject.Invoke(this.AttractedInteractiveObject);
                }
            }
        }
    }

    public class AIDisarmObjectState
    {
        public delegate void OnAIIsJustDisarmingObjectDelegate();
        public delegate void OnAIIsNoMoreJustDisarmingObjectDelegate();

        private OnAIIsJustDisarmingObjectDelegate OnAIIsJustDisarmingObject;
        private OnAIIsNoMoreJustDisarmingObjectDelegate OnAIIsNoMoreJustDisarmingObject;

        public AIDisarmObjectState(OnAIIsJustDisarmingObjectDelegate onAIIsJustDisarmingObject, OnAIIsNoMoreJustDisarmingObjectDelegate onAIIsNoMoreJustDisarmingObject)
        {
            OnAIIsJustDisarmingObject = onAIIsJustDisarmingObject;
            OnAIIsNoMoreJustDisarmingObject = onAIIsNoMoreJustDisarmingObject;
        }

        private bool isDisarming;

        public bool IsDisarming
        {
            get => isDisarming;
            set
            {
                bool hasChanged = this.isDisarming != value;
                bool changedTo = value;
                this.isDisarming = value;
                if (hasChanged)
                {
                    if (changedTo)
                    {
                        this.OnAIIsJustDisarmingObject.Invoke();
                    }
                    else
                    {
                        this.OnAIIsNoMoreJustDisarmingObject.Invoke();
                    }
                }
            }
        }
    }


}
