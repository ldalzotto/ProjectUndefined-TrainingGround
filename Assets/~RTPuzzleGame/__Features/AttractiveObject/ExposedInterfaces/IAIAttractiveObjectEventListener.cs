namespace RTPuzzle
{
    public interface IAIAttractiveObjectEventListener
    {
        void AI_AttractedObject_Start(IAttractiveObjectModuleDataRetriever InvolvedAttractiveObjectModuleDataRetriever, AIObjectDataRetriever AIObjectDataRetriever);
        void AI_AttractedObject_End(IAttractiveObjectModuleDataRetriever InvolvedAttractiveObjectModuleDataRetriever, AIObjectDataRetriever AIObjectDataRetriever);
    }
}