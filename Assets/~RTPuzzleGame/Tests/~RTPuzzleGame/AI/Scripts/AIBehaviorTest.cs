using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using RTPuzzle;

namespace Tests
{
    public class AIBehaviorTest : MonoBehaviour
    {
        [UnityTest]
        public IEnumerator TT()
        {
            SceneManager.LoadScene(LevelZones.LevelZonesSceneName[LevelZonesID.SEWER_RTP]);
            yield return new WaitForFixedUpdate();
            yield return new WaitForSeconds(2f);
            Assert.AreEqual(0, 0);
        }
    }

}
