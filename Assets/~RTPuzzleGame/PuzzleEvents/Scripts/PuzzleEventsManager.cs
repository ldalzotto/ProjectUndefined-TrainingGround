using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public class PuzzleEventsManager : MonoBehaviour
    {
        private List<PuzzleEventsListener> registeredListeners = new List<PuzzleEventsListener>();

        #region External Dependencies
        private NPCAIManagerContainer NPCAIManagerContainer;
        #endregion

        public void Init()
        {
            this.NPCAIManagerContainer = GameObject.FindObjectOfType<NPCAIManagerContainer>();
        }

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
            this.NPCAIManagerContainer.OnGameOver();
            SceneLoadHelper.LoadScene(Coroutiner.Instance, nextZone);
        }
    }

    public interface PuzzleEventsListener
    {
        void ReceivedEvend(PuzzleEvent puzzleEvent);
    }
}
