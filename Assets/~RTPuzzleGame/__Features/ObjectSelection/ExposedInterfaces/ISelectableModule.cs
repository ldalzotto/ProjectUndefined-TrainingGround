using UnityEngine;
using System.Collections;
using CoreGame;

namespace RTPuzzle
{
    public interface ISelectableModule : ISelectable
    {
        RTPPlayerAction GetAssociatedPlayerAction();
    }
}