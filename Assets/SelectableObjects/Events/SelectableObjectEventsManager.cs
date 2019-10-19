using System;
using CoreGame;

namespace SelectableObject
{
    public class SelectableObjectEventsManager : GameSingleton<SelectableObjectEventsManager>
    {
        private event Action<ISelectableObjectSystem> OnSelectableObjectSelectedEvent;
        private event Action<ISelectableObjectSystem> OnSelectableObjectNoMoreSelectedEvent;

        public void RegisterOnSelectableObjectSelectedEventAction(Action<ISelectableObjectSystem> action)
        {
            OnSelectableObjectSelectedEvent += action;
        }

        public void RegisterOnSelectableObjectNoMoreSelectedEventAction(Action<ISelectableObjectSystem> action)
        {
            OnSelectableObjectNoMoreSelectedEvent += action;
        }

        public void OnSelectableObjectSelected(ISelectableObjectSystem selectableObject)
        {
            if (OnSelectableObjectSelectedEvent != null) OnSelectableObjectSelectedEvent.Invoke(selectableObject);
        }

        public void OnSelectableObjectNoMoreSelected(ISelectableObjectSystem selectableObject)
        {
            if (OnSelectableObjectNoMoreSelectedEvent != null) OnSelectableObjectNoMoreSelectedEvent.Invoke(selectableObject);
        }

        private event Action<ISelectableObjectSystem> OnSelectableObjectEnterEvent;
        private event Action<ISelectableObjectSystem> OnSelectableObjectExitEvent;

        public void RegisterOnSelectableObjectEnterEventAction(Action<ISelectableObjectSystem> action)
        {
            OnSelectableObjectEnterEvent += action;
        }

        public void RegisterOnSelectableObjectExitEventAction(Action<ISelectableObjectSystem> action)
        {
            OnSelectableObjectExitEvent += action;
        }

        public void OnSelectableObjectEnter(ISelectableObjectSystem selectableObject)
        {
            if (OnSelectableObjectEnterEvent != null) OnSelectableObjectEnterEvent.Invoke(selectableObject);
        }

        public void OnSelectableObjectExit(ISelectableObjectSystem selectableObject)
        {
            if (OnSelectableObjectExitEvent != null) OnSelectableObjectExitEvent.Invoke(selectableObject);
        }
    }
}