using CoreGame;
using InteractiveObjectTest;
using System;

namespace InteractiveObjectTest
{
    public class InteractiveObjectEventsManager : GameSingleton<InteractiveObjectEventsManager>
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
