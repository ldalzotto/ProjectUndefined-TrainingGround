using UnityEngine;
using System.Collections;

namespace AIObjects
{
    public interface IActionAbortedOnDestinationReached
    {
        void OnDestinationReached();
    }
}