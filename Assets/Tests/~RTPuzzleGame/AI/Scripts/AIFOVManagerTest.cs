using NUnit.Framework;
using RTPuzzle;
using System.Collections.Generic;
using UnityEngine.AI;

namespace Tests
{
    public class AIFOVManagerTest
    {

        private AIFOVManager InitializeFOVManager()
        {
            NavMeshAgent agent = new NavMeshAgent();
            return new AIFOVManager(agent);
        }

        [Test]
        public void AIFOVManagerTest_Intersect_1stPass_UpAndDown()
        {
            var aiFOVManager = InitializeFOVManager();
            var cuttedFOV = aiFOVManager.IntersectFOV(-128f, 51f);
            Assert.AreEqual(new List<FOVSlice>() { new FOVSlice(232f, 360f), new FOVSlice(0f, 51f) }, cuttedFOV);
        }
        [Test]
        public void AIFOVManagerTest_Intersect_1stPass_Up()
        {
            var aiFOVManager = InitializeFOVManager();
            var cuttedFOV = aiFOVManager.IntersectFOV(190f, 260);
            Assert.AreEqual(new List<FOVSlice>() { new FOVSlice(190f, 260) }, cuttedFOV);
        }

        [Test]
        public void AIFOVManagerTest_Intersect_1stPass_Down()
        {
            var aiFOVManager = InitializeFOVManager();
            var cuttedFOV = aiFOVManager.IntersectFOV(260f, 190f);
            Assert.AreEqual(new List<FOVSlice>() { new FOVSlice(260f, 190f) }, cuttedFOV);
        }

        [Test]
        public void AIFOVManagerTest_Intersect_1stPass_UpAndDown_2ndPass_UpOutOfRange()
        {
            var aiFOVManager = InitializeFOVManager();
            var cuttedFOV = aiFOVManager.IntersectFOV(-124f, 55f);
            Assert.AreEqual(new List<FOVSlice>() { new FOVSlice(236f, 360f), new FOVSlice(0f, 55f) }, cuttedFOV);
            var cuttedFOV2ndPass = aiFOVManager.IntersectFOV(-223f, -43f);
            Assert.AreEqual(new List<FOVSlice>() { new FOVSlice(236f, 317f) }, cuttedFOV2ndPass);
        }

    }
}
