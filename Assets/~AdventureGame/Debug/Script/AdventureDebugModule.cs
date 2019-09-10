using CoreGame;
using GameConfigurationID;
using UnityEngine;

namespace AdventureGame
{

#if UNITY_EDITOR
    public class AdventureDebugModule : MonoBehaviour
    {

        [Header("Cutscene")]
        [CustomEnum()]
        public CutsceneId cutsceneId;
        public bool play;

        [Header("Camera Follow")]
        [CustomEnum()]
        public PointOfInterestId poiToFollow;
        public bool follow;

        [Header("Camera Rotation")]
        public float targetAngle;
        public bool targetRotation;

        private CutscenePlayerManagerV2 CutscenePlayerManagerV2;
        private CutsceneGlobalController CutsceneGlobalController;
        private CameraMovementManager CameraMovementManager;

        [Header("DiscussioonUI")]
        public bool ItemReceivedPopup;
        [CustomEnum()]
        public ItemID PopupItem;
        private ItemReceivedPopup ItemReceivedPopupObject;

        [Header("Tutorial Step")]
        public bool PlayTutorialStep;
        public bool InterrupTutorialStep;
        [CustomEnum()]
        public TutorialStepID TutorialStepID;
        private TutorialManager TutorialManager;

        public void Init()
        {
            this.CutscenePlayerManagerV2 = AdventureGameSingletonInstances.CutscenePlayerManagerV2;
            this.CutsceneGlobalController = AdventureGameSingletonInstances.CutsceneGlobalController;
            this.CameraMovementManager = CoreGameSingletonInstances.CameraMovementManager;
            this.TutorialManager = CoreGameSingletonInstances.TutorialManager;
        }

        public void Tick(float d)
        {
            if (this.play)
            {
                this.play = false;
                this.CutscenePlayerManagerV2.ManualCutsceneStart(this.cutsceneId);
            }
            if (this.follow)
            {
                this.follow = false;
                this.CutsceneGlobalController.SetCameraFollow(this.poiToFollow);
            }
            if (this.targetRotation)
            {
                this.targetRotation = false;
                this.CameraMovementManager.SetTargetAngle(this.targetAngle);
            }
            if (this.ItemReceivedPopup)
            {
                this.ItemReceivedPopup = false;
                this.ItemReceivedPopupObject = MonoBehaviour.Instantiate(PrefabContainer.Instance.ItemReceivedPopup, CoreGameSingletonInstances.GameCanvas.transform);
                this.ItemReceivedPopupObject.Init(this.PopupItem, () => { });
            }
            if (this.ItemReceivedPopupObject != null)
            {
                this.ItemReceivedPopupObject.Tick(d);
            }
            if (this.PlayTutorialStep)
            {
                this.PlayTutorialStep = false;
                this.TutorialManager.PlayTutorialStep(this.TutorialStepID);
            }
            if (this.InterrupTutorialStep)
            {
                this.TutorialManager.Abort();
            }
        }
    }
#endif
}

