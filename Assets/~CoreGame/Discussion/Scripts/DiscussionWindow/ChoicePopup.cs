using System;
using System.Collections.Generic;
using TextMesh;
using UnityEngine;

namespace CoreGame
{
    public class ChoicePopup : MonoBehaviour
    {
        private const string CHOICES_CONTAINER_OBJECT_NAME = "ChoicesContainer";
        private const string CHOICES_SELECTION_RECTANGLE_OBJECT_NAME = "SelectionRectangle";

        public ChoicePopupDimensionsComponent ChoicePopupDimensionsComponent;
        public ChoicePopupSelectionManagerComponent ChoicePopupSelectionManagerComponent;
        public GeneratedTextDimensionsComponent SingleChoiceGeneratedTextDimensionsComponent;

        private ChoicePopupDimensionsManager ChoicePopupDimensionsManager;
        private ChoicePopupSelectionManager ChoicePopupSelectionManager;
        private DiscussionChoiceSelectionInputManager DiscussionChoiceSelectionInputManager;
        private ChoicePopupAnimationManager ChoicePopupAnimationManager;


        private GameObject choicesContainerObject;

        public void Tick(float d)
        {
            DiscussionChoiceSelectionInputManager.Tick();
            ChoicePopupSelectionManager.Tick(d);
        }

        #region External Events

        public void OnChoicePopupAwake(List<DiscussionChoice> nexDiscussionChoices, Vector2 localPosition, DiscussionTextConfiguration DiscussionTextRepertoire)
        {
            var corePrefabConfiguration = CoreGameSingletonInstances.CoreStaticConfigurationContainer.CoreStaticConfiguration.CorePrefabConfiguration;

            #region Internal Dependencies

            choicesContainerObject = gameObject.FindChildObjectRecursively(CHOICES_CONTAINER_OBJECT_NAME);
            var choicesSelectionRectangle = gameObject.FindChildObjectRecursively(CHOICES_SELECTION_RECTANGLE_OBJECT_NAME);

            #endregion

            transform.localPosition = localPosition;

            var choicePopupTexts = new List<ChoicePopupText>();
            foreach (var choice in nexDiscussionChoices)
            {
                var choicesPopupText = Instantiate(corePrefabConfiguration.ChoicePopupTextPrefab, choicesContainerObject.transform);
                choicesPopupText.Init(choice, DiscussionTextRepertoire, this.SingleChoiceGeneratedTextDimensionsComponent);
                choicePopupTexts.Add(choicesPopupText);
            }

            ChoicePopupDimensionsManager = new ChoicePopupDimensionsManager(ChoicePopupDimensionsComponent, (RectTransform) choicesContainerObject.transform, (RectTransform) transform);
            ChoicePopupSelectionManager = new ChoicePopupSelectionManager(choicesSelectionRectangle, ChoicePopupSelectionManagerComponent, choicePopupTexts);
            DiscussionChoiceSelectionInputManager = new DiscussionChoiceSelectionInputManager(this, CoreGameSingletonInstances.GameInputManager);
            ChoicePopupAnimationManager = new ChoicePopupAnimationManager(GetComponent<Animator>());

            ChoicePopupDimensionsManager.OnChoicePopupAwake(choicePopupTexts);
            ChoicePopupAnimationManager.OnChoicePopupAwake();
        }

        #endregion

        #region Internal Events

        public void OnSelectChoiceUp()
        {
            ChoicePopupSelectionManager.OnSelectChoiceUp();
        }

        public void OnSelectChoiceDown()
        {
            ChoicePopupSelectionManager.OnSelectChoiceDown();
        }

        #endregion

        public DiscussionChoice GetSelectedDiscussionChoice()
        {
            return ChoicePopupSelectionManager.CurrentSelectedChoice.DiscussionChoice;
        }
    }

    #region PopupDimensions

    [Serializable]
    public class ChoicePopupDimensionsComponent
    {
        public float Margin;
        public float ChoicesSpacing;
    }

    class ChoicePopupDimensionsManager
    {
        private ChoicePopupDimensionsComponent ChoicePopupDimensionsComponent;
        private RectTransform ChoicesContainerTransform;
        private RectTransform ChoicePopupTransform;

        public ChoicePopupDimensionsManager(ChoicePopupDimensionsComponent ChoicePopupDimensionsComponent, RectTransform choicesContainerTransform, RectTransform choicesPopupTransform)
        {
            this.ChoicePopupDimensionsComponent = ChoicePopupDimensionsComponent;
            this.ChoicePopupTransform = choicesPopupTransform;
            ChoicesContainerTransform = choicesContainerTransform;

            var margin = ChoicePopupDimensionsComponent.Margin;
            ChoicesContainerTransform.offsetMin = new Vector2(margin, margin);
            ChoicesContainerTransform.offsetMax = new Vector2(-margin, -margin);
        }

        public void OnChoicePopupAwake(List<ChoicePopupText> discussionChoices)
        {
            var longuestSentenceLength = 0f;
            foreach (var choice in discussionChoices)
            {
                longuestSentenceLength = Mathf.Max(choice.GetTextCharacterLength(), longuestSentenceLength);
            }

            var allChoicesHeight = 0f;
            foreach (var choice in discussionChoices)
            {
                var choiceHeight = choice.GetTextCharacterHeight();
                ((RectTransform) choice.transform).anchoredPosition = new Vector2(choice.transform.localPosition.x, -allChoicesHeight - (ChoicePopupDimensionsComponent.ChoicesSpacing * discussionChoices.IndexOf(choice)));
                ((RectTransform) choice.transform).sizeDelta = new Vector2(1, choiceHeight);
                allChoicesHeight += choiceHeight;
            }

            allChoicesHeight += ((discussionChoices.Count - 1) * ChoicePopupDimensionsComponent.ChoicesSpacing);

            ChoicePopupTransform.sizeDelta = new Vector2(longuestSentenceLength + (ChoicePopupDimensionsComponent.Margin * 2), allChoicesHeight + (ChoicePopupDimensionsComponent.Margin * 2));
        }
    }

    #endregion

    #region Discussion Choice Selection Input

    class DiscussionChoiceSelectionInputManager
    {
        private ChoicePopup ChoicePopupReference;
        private GameInputManager GameInputManager;

        public DiscussionChoiceSelectionInputManager(ChoicePopup ChoicePopupReference, GameInputManager gameInputManager)
        {
            this.ChoicePopupReference = ChoicePopupReference;
            GameInputManager = gameInputManager;
        }

        private bool isPressed;

        public void Tick()
        {
            var leftStickAxis = GameInputManager.CurrentInput.LocomotionAxis();

            if (leftStickAxis.z >= 0.5)
            {
                if (!isPressed)
                {
                    ChoicePopupReference.OnSelectChoiceUp();
                }

                isPressed = true;
            }
            else if (leftStickAxis.z <= -0.5)
            {
                if (!isPressed)
                {
                    ChoicePopupReference.OnSelectChoiceDown();
                }

                isPressed = true;
            }
            else
            {
                isPressed = false;
            }
        }
    }

    #endregion

    #region

    [Serializable]
    public class ChoicePopupSelectionManagerComponent
    {
        public float TransitionSpeed;
    }

    class ChoicePopupSelectionManager
    {
        private ChoicePopupSelectionManagerComponent ChoicePopupSelectionManagerComponent;
        private List<ChoicePopupText> choicePopupTexts;

        private ChoicePopupText currentSelectedChoice;

        private GameObject SelectionRectangleObject;
        private RectTransform targetSelectionRectangleObjectPosition;
        private bool isTransitioning;

        public ChoicePopupText CurrentSelectedChoice
        {
            get => currentSelectedChoice;
        }

        public ChoicePopupSelectionManager(GameObject selectionRectangleObject, ChoicePopupSelectionManagerComponent ChoicePopupSelectionManagerComponent, List<ChoicePopupText> choicePopupTexts)
        {
            SelectionRectangleObject = selectionRectangleObject;
            this.ChoicePopupSelectionManagerComponent = ChoicePopupSelectionManagerComponent;
            this.choicePopupTexts = choicePopupTexts;

            OnSelectedChoiceChange(choicePopupTexts[0]);
        }

        public void Tick(float d)
        {
            if (isTransitioning)
            {
                var selectionRectangleObjectTransform = (RectTransform) SelectionRectangleObject.transform;
                selectionRectangleObjectTransform.anchoredPosition = Vector2.Lerp(selectionRectangleObjectTransform.anchoredPosition, Vector2.zero, d * ChoicePopupSelectionManagerComponent.TransitionSpeed);

                if (Vector2.Distance(selectionRectangleObjectTransform.anchoredPosition, Vector2.zero) <= 0.05)
                {
                    isTransitioning = false;
                    selectionRectangleObjectTransform.anchoredPosition = Vector2.zero;
                }
            }
        }

        public void OnSelectChoiceUp()
        {
            var newIndex = choicePopupTexts.IndexOf(currentSelectedChoice);
            if (newIndex - 1 >= 0)
            {
                OnSelectedChoiceChange(choicePopupTexts[newIndex - 1]);
            }
        }

        public void OnSelectChoiceDown()
        {
            var newIndex = choicePopupTexts.IndexOf(currentSelectedChoice);
            if (newIndex + 1 <= choicePopupTexts.Count - 1)
            {
                OnSelectedChoiceChange(choicePopupTexts[newIndex + 1]);
            }
        }

        private void OnSelectedChoiceChange(ChoicePopupText newSelectedChoice)
        {
            currentSelectedChoice = newSelectedChoice;
            targetSelectionRectangleObjectPosition = (RectTransform) newSelectedChoice.transform;
            SelectionRectangleObject.transform.SetParent(newSelectedChoice.transform);
            ((RectTransform) SelectionRectangleObject.transform).sizeDelta = Vector2.one;
            isTransitioning = true;
        }
    }

    #endregion

    #region Choice Popup Animation

    class ChoicePopupAnimationManager
    {
        private const string ENTER_ANIMATION = "PopupChoiceEnterAnimation";

        private Animator popupAnimator;

        public ChoicePopupAnimationManager(Animator popupAnimator)
        {
            this.popupAnimator = popupAnimator;
        }

        public void OnChoicePopupAwake()
        {
            popupAnimator.Play(ENTER_ANIMATION);
        }
    }

    #endregion
}