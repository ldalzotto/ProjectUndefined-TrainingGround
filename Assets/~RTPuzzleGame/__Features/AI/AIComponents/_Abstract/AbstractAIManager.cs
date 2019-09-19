using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    public abstract class AbstractAIManager<C> where C : AbstractAIComponent
    {
        protected C AssociatedAIComponent;

        protected AbstractAIManager(C associatedAIComponent)
        {
            AssociatedAIComponent = associatedAIComponent;
        }
    }
}
