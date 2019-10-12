namespace InteractiveObjectTest
{
    public class AIAttractiveObjectState
    {
        public CoreInteractiveObject AttractedInteractiveObject { get; private set; }
        public BoolVariable IsAttractedByAttractiveObject { get; private set; }

        public AIAttractiveObjectState(BoolVariable isAttractedByAttractiveObject)
        {
            IsAttractedByAttractiveObject = isAttractedByAttractiveObject;
        }

        public void SetIsAttractedByAttractiveObject(bool value, CoreInteractiveObject AttractedInteractiveObject)
        {
            this.AttractedInteractiveObject = AttractedInteractiveObject;
            this.IsAttractedByAttractiveObject.SetValue(value);
        }
    }

    public class AIDisarmObjectState
    {
        public BoolVariable IsDisarming { get; private set; }

        public AIDisarmObjectState(BoolVariable IsDisarming)
        {
            this.IsDisarming = IsDisarming;
        }
    }


    public class AIPatrollingState
    {
        public bool isPatrolling;
    }
}
