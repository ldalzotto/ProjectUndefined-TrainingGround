namespace RTPuzzle
{
    public interface IDisarmObjectAIEventListener
    {
        void AI_EVT_DisarmObject_End(AIObjectDataRetriever AIObjectDataRetriever, IDisarmObjectModuleDataRetrieval disarmedObjectModule);
        void AI_EVT_DisarmObject_Start(AIObjectDataRetriever AIObjectDataRetriever, IDisarmObjectModuleDataRetrieval disarmedObjectModule);
    }

}
