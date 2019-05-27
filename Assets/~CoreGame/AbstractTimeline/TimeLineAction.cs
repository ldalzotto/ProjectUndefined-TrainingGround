namespace CoreGame
{
    public interface TimeLineAction {
#if UNITY_EDITOR
        void NodeGUI();
#endif
    }
}
