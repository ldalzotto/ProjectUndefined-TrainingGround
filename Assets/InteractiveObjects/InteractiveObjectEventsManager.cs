using CoreGame;
using InteractiveObjects;
using System;

namespace InteractiveObjects
{
    public interface IInteractiveObjectEventsManager
    {
        void RegisterOnInteractiveObjectCreatedEventListener(Action<CoreInteractiveObject> action);
        void UpRegisterOnInteractiveObjectCreatedEventListener(Action<CoreInteractiveObject> action);
        void RegisterOnInteractiveObjectDestroyedEventListener(Action<CoreInteractiveObject> action);
        void UnRegisterOnInteractiveObjectDestroyedEventListener(Action<CoreInteractiveObject> action);
    }

    public static class InteractiveObjectEventsManagerSingleton
    {
        public static IInteractiveObjectEventsManager Get() { return InteractiveObjectEventsManager.Get(); }
    }

    internal class InteractiveObjectEventsManager : GameSingleton<InteractiveObjectEventsManager>, IInteractiveObjectEventsManager
    {
        private event Action<CoreInteractiveObject> OnInteractiveObjectCreatedEvent;
        private event Action<CoreInteractiveObject> OnInteractiveObjectDestroyedEvent;

        public void RegisterOnInteractiveObjectCreatedEventListener(Action<CoreInteractiveObject> action) { this.OnInteractiveObjectCreatedEvent += action; }
        public void UpRegisterOnInteractiveObjectCreatedEventListener(Action<CoreInteractiveObject> action) { this.OnInteractiveObjectCreatedEvent -= action; }
        public void RegisterOnInteractiveObjectDestroyedEventListener(Action<CoreInteractiveObject> action) { this.OnInteractiveObjectDestroyedEvent += action; }
        public void UnRegisterOnInteractiveObjectDestroyedEventListener(Action<CoreInteractiveObject> action) { this.OnInteractiveObjectDestroyedEvent -= action; }

        public void OnInteractiveObjectCreated(CoreInteractiveObject CoreInteractiveObject)
        {
            if (this.OnInteractiveObjectCreatedEvent != null) { this.OnInteractiveObjectCreatedEvent.Invoke(CoreInteractiveObject); }
        }

        public void OnInteractiveObjectDestroyed(CoreInteractiveObject CoreInteractiveObject)
        {
            if (this.OnInteractiveObjectDestroyedEvent != null) { this.OnInteractiveObjectDestroyedEvent.Invoke(CoreInteractiveObject); }
        }

    }

}
