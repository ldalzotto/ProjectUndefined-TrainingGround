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

        public void Init()
        {
            this.CutscenePlayerManagerV2 = GameObject.FindObjectOfType<CutscenePlayerManagerV2>();
            this.CutsceneGlobalController = GameObject.FindObjectOfType<CutsceneGlobalController>();
            this.CameraMovementManager = GameObject.FindObjectOfType<CameraMovementManager>();
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
                this.ItemReceivedPopupObject = MonoBehaviour.Instantiate(PrefabContainer.Instance.ItemReceivedPopup, GameObject.FindObjectOfType<Canvas>().transform);
                this.ItemReceivedPopupObject.Init(this.PopupItem, () => { });
            }
            if (this.ItemReceivedPopupObject != null)
            {
                this.ItemReceivedPopupObject.Tick(d);
            }
        }
    }
#endif
}

