using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CoreGame
{
    public interface ISelectable
    {
        ExtendedBounds GetAverageModelBoundLocalSpace();
        Transform GetTransform();
    }

    public abstract class AbstractSelectableObjectSelectionManager<T> : MonoBehaviour where T : ISelectable
    {
        #region Internal State
        private T CurrentSelectedObject;
        private List<T> interactableObjects;
        #endregion

        #region External Dependencies
        private IGameInputManager GameInputManager;
        public abstract SelectableObjectSelectionManagerEventListener<T> SelectableObjectSelectionManagerEventListener { get; }
        #endregion

        #region Internal Managers
        private ObjectSelectionRendererManager InteractiveObjectSelectionRendererManager;
        #endregion

        #region Data Retrieval
        public T GetCurrentSelectedObject()
        {
            return this.CurrentSelectedObject;
        }
        #endregion


        public virtual void Init(IGameInputManager GameInputManager)
        {
            #region Exnternal Dependencies
            var CoreMaterialConfiguration = CoreGameSingletonInstances.CoreStaticConfigurationContainer.CoreStaticConfiguration.CoreMaterialConfiguration;
            this.GameInputManager = GameInputManager;
            #endregion

            this.InteractiveObjectSelectionRendererManager = new ObjectSelectionRendererManager(CoreMaterialConfiguration);
            this.interactableObjects = new List<T>();
        }

        public virtual void Tick(float d)
        {
            if (this.interactableObjects.Count > 0 && this.CurrentSelectedObject == null)
            {
                this.SetCurrentSelectedObject(this.interactableObjects.First());
            }
            if (this.CurrentSelectedObject != null && this.GameInputManager.CurrentInput.SwitchSelectionButtonD())
            {
                SwitchSelection();
            }
            if (this.interactableObjects.Count == 0)
            {
                this.SetCurrentSelectedObject(default(T));
            }

            this.InteractiveObjectSelectionRendererManager.Tick(d, this.GetCurrentSelectedObject(), this.interactableObjects.Count > 1);
        }

        private void SwitchSelection()
        {
            var currentSelectedIndex = this.interactableObjects.IndexOf(this.CurrentSelectedObject);
            int nextSelectedIndex = currentSelectedIndex + 1;
            if (nextSelectedIndex == this.interactableObjects.Count)
            {
                nextSelectedIndex = 0;
            }
            this.SetCurrentSelectedObject(this.interactableObjects[nextSelectedIndex]);
        }

        private void SetCurrentSelectedObject(T SelectableObject)
        {
            if (this.CurrentSelectedObject != null)
            {
                if (!this.CurrentSelectedObject.Equals(SelectableObject))
                {
                    if (SelectableObject == null)
                    {
                        this.SelectableObjectSelectionManagerEventListener.OnSelectableObjectDeSelected(this.CurrentSelectedObject);
                    }
                    else
                    {
                        this.SelectableObjectSelectionManagerEventListener.OnSelectableObjectDeSelected(this.CurrentSelectedObject);
                        this.SelectableObjectSelectionManagerEventListener.OnSelectableObjectSelected(SelectableObject);
                    }
                }

            }
            else if (this.CurrentSelectedObject == null)
            {
                if (SelectableObject != null)
                {
                    this.SelectableObjectSelectionManagerEventListener.OnSelectableObjectSelected(SelectableObject);
                }
            }
            this.CurrentSelectedObject = SelectableObject;
        }

        protected void RemoveInteractiveObjectFromSelectable(T selectableObject)
        {
            if (this.CurrentSelectedObject != null && this.interactableObjects.Contains(this.CurrentSelectedObject))
            {
                this.SetCurrentSelectedObject(default(T));
            }
            this.interactableObjects.Remove(selectableObject);
        }

        protected void AddInteractiveObjectFromSelectable(T selectableObject)
        {
            if (!this.interactableObjects.Contains(selectableObject))
            {
                this.interactableObjects.Add(selectableObject);
            }
        }

        protected void ReplaceSelectableObjects(List<T> selectableObjects)
        {
            List<T> currentSelectableObjectsToRemove = null;
            foreach (var currentSelectableObject in this.interactableObjects)
            {
                if (!selectableObjects.Contains(currentSelectableObject))
                {
                    if (currentSelectableObjectsToRemove == null) { currentSelectableObjectsToRemove = new List<T>(); }
                    currentSelectableObjectsToRemove.Add(currentSelectableObject);
                }
            }

            if (currentSelectableObjectsToRemove != null)
            {
                foreach (var currentSelectableObjectToRemove in currentSelectableObjectsToRemove)
                {
                    this.RemoveInteractiveObjectFromSelectable(currentSelectableObjectToRemove);
                }
            }

            this.interactableObjects = selectableObjects;
        }

    }

    public interface SelectableObjectSelectionManagerEventListener<T> where T : ISelectable
    {
        void OnSelectableObjectSelected(T SelectableObject);
        void OnSelectableObjectDeSelected(T SelectableObject);
    }
}
