using OdinSerializer;
using System;

namespace RTPuzzle
{
    /// <summary>
    /// <see cref="AbstractAIComponent"/> is the definition of <see cref="InterfaceAIManager"/> implementation type and configuration data associated to the <see cref="InterfaceAIManager"/>.
    /// It is used by <see cref="PuzzleAIBehavior"/> to intialize the <see cref="AIBehaviorManagerContainer"/>.
    /// </summary>
    [System.Serializable]
    public abstract class AbstractAIComponent : SerializedScriptableObject
    {
    }

    public static class AIManagerTypeOperation
    {
       
        public static void InvokeIfNotNullAndTypeCorresponds(Type managerType, Type comparedType, Action action)
        {
            if (managerType == comparedType)
            {
                if (action != null)
                {
                    action.Invoke();
                }
            }
        }
    }

}
