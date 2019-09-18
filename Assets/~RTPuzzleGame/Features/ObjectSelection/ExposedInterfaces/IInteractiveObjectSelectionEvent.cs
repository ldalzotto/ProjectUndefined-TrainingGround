using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    public interface IInteractiveObjectSelectionEvent
    {
        void OnSelectableEnter(ISelectableModule ISelectableModule);
        void OnSelectableExit(ISelectableModule ISelectableModule);
    }

}
