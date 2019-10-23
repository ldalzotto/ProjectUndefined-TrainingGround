using System;
using System.Collections.Generic;
using InteractiveObjects;
using OdinSerializer;
using SequencedAction;

namespace RTPuzzle
{
    [Serializable]
    public abstract class LocalPuzzleCutsceneTemplate : SerializedScriptableObject
    {
        public abstract List<ASequencedAction> GetSequencedActions(CoreInteractiveObject associatedInteractiveObject);
    }
}