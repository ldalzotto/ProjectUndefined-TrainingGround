using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.Assertions;

namespace Tests
{
    public class AbstractTimelineTest : MonoBehaviour
    {
        private IEnumerator Before()
        {
            SceneManager.LoadScene("AbstractTimelineSceneTest", LoadSceneMode.Single);
            yield return null;
            GameObject.FindObjectOfType<TestTimelineManager>().Init();
        }

        [UnityTest]
        public IEnumerator TimelineFLowTest()
        {
            yield return this.Before();
            var TestTimelineManager = GameObject.FindObjectOfType<TestTimelineManager>();

            Assert.IsTrue(TestTimelineManager.Nodes.Count == 1);
            Assert.IsTrue(TestTimelineManager.Nodes[0] == TestTimelineKey.TestTimeline1);
            Assert.IsTrue(TestTimelineManager.TestTimelineContext.CallCounter == 0);

            //increment doing nothing
            TestTimelineManager.IncrementGraph(new TestTimelineAction(TestTimelineActionKey.TestTimelineActionKey2));
            Assert.IsTrue(TestTimelineManager.Nodes.Count == 1);
            Assert.IsTrue(TestTimelineManager.Nodes[0] == TestTimelineKey.TestTimeline1);
            Assert.IsTrue(TestTimelineManager.TestTimelineContext.CallCounter == 0);

            //increment to next level
            TestTimelineManager.IncrementGraph(new TestTimelineAction(TestTimelineActionKey.TestTimelineActionKey1));
            Assert.IsTrue(TestTimelineManager.Nodes.Count == 2);
            Assert.IsTrue(TestTimelineManager.Nodes[0] == TestTimelineKey.TestTimeline2);
            Assert.IsTrue(TestTimelineManager.Nodes[1] == TestTimelineKey.TestTimeline3);
            Assert.IsTrue(TestTimelineManager.TestTimelineContext.CallCounter == 2);

            //increment doing nothing
            TestTimelineManager.IncrementGraph(new TestTimelineAction(TestTimelineActionKey.TestTimelineActionKey1));
            Assert.IsTrue(TestTimelineManager.Nodes.Count == 2);
            Assert.IsTrue(TestTimelineManager.Nodes[0] == TestTimelineKey.TestTimeline2);
            Assert.IsTrue(TestTimelineManager.Nodes[1] == TestTimelineKey.TestTimeline3);
            Assert.IsTrue(TestTimelineManager.TestTimelineContext.CallCounter == 2);

            //increment to next level
            TestTimelineManager.IncrementGraph(new TestTimelineAction(TestTimelineActionKey.TestTimelineActionKey2));
            Assert.IsTrue(TestTimelineManager.Nodes.Count == 2);
            Assert.IsTrue(TestTimelineManager.Nodes[0] == TestTimelineKey.TestTimeline2);
            Assert.IsTrue(TestTimelineManager.Nodes[1] == TestTimelineKey.TestTimeline4);
            Assert.IsTrue(TestTimelineManager.TestTimelineContext.CallCounter == 3);


            //increment to end level
            TestTimelineManager.IncrementGraph(new TestTimelineAction(TestTimelineActionKey.TestTimelineActionKey3));
            Assert.IsTrue(TestTimelineManager.Nodes.Count == 1);
            Assert.IsTrue(TestTimelineManager.Nodes[0] == TestTimelineKey.TestTimeline5);
            Assert.IsTrue(TestTimelineManager.TestTimelineContext.CallCounter == 3);
        }
    }

}
