using CoreGame;
using GameConfigurationID;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace AdventureGame
{
    public class ItemReceivedPopup : MonoBehaviour
    {
        private const string PopupContentObjectName = "PopupContent";
        private const string PopupTextObjectName = "ItemText";
        private const string PopupImageObjectName = "ItemIcon";

        public ItemReceivedPopupDimensionsComponent ItemReceivedPopupDimensionsComponent;
        public DiscussionWriterComponent DiscussionWriterComponent;

        private ItemReceivedPopupDimensionsManager ItemReceivedPopupDimensionsManager;
        private DiscussionWriterManager DiscussionWriterManager;
        private ItemReceivedPopupAnimationManager ItemReceivedPopupAnimationManager;

        public void Init(ItemID involvedItem)
        {
            #region Internal Dependencies
            var popupContentTransform = (RectTransform)gameObject.FindChildObjectRecursively(PopupContentObjectName).transform;
            var popupText = ((RectTransform)gameObject.FindChildObjectRecursively(PopupTextObjectName).transform).GetComponent<Text>();
            var popupIcon = ((RectTransform)gameObject.FindChildObjectRecursively(PopupImageObjectName).transform).GetComponent<Image>();
            #endregion

            #region External Dependencies
            var adventureGameConfigurationManager = GameObject.FindObjectOfType<AdventureGameConfigurationManager>();
            #endregion

            ItemReceivedPopupDimensionsManager = new ItemReceivedPopupDimensionsManager(ItemReceivedPopupDimensionsComponent, (RectTransform)transform, popupContentTransform);
            DiscussionWriterManager = new DiscussionWriterManager(() => { this.OnTextFinishedWriting(); }, DiscussionWriterComponent, popupText);
            ItemReceivedPopupAnimationManager = new ItemReceivedPopupAnimationManager(GetComponent<Animator>());
           //TODO popupIcon.sprite = ItemResourceResolver.ResolveItemInventoryIcon(involvedItem);
            this.OnDiscussionTextStartWriting(adventureGameConfigurationManager.ItemConf()[involvedItem].ItemReceivedDescriptionText);
        }

        public void Tick(float d)
        {
            DiscussionWriterManager.Tick(d);
        }

        #region External Events
        public IEnumerator OnClose()
        {
            return ItemReceivedPopupAnimationManager.PlayClose();
        }
        #endregion

        #region Internal Events
        public void OnDiscussionTextStartWriting(string text)
        {
            DiscussionWriterManager.OnDiscussionTextStartWriting(text);
        }
        public void OnTextFinishedWriting()
        {

        }
        #endregion
    }


    #region ItemReceivePopup Dimensions
    [System.Serializable]
    public class ItemReceivedPopupDimensionsComponent
    {
        public float PopupMargin;
        public float MaxPopupWidth;
        public float MaxPopupHeight;
    }

    class ItemReceivedPopupDimensionsManager
    {
        private ItemReceivedPopupDimensionsComponent ItemReceivedPopupDimensionsComponent;
        private RectTransform popupContentTransform;

        public ItemReceivedPopupDimensionsManager(ItemReceivedPopupDimensionsComponent itemReceivedPopupDimensionsComponent,
            RectTransform rootPopupElementTransform, RectTransform popupContentTransform)
        {
            var margin = itemReceivedPopupDimensionsComponent.PopupMargin;
            ItemReceivedPopupDimensionsComponent = itemReceivedPopupDimensionsComponent;
            popupContentTransform.offsetMin = new Vector2(margin, margin);
            popupContentTransform.offsetMax = new Vector2(-margin, -margin);
            rootPopupElementTransform.sizeDelta = new Vector2(itemReceivedPopupDimensionsComponent.MaxPopupWidth, itemReceivedPopupDimensionsComponent.MaxPopupHeight);
        }

    }
    #endregion

    #region ItemReceiving Animation
    public class ItemReceivedPopupAnimationManager
    {
        private const string OPENING_ANIMATION_NAME = "ItemReceivedPopupOpening";
        private const string CLOSING_ANIMATION_NAME = "ItemReceivedPopupClosing";


        private Animator Animator;

        public ItemReceivedPopupAnimationManager(Animator animator)
        {
            Animator = animator;
        }

        public void PlayOpening()
        {
            Animator.Play(OPENING_ANIMATION_NAME);
        }

        public IEnumerator PlayClose()
        {
            return AnimationPlayerHelper.PlayAndWait(Animator, CLOSING_ANIMATION_NAME, 0, 0f);
        }
    }
    #endregion
}
