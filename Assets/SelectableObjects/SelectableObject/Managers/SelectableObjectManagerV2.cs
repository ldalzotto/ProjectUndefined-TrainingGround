﻿using System.Collections.Generic;
using CoreGame;

namespace SelectableObject
{
    public class SelectableObjectManagerV2 : GameSingleton<SelectableObjectManagerV2>
    {
        #region Internal Managers

        internal SelectableObjectRendererManager SelectableObjectRendererManager { get; private set; }

        #endregion

        #region Data Retrieval

        public ISelectableObjectSystem GetCurrentSelectedObject()
        {
            return CurrentSelectedObject;
        }

        #endregion

        public void Init(IGameInputManager GameInputManager)
        {
            #region Event Registering

            SelectableObjectEventsManager.RegisterOnSelectableObjectEnterEventAction(OnSelectableObjectEnder);
            SelectableObjectEventsManager.RegisterOnSelectableObjectExitEventAction(RemoveInteractiveObjectFromSelectable);

            #endregion

            #region Exnternal Dependencies

            var CoreMaterialConfiguration = CoreGameSingletonInstances.CoreStaticConfigurationContainer.CoreStaticConfiguration.CoreMaterialConfiguration;
            this.GameInputManager = GameInputManager;

            #endregion

            SelectableObjectRendererManager = new SelectableObjectRendererManager(CoreMaterialConfiguration);
            interactableObjects = new List<ISelectableObjectSystem>();
        }

        public virtual void Tick(float d)
        {
            if (interactableObjects.Count > 0 && CurrentSelectedObject == null) SetCurrentSelectedObject(interactableObjects[0]);

            if (CurrentSelectedObject != null && GameInputManager.CurrentInput.SwitchSelectionButtonD()) SwitchSelection();

            if (interactableObjects.Count == 0) SetCurrentSelectedObject(default(ISelectableObjectSystem));

            SelectableObjectRendererManager.Tick(d, GetCurrentSelectedObject(), interactableObjects.Count > 1);
        }

        private void SwitchSelection()
        {
            var currentSelectedIndex = interactableObjects.IndexOf(CurrentSelectedObject);
            var nextSelectedIndex = currentSelectedIndex + 1;
            if (nextSelectedIndex == interactableObjects.Count) nextSelectedIndex = 0;

            SetCurrentSelectedObject(interactableObjects[nextSelectedIndex]);
        }

        private void SetCurrentSelectedObject(ISelectableObjectSystem SelectableObject)
        {
            if (CurrentSelectedObject != null)
            {
                if (!CurrentSelectedObject.Equals(SelectableObject))
                {
                    SelectableObjectEventsManager.OnSelectableObjectNoMoreSelected(CurrentSelectedObject);
                    if (SelectableObject != null) SelectableObjectEventsManager.OnSelectableObjectSelected(SelectableObject);
                }
            }
            else if (CurrentSelectedObject == null)
            {
                if (SelectableObject != null) SelectableObjectEventsManager.OnSelectableObjectSelected(SelectableObject);
            }

            CurrentSelectedObject = SelectableObject;
        }

        private void RemoveInteractiveObjectFromSelectable(ISelectableObjectSystem selectableObject)
        {
            if (CurrentSelectedObject != null && interactableObjects.Contains(CurrentSelectedObject)) SetCurrentSelectedObject(default(ISelectableObjectSystem));

            interactableObjects.Remove(selectableObject);
        }

        private void OnSelectableObjectEnder(ISelectableObjectSystem selectableObject)
        {
            if (!interactableObjects.Contains(selectableObject)) interactableObjects.Add(selectableObject);
        }

        #region Internal State

        private ISelectableObjectSystem CurrentSelectedObject;
        private List<ISelectableObjectSystem> interactableObjects;

        #endregion

        #region External Dependencies

        private SelectableObjectEventsManager SelectableObjectEventsManager = SelectableObjectEventsManager.Get();
        private IGameInputManager GameInputManager;

        #endregion
    }
}