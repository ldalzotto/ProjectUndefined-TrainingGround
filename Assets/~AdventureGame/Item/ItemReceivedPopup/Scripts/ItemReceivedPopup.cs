using CoreGame;
using GameConfigurationID;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace AdventureGame
{
    public class ItemReceivedPopup : MonoBehaviour
    {
        private const string PopupContentObjectName = "PopupContent";
        private const string PopupImageObjectName = "ItemIcon";
        
        public DiscussionWriterComponent DiscussionWriterComponent;
        private DiscussionWindow DiscussionWindow;

        public void Init(ItemID involvedItem, Action onWindowClosed)
        {
            #region Internal Dependencies
            var popupIcon = ((RectTransform)gameObject.FindChildObjectRecursively(PopupImageObjectName).transform).GetComponent<Image>();
            #endregion

            #region External Dependencies
            var adventureGameConfigurationManager = GameObject.FindObjectOfType<AdventureGameConfigurationManager>();
            #endregion

            this.DiscussionWindow = this.GetComponent<DiscussionWindow>();
            this.DiscussionWindow.InitializeDependencies(onWindowClosed);
            
            this.OnDiscussionTextStartWriting(adventureGameConfigurationManager.ItemConf()[involvedItem].ItemReceivedDescriptionText);
        }

        public void Tick(float d)
        {
            this.DiscussionWindow.Tick(d);
        }

        #region External Events
        public void PlayCloseAnimation()
        {
            this.DiscussionWindow.PlayDiscussionCloseAnimation();
        }
        #endregion

        #region Internal Events
        public void OnDiscussionTextStartWriting(string text)
        {
            //TODO
          //  this.DiscussionWindow.OnDiscussionWindowAwake(text, (RectTransform)this.transform);
        }
        #endregion
    }
}
