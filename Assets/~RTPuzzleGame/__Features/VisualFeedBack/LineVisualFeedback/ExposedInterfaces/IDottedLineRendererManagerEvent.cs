namespace RTPuzzle
{
    public interface IDottedLineRendererManagerEvent
    {
        void OnDottedLineDestroyed(DottedLine dottedLine);
        void OnComputeBeziersInnerPointEvent(DottedLine DottedLine);
        void OnLevelExit();
    }

}
