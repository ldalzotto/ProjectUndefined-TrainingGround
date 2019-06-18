using CoreGame;
using GameConfigurationID;
using System;
using System.Collections;
using UnityEngine;

namespace AdventureGame
{
    [System.Serializable]
    public class GrabAction : AContextAction
    {
        [SerializeField]
        private ItemID itemInvolved;
        [SerializeField]
        private bool deletePOIOnGrab;

        [NonSerialized]
        private GrabActionInput grabActionInput;

        #region External Dependencies
        [NonSerialized]
        private InventoryEventManager InventoryEventManager;
        [NonSerialized]
        private APointOfInterestEventManager PointOfInterestEventManager;
        #endregion

        #region Internal Dependencies
        [NonSerialized]
        private PointOfInterestType associatedPOI;
        [NonSerialized]
        private ItemReceivedPopupManager ItemReceivedPopupManager;
        #endregion

        public PointOfInterestType AssociatedPOI { get => associatedPOI; }
        public ItemID ItemInvolved { get => itemInvolved; }

        public GrabAction(ItemID itemId, bool deletePOIOnGrab, AContextAction nextAction) : base(nextAction)
        {
            this.itemInvolved = itemId;
            this.deletePOIOnGrab = deletePOIOnGrab;
        }

        public override bool ComputeFinishedConditions()
        {
            return !isItemPopupOpen();
        }

        public override void FirstExecutionAction(AContextActionInput ContextActionInput)
        {

            #region External Dependencies
            InventoryEventManager = GameObject.FindObjectOfType<InventoryEventManager>();
            PointOfInterestEventManager = GameObject.FindObjectOfType<APointOfInterestEventManager>();
            var GameCanvas = GameObject.FindObjectOfType<Canvas>();
            var GameInputManager = GameObject.FindObjectOfType<GameInputManager>();
            #endregion

            ItemReceivedPopupManager = new ItemReceivedPopupManager(GameCanvas, GameInputManager, this.itemInvolved);


            ItemReceivedPopupManager.ResetState();
            grabActionInput = (GrabActionInput)ContextActionInput;
            this.associatedPOI = grabActionInput.TargetedPOI;
        }

        public override void Tick(float d)
        {
            if (isItemPopupOpen())
            {
                ItemReceivedPopupManager.Tick(d);
            }
            else
            {
                InventoryEventManager.OnAddItem(grabActionInput.GrabbedItem);
            }
        }

        public override void AfterFinishedEventProcessed()
        {
            if (deletePOIOnGrab)
            {
                PointOfInterestEventManager.DisablePOI(this.associatedPOI);
            }
        }

        #region Logical Conditions
        private bool isItemPopupOpen()
        {
            return ItemReceivedPopupManager.IsOpened;
        }
        #endregion
    }

    class ItemReceivedPopupManager
    {
        private Canvas GameCanvas;
        private ItemReceivedPopup ItemReceivedPopup;
        private GameInputManager gameInputManager;
        private ItemID involvedItem;

        public ItemReceivedPopupManager(Canvas gameCanvas, GameInputManager gameInputManager, ItemID involvedItem)
        {
            this.GameCanvas = gameCanvas;
            this.gameInputManager = gameInputManager;
            this.involvedItem = involvedItem;
        }

        private bool isOpened;

        public bool IsOpened { get => isOpened; }

        public void Tick(float d)
        {
            ItemReceivedPopup.Tick(d);
            if (gameInputManager.CurrentInput.ActionButtonD())
            {
                ItemReceivedPopup.StartCoroutine(ExitPopup());
            }
        }

        private IEnumerator ExitPopup()
        {
            yield return ItemReceivedPopup.StartCoroutine(ItemReceivedPopup.OnClose());
            isOpened = false;
            Debug.Log("Destroy : " + ItemReceivedPopup.name);
            MonoBehaviour.Destroy(ItemReceivedPopup.gameObject);
        }

        public void ResetState()
        {
            ItemReceivedPopup = MonoBehaviour.Instantiate(PrefabContainer.Instance.ItemReceivedPopup, GameCanvas.transform);
            ItemReceivedPopup.Init(involvedItem);
            isOpened = true;
        }
    }

    public class GrabActionInput : AContextActionInput
    {
        private PointOfInterestType targetedPOI;
        private ItemID grabbedItem;

        public GrabActionInput(PointOfInterestType targetedPOI, ItemID grabbedItem)
        {
            this.targetedPOI = targetedPOI;
            this.grabbedItem = grabbedItem;
        }

        public ItemID GrabbedItem { get => grabbedItem; }
        public PointOfInterestType TargetedPOI { get => targetedPOI; }
    }
}