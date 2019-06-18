using UnityEngine;
using System.Collections;
using GameConfigurationID;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CoreGame
{
    [System.Serializable]
    public class LevelCompletedTimelineAction : TimeLineAction
    {
        [SerializeField]
       // [SearchableEnum]
        private LevelZonesID completedLevelZone;

        public LevelCompletedTimelineAction()
        {
        }

        public LevelCompletedTimelineAction(LevelZonesID completedLevelZone)
        {
            this.completedLevelZone = completedLevelZone;
        }

        public override bool Equals(object obj)
        {
            return obj is LevelCompletedTimelineAction action &&
                   completedLevelZone == action.completedLevelZone;
        }

        public override int GetHashCode()
        {
            return -1721677994 + completedLevelZone.GetHashCode();
        }
#if UNITY_EDITOR
        public void NodeGUI()
        {
            EditorGUILayout.LabelField(this.GetType().Name);
            this.completedLevelZone = (LevelZonesID)EditorGUILayout.EnumPopup(this.completedLevelZone);
        }
#endif
    }
}

