namespace RTPuzzle
{
    public interface IActionInteractableObjectModuleEventListener
    {
        void PZ_EVT_OnActionInteractableEnter(ISelectableModule ActionInteractableObjectModule);
        void PZ_EVT_OnActionInteractableExit(ISelectableModule ActionInteractableObjectModule);
    }
}
