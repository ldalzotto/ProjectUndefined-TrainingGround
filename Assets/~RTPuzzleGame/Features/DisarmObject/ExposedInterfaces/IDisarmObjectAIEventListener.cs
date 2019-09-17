namespace RTPuzzle
{
    public interface IDisarmObjectAIEventListener
    {
        void AI_EVT_DisarmObject_End(AIObjectDataRetriever AIObjectDataRetriever, IDisarmObjectModuleEvent IDisarmObjectModuleEvent);
        void AI_EVT_DisarmObject_Start(AIObjectDataRetriever AIObjectDataRetriever, IDisarmObjectModuleEvent IDisarmObjectModuleEvent);
    }

}
