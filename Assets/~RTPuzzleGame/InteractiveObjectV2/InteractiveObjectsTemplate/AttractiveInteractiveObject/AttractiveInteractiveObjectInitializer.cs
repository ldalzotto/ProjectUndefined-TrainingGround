namespace InteractiveObjectTest
{
    [System.Serializable]
    public class AttractiveObjectInitializerData
    {
        public AttractiveObjectSystemDefinition AttractiveObjectSystemDefinition;
        public bool IsDisarmable;
        public DisarmSystemDefinition DisarmSystemDefinition;
        public SelectableObjectSystemDefinition SelectableObjectSystemDefinition;
    }

    [System.Serializable]
    public class AttractiveInteractiveObjectInitializer : A_InteractiveObjectInitializer
    {
        public AttractiveObjectInitializerData InteractiveObjectInitializerData;

        protected override CoreInteractiveObject GetInteractiveObject()
        {
            return new AttractiveInteractiveObject(new InteractiveGameObject(this.gameObject), InteractiveObjectInitializerData);
        }
    }

}
