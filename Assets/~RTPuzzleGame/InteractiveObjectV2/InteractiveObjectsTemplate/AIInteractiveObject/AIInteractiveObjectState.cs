namespace InteractiveObjectTest
{
    public class AIAttractiveObjectState
    {
        public delegate void OnAIIsJustAttractedByAttractiveObjectDelegate();
        public delegate void OnAIisNoMoreAttractedByAttractiveObjectDelegate();

        private OnAIIsJustAttractedByAttractiveObjectDelegate OnAIIsJustAttractedByAttractiveObject;
        private OnAIisNoMoreAttractedByAttractiveObjectDelegate OnAIisNoMoreAttractedByAttractiveObject;

        public AIAttractiveObjectState(OnAIIsJustAttractedByAttractiveObjectDelegate onAIIsJustAttractedByAttractiveObject, OnAIisNoMoreAttractedByAttractiveObjectDelegate onAIisNoMoreAttractedByAttractiveObject)
        {
            OnAIIsJustAttractedByAttractiveObject = onAIIsJustAttractedByAttractiveObject;
            OnAIisNoMoreAttractedByAttractiveObject = onAIisNoMoreAttractedByAttractiveObject;
        }

        private bool isAttractedByAttractiveObject;

        public bool IsAttractedByAttractiveObject
        {
            get => isAttractedByAttractiveObject;
            set
            {
                bool hasChanged = this.isAttractedByAttractiveObject != value;
                bool changedTo = value;
                this.isAttractedByAttractiveObject = value;
                if (hasChanged)
                {
                    if (changedTo)
                    {
                        this.OnAIIsJustAttractedByAttractiveObject.Invoke();
                    }
                    else
                    {
                        this.OnAIisNoMoreAttractedByAttractiveObject.Invoke();
                    }
                }
            }
        }
    }


}
