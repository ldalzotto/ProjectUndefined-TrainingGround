namespace InteractiveObjectTest
{
    [System.Serializable]
    public class AttractiveObjectInitializerData
    {
        public AttractiveObjectSystemDefinition AttractiveObjectSystemDefinition;
    }
    [System.Serializable]
    public class AttractiveInteractiveObjectInitializer : AInteractiveObjectInitializer
    {
        public AttractiveObjectInitializerData InteractiveObjectInitializerData;
        public override void Init()
        {
            InteractiveObjectV2Manager.Get().OnInteractiveObjectCreated(new AttractiveInteractiveObject(new InteractiveGameObject(this.gameObject), InteractiveObjectInitializerData));
        }
    }

}
