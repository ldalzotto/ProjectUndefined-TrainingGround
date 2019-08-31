using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CoreGame
{
    public abstract class AbstractSelectableObjectSelectionManager<T> : MonoBehaviour where T : AbstractSelectableObject
    {
        #region Internal State
        private T CurrentSelectedObject;
        private Dictionary<Object, T> interactableObjects;
        #endregion

        #region External Dependencies
        private GameInputManager GameInputManager;
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


        public virtual void Init()
        {
            #region Exnternal Dependencies
            var CoreMaterialConfiguration = CoreGameSingletonInstances.CoreStaticConfigurationContainer.CoreStaticConfiguration.CoreMaterialConfiguration;
            this.GameInputManager = CoreGameSingletonInstances.GameInputManager;
            #endregion

            this.InteractiveObjectSelectionRendererManager = new ObjectSelectionRendererManager(CoreMaterialConfiguration.OutlineImageEffectComputeShader, CoreMaterialConfiguration.OutlineColorShader, CoreMaterialConfiguration.BufferScreenSampleMaterial);
            this.interactableObjects = new Dictionary<Object, T>();
        }

        public void Tick(float d)
        {
            if (this.interactableObjects.Count > 0 && this.CurrentSelectedObject == null)
            {
                this.SetCurrentSelectedObject(this.interactableObjects.First().Value);
            }
            else if (this.CurrentSelectedObject != null && this.GameInputManager.CurrentInput.SwitchSelectionButtonD())
            {
                var availableSelectableObjectList = this.interactableObjects.Values.ToList();
                var currentSelectedIndex = availableSelectableObjectList.IndexOf(this.CurrentSelectedObject);
                int nextSelectedIndex = currentSelectedIndex + 1;
                if (nextSelectedIndex == availableSelectableObjectList.Count)
                {
                    nextSelectedIndex = 0;
                }
                this.SetCurrentSelectedObject(availableSelectableObjectList[nextSelectedIndex]);
            }
            else if (this.interactableObjects.Count == 0)
            {
                this.SetCurrentSelectedObject(null);
            }

            this.InteractiveObjectSelectionRendererManager.Tick(this.GetCurrentSelectedObject());
        }

        private void SetCurrentSelectedObject(T SelectableObject)
        {
            if (this.CurrentSelectedObject != SelectableObject)
            {
                if (this.CurrentSelectedObject != null)
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
                else if (this.CurrentSelectedObject == null)
                {
                    if (SelectableObject != null)
                    {
                        this.SelectableObjectSelectionManagerEventListener.OnSelectableObjectSelected(SelectableObject);
                    }
                }
            }


            this.CurrentSelectedObject = SelectableObject;
        }

        protected void RemoveInteractiveObjectFromSelectable(Object moduleToRemove)
        {
            if (this.CurrentSelectedObject != null && this.CurrentSelectedObject == this.interactableObjects[moduleToRemove])
            {
                this.SetCurrentSelectedObject(null);
            }
            this.interactableObjects.Remove(moduleToRemove);
        }

        protected void AddInteractiveObjectFromSelectable(Object moduleToAdd, T selectableObject)
        {
            this.interactableObjects.Add(moduleToAdd, selectableObject);
        }
    }

    public abstract class AbstractSelectableObject
    {
        public IRendererRetrievable ModelObjectModule;

        public AbstractSelectableObject(IRendererRetrievable modelObjectModule)
        {
            ModelObjectModule = modelObjectModule;
        }
    }

    public interface SelectableObjectSelectionManagerEventListener<T> where T : AbstractSelectableObject
    {
        void OnSelectableObjectSelected(T SelectableObject);
        void OnSelectableObjectDeSelected(T SelectableObject);
    }
}
