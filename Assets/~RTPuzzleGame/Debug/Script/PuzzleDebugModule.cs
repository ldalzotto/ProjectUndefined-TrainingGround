using CoreGame;
using LevelManagement;
using UnityEngine;
using UnityEngine.UI;

namespace RTPuzzle
{
    public class PuzzleDebugModule : MonoBehaviour
    {
#if UNITY_EDITOR
        private static PuzzleDebugModule Instance;

        public static PuzzleDebugModule Get()
        {
            if (Instance == null)
            {
                Instance = FindObjectOfType<PuzzleDebugModule>();
            }

            return Instance;
        }


        public bool TriggerLevelSuccessEvent;


        /*
         *        public ProceduralText(string initialRawText ,  GeneratedTextParameter GeneratedTextParameter, GeneratedTextDimensionsComponent GeneratedTextDimensionsComponent,
            //DiscussionHeightChangeListener DiscussionHeightChangeListener, Text textAreaText)
         * 
         */

        public bool ShowDiscussionWindow;
        public string TextDisplayed;
        public GeneratedTextDimensionsComponent GeneratedTextDimensionsComponent;
        public Text TextObject;
        public DiscussionTextInherentData DiscussionTextInherentData;
        private ProceduralText ProceduralText;

        public void Init()
        {
        }

        public void Tick()
        {
            if (TriggerLevelSuccessEvent)
            {
                LevelAvailabilityTimelineEventManager.Get().OnLevelCompleted();
                TriggerLevelSuccessEvent = false;
            }

            if (ShowDiscussionWindow)
            {
                ShowDiscussionWindow = false;
                this.ProceduralText = new ProceduralText(this.TextDisplayed, this.GeneratedTextDimensionsComponent, null, this.TextObject);
                this.ProceduralText.CalculateCurrentPage();
            }

            if (this.ProceduralText != null)
            {
                this.ProceduralText.Increment();
            }
        }
#endif
    }
}