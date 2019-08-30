using CoreGame;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace RTPuzzle
{
    public class InteractiveObjectSelectionManager : MonoBehaviour
    {
        public ComputeShader OutlineComputeShader;
        public Material OutlineColorShader;
        [FormerlySerializedAs("AddBlitShader")]
        public Material BufferScreenSampleShader;

        #region Internal State
        private SelectableObject CurrentSelectedObject;
        private Dictionary<InteractiveObjectModule, SelectableObject> interactableModules;
        #endregion

        #region External Dependencies
        private PlayerActionPuzzleEventsManager PlayerActionPuzzleEventsManager;
        private GameInputManager GameInputManager;
        #endregion

        #region Internal Managers
        private InteractiveObjectSelectionRendererManager InteractiveObjectSelectionRendererManager;
        #endregion

        #region Data Retrieval
        public SelectableObject GetCurrentSelectedObject()
        {
            return this.CurrentSelectedObject;
        }
        #endregion


        public void Init()
        {
            #region Exnternal Dependencies
            this.PlayerActionPuzzleEventsManager = GameObject.FindObjectOfType<PlayerActionPuzzleEventsManager>();
            this.GameInputManager = CoreGameSingletonInstances.GameInputManager;
            #endregion

            this.InteractiveObjectSelectionRendererManager = new InteractiveObjectSelectionRendererManager(this, this.OutlineComputeShader, this.OutlineColorShader, this.BufferScreenSampleShader);
            this.interactableModules = new Dictionary<InteractiveObjectModule, SelectableObject>();
        }

        public void Tick(float d)
        {
            if (this.interactableModules.Count > 0 && this.CurrentSelectedObject == null)
            {
                this.SetCurrentSelectedObject(this.interactableModules.First().Value);
            }
            else if (this.CurrentSelectedObject != null && this.GameInputManager.CurrentInput.SwitchSelectionButtonD())
            {
                var availableSelectableObjectList = this.interactableModules.Values.ToList();
                var currentSelectedIndex = availableSelectableObjectList.IndexOf(this.CurrentSelectedObject);
                int nextSelectedIndex = currentSelectedIndex + 1;
                if (nextSelectedIndex == availableSelectableObjectList.Count)
                {
                    nextSelectedIndex = 0;
                }
                this.SetCurrentSelectedObject(availableSelectableObjectList[nextSelectedIndex]);
            }
            else if (this.interactableModules.Count == 0)
            {
                this.SetCurrentSelectedObject(null);
            }

            this.InteractiveObjectSelectionRendererManager.Tick(d);
        }

        private void SetCurrentSelectedObject(SelectableObject SelectableObject)
        {
            if (this.CurrentSelectedObject != SelectableObject)
            {
                if (this.CurrentSelectedObject != null)
                {
                    if (SelectableObject == null)
                    {
                        this.PlayerActionPuzzleEventsManager.OnSelectableObjectDeSelected(this.CurrentSelectedObject);
                    }
                    else
                    {
                        this.PlayerActionPuzzleEventsManager.OnSelectableObjectDeSelected(this.CurrentSelectedObject);
                        this.PlayerActionPuzzleEventsManager.OnSelectableObjectSelected(SelectableObject);
                    }
                }
                else if (this.CurrentSelectedObject == null)
                {
                    if (SelectableObject != null)
                    {
                        this.PlayerActionPuzzleEventsManager.OnSelectableObjectSelected(SelectableObject);
                    }
                }
            }


            this.CurrentSelectedObject = SelectableObject;
        }

        #region External Events
        public void OnActionInteractableEnter(ActionInteractableObjectModule actionInteractableObjectModule)
        {
            this.interactableModules.Add(actionInteractableObjectModule, new SelectableObject(actionInteractableObjectModule.GetModelObjectModule(), actionInteractableObjectModule.AssociatedPlayerAction));
        }

        public void OnActionInteractableExit(ActionInteractableObjectModule actionInteractableObjectModule)
        {
            this.RemoveInteractiveObjectModuleFromSelectable(actionInteractableObjectModule);
        }

        public void OnGrabObjectEnter(GrabObjectModule grabObjectModule)
        {
            this.interactableModules.Add(grabObjectModule, new SelectableObject(grabObjectModule.ModelObjectModule, grabObjectModule.GrabObjectAction));
        }

        public void OnGrabObjectExit(GrabObjectModule grabObjectModule)
        {
            this.RemoveInteractiveObjectModuleFromSelectable(grabObjectModule);
        }
        #endregion

        private void RemoveInteractiveObjectModuleFromSelectable(InteractiveObjectModule moduleToRemove)
        {
            if (this.CurrentSelectedObject != null && this.CurrentSelectedObject == this.interactableModules[moduleToRemove])
            {
                this.SetCurrentSelectedObject(null);
            }
            this.interactableModules.Remove(moduleToRemove);
        }
    }

    public class SelectableObject
    {
        public ModelObjectModule ModelObjectModule;
        public RTPPlayerAction AssociatedPlayerAction;

        public SelectableObject(ModelObjectModule modelObjectModule, RTPPlayerAction associatedPlayerAction)
        {
            ModelObjectModule = modelObjectModule;
            AssociatedPlayerAction = associatedPlayerAction;
        }
    }
}
