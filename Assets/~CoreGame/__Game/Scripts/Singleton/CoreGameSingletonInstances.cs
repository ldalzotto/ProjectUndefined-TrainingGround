using System;
using UnityEngine;

namespace CoreGame
{
    public static class CoreGameSingletonInstances
    {
        private static GameInputManager gameInputManager;
        private static Coroutiner coroutiner;
        private static CoreConfigurationManager coreConfigurationManager;
        private static CoreStaticConfigurationContainer coreStaticConfigurationContainer;
        private static DiscussionPositionsType discussionPositionsType;
        private static Canvas persistantCanvas;

        public static GameInputManager GameInputManager => FindAndSetInstanceIfNull(gameInputManager, obj => gameInputManager = obj);

        public static Coroutiner Coroutiner => FindAndSetInstanceIfNull(coroutiner, obj => coroutiner = obj);
        public static CoreConfigurationManager CoreConfigurationManager => FindAndSetInstanceIfNull(coreConfigurationManager, obj => coreConfigurationManager = obj);

        public static CoreStaticConfigurationContainer CoreStaticConfigurationContainer => FindAndSetInstanceIfNull(coreStaticConfigurationContainer, obj => coreStaticConfigurationContainer = obj);

        public static Canvas GameCanvas
        {
            get
            {
                var gameCanvas = GameObject.FindGameObjectWithTag(TagConstants.GAME_CANVAS);
                if (gameCanvas == null)
                {
                    gameCanvas = GameObject.FindGameObjectWithTag(TagConstants.START_MENU_CANVAS);
                }

                return gameCanvas.GetComponent<Canvas>();
            }
        }

        public static Canvas PersistantCanvas()
        {
            if (persistantCanvas == null)
            {
                persistantCanvas = GameObject.FindGameObjectWithTag(TagConstants.PERSISTANT_CANVAS).GetComponent<Canvas>();
            }

            return persistantCanvas;
        }

        public static DiscussionPositionsType DiscussionPositionsType => FindAndSetInstanceIfNull(discussionPositionsType, obj => discussionPositionsType = obj);

        public static T FindAndSetInstanceIfNull<T>(T obj, Action<T> setter) where T : Behaviour
        {
            if (obj == null)
            {
                var foundObj = GameObject.FindObjectOfType<T>();
                setter.Invoke(foundObj);
                return foundObj;
            }
            else
            {
                return obj;
            }
        }

        public static T NewInstanceIfNull<T>(T obj, Action<T> setter)
        {
            if (obj == null)
            {
                var createdObject = Activator.CreateInstance<T>();
                setter.Invoke(createdObject);
                return createdObject;
            }
            else
            {
                return obj;
            }
        }
    }
}