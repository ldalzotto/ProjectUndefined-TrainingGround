using UnityEngine;
using System.Collections;
using OdinSerializer;
using System.Collections.Generic;
using CoreGame;
using InteractiveObjectTest;

namespace RTPuzzle
{
    [System.Serializable]
    public abstract class LocalPuzzleCutsceneTemplate : SerializedScriptableObject
    {
        public abstract List<SequencedAction> GetSequencedActions(CoreInteractiveObject associatedInteractiveObject);
    }

}
