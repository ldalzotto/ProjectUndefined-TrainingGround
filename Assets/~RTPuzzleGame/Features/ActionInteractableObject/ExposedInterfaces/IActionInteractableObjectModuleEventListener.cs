namespace RTPuzzle
{
    public interface IActionInteractableObjectModuleEventListener
    {
        void PZ_EVT_OnActionInteractableEnter(ActionInteractableObjectModule ActionInteractableObjectModule);
        void PZ_EVT_OnActionInteractableExit(ActionInteractableObjectModule ActionInteractableObjectModule);
    }
}
