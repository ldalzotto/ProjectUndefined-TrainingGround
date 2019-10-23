using UnityEngine;
using System.Collections;
using CoreGame;
using Timelines;
using UnityEditor;

namespace Tests
{
    [System.Serializable]
    public class TestTimelineAction : TimeLineAction
    {
        public TestTimelineActionKey TestTimelineActionKey;

        public TestTimelineAction(TestTimelineActionKey testTimelineActionKey)
        {
            TestTimelineActionKey = testTimelineActionKey;
        }

        public override bool Equals(object obj)
        {
            return obj is TestTimelineAction action &&
                   TestTimelineActionKey == action.TestTimelineActionKey;
        }

        public override int GetHashCode()
        {
            return -82654653 + TestTimelineActionKey.GetHashCode();
        }

        public void NodeGUI()
        {
            this.TestTimelineActionKey = (TestTimelineActionKey) EditorGUILayout.EnumPopup(this.TestTimelineActionKey);
        }
    }

    public enum TestTimelineActionKey
    {
        TestTimelineActionKey1,
        TestTimelineActionKey2,
        TestTimelineActionKey3
    }
}