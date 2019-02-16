using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public class PuzzleEventsManager : MonoBehaviour
    {
        private List<PuzzleEventsListener> registeredListeners = new List<PuzzleEventsListener>();

        public void AddListener(PuzzleEventsListener PuzzleEventsListener)
        {
            registeredListeners.Add(PuzzleEventsListener);
        }

        public void SendEvent(PuzzleEvent PuzzleEvent)
        {
            foreach (var listener in registeredListeners)
            {
                listener.ReceivedEvend(PuzzleEvent);
            }
        }

        public void OnGameOver(LevelZonesID nextZone)
        {
            SceneLoadHelper.LoadScene(Coroutiner.Instance, nextZone);
        }
    }

    public interface PuzzleEventsListener
    {
        void ReceivedEvend(PuzzleEvent puzzleEvent);
    }
}
